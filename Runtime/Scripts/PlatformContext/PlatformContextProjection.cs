using Newtonsoft.Json;

using Virtuademy.SDK.Interface;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    /// <summary>
    /// Projects the platform's wire types onto the contract types in
    /// <c>Virtuademy.SDK.Interface</c>. Pure functions: nothing here fetches, caches or mutates.
    /// </summary>
    /// <remarks>
    /// This is the field-level half of the adapter. It is separate from the orchestration on
    /// purpose — the projection is where every fidelity question lives, and none of those questions
    /// need a running session to answer.
    /// <para>
    /// It projects from the <b>DTOs</b> and not from the <c>CM*</c> client models, even though the
    /// contract's field lists were derived from those models. The models live in the Creator Kit
    /// package, which an external app developer does not install; projecting from them would put a
    /// creator-only package in the dependency path of every external app. The cost is that a few
    /// fields the models assemble from more than one source have to be assembled here too, and the
    /// parameters below are exactly those cases.
    /// </para>
    /// </remarks>
    public static class PlatformContextProjection
    {
        /// <summary>
        /// The signed-in user, or another participant's user record.
        /// </summary>
        /// <remarks>
        /// The nickname is not a field on <see cref="UserDTO"/> — it lives inside the untyped
        /// <c>preferences</c> blob, so it has to be deserialized out. The client-model equivalent
        /// does the same thing but calls <c>.ToString()</c> on that blob without a null check,
        /// which throws for a user who has never saved a preference. This one returns an empty
        /// nickname instead.
        /// </remarks>
        public static PlatformUser ToPlatformUser(UserDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            string nickname = ReadNickname(dto.Preferences);

            return new PlatformUser(dto.Id, nickname, nickname, dto.Code ?? 0, dto.Email);
        }

        /// <summary>
        /// The user record carried by a presence entry. <see cref="OnlineUserDTO"/> is not a
        /// <see cref="UserDTO"/> — it repeats some of the same fields and carries no nested user —
        /// so this is a second projection rather than a reuse of the first.
        /// </summary>
        public static PlatformUser ToPlatformUser(OnlineUserDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            string nickname = ReadNickname(dto.Preferences);

            return new PlatformUser(dto.UserId, nickname, nickname, dto.Code ?? 0, dto.Email);
        }

        /// <param name="localUserId">
        /// Needed for <c>IsOwner</c>: the wire carries an owner id, and whether that is "me" is not
        /// something a session knows about itself.
        /// </param>
        public static PlatformSession ToPlatformSession(SessionDTO dto, int localUserId)
        {
            if (dto == null)
            {
                return null;
            }

            return new PlatformSession(
                id: dto.Id,
                title: dto.Label,
                status: ToSessionStatus(dto.Status),
                isLobby: dto.Lobby,
                multiplayer: dto.Multiplayer,
                maxParticipants: dto.Capacity,
                isPublic: dto.Accessibility == ESessionAccessibility.Open,
                isOwner: dto.OwnerUserId == localUserId,
                startDateTime: dto.StartDate,
                endDateTime: dto.EndDate);
        }

        public static PlatformWorld ToPlatformWorld(WorldDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            // Note is the world's description on the wire; there is no separate description field.
            return new PlatformWorld(dto.Id, dto.Label, dto.Note, dto.ThumbnailUri, dto.Multiplayer);
        }

        /// <summary>
        /// The world a publication points at, from the app-worlds answer.
        /// </summary>
        /// <remarks>
        /// The placement carries exactly the five fields <see cref="PlatformWorld"/> has, which is
        /// not a coincidence — the endpoint was specified to return "enough of that world to render
        /// a chooser without a second round trip". So resolving an app's worlds costs **one**
        /// request, not one plus a world lookup per candidate.
        /// <para>
        /// It is also the confirmation that dropping <c>MaxOnlineUsers</c> from the contract was
        /// right: the placement deliberately excludes world configuration, so a field sourced from
        /// the world's limits could never have been filled on this path.
        /// </para>
        /// </remarks>
        public static PlatformWorld ToPlatformWorld(ExternalAppPlacementDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new PlatformWorld(dto.WorldId,
                                     dto.WorldLabel,
                                     dto.WorldDescription,
                                     dto.WorldThumbnailUri,
                                     dto.Multiplayer);
        }

        /// <param name="localUserId">Needed for <c>IsOwner</c>, as with the session.</param>
        public static PlatformExperience ToPlatformExperience(ExperienceDTO dto, int localUserId)
        {
            if (dto == null)
            {
                return null;
            }

            return new PlatformExperience(
                id: dto.Id,
                title: dto.Label,
                description: dto.Description,
                type: (ExperienceType)(int)dto.Type,
                isPublic: dto.Status == ExperienceDTO.EExperienceStatusOption.Published,
                isOwner: dto.OwnerUserId == localUserId,
                lastUpdate: dto.LastUpdate);
        }

        public static SessionParticipant ToSessionParticipant(OnlineUserDTO dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new SessionParticipant(
                user: ToPlatformUser(dto),
                connectionId: dto.ConnectionId,
                sessionId: dto.SessionId,
                worldId: dto.WorldId,
                shard: dto.ShardNumber,
                platform: ToParticipantPlatform(dto.Platform));
        }

        /// <summary>
        /// The granted set, from the facet identifiers the platform returns.
        /// </summary>
        /// <remarks>
        /// <b>Matched by name, not by number.</b> The platform answers
        /// <c>GET /my/session-permissions</c> and <c>/my/world-permissions</c> with a list of
        /// <em>strings</em>, and the match is case-sensitive. So renaming a
        /// <see cref="PlatformPermission"/> member breaks this silently — the name simply stops
        /// matching and the permission stops being granted, which reads as a permissions bug rather
        /// than a rename.
        /// <para>
        /// An unrecognised identifier is logged and dropped, which is the existing behaviour and the
        /// right one: a new facet the client has never heard of should not grant anything, and it
        /// should not stop the other permissions from being read either.
        /// </para>
        /// </remarks>
        public static IReadOnlyList<PlatformPermission> ToPlatformPermissions(IEnumerable<string> facetIdentifiers)
        {
            List<PlatformPermission> permissions = new();

            if (facetIdentifiers == null)
            {
                return permissions;
            }

            foreach (string identifier in facetIdentifiers)
            {
                if (Enum.TryParse(identifier, ignoreCase: false, out PlatformPermission permission))
                {
                    permissions.Add(permission);
                }
                else
                {
                    Debug.LogError($"[{nameof(PlatformContextProjection)}] unknown facet identifier " +
                                   $"'{identifier}'; the permission it names will not be granted. " +
                                   $"Either the platform added a facet this client predates, or a " +
                                   $"{nameof(PlatformPermission)} member was renamed.");
                }
            }

            return permissions;
        }

        /// <summary>
        /// The session's kind, with the two server-side statuses refused.
        /// </summary>
        /// <remarks>
        /// <c>Expired</c> and <c>Empty</c> exist on the wire but are server-side bookkeeping and
        /// must never describe a session a client is in. The conversion this replaces handled three
        /// of the five values in a <c>switch</c> with no <c>default</c>, over an initializer of
        /// <c>Persistent</c> — so those two arrived at the client silently relabelled as a permanent
        /// session, which is a server-side leak wearing a plausible disguise. Here they are reported.
        /// </remarks>
        public static SessionStatus ToSessionStatus(ESessionStatus status)
        {
            switch (status)
            {
                case ESessionStatus.OnTheFly:
                    return SessionStatus.OnTheFly;

                case ESessionStatus.Scheduled:
                    return SessionStatus.Scheduled;

                case ESessionStatus.Persistent:
                    return SessionStatus.Persistent;

                default:
                    Debug.LogError($"[{nameof(PlatformContextProjection)}] session status '{status}' " +
                                   "should never reach a client — it is server-side state. Reporting the " +
                                   "session as Persistent to keep going, but this is a leak on the " +
                                   "platform side and should be raised there.");
                    return SessionStatus.Persistent;
            }
        }

        private static ParticipantPlatform ToParticipantPlatform(string platform)
        {
            // A string on the wire rather than an enum, so an unknown value is expected rather than
            // exceptional and simply means "not reported".
            return Enum.TryParse(platform, ignoreCase: true, out ParticipantPlatform parsed)
                ? parsed
                : ParticipantPlatform.None;
        }

        /// <summary>
        /// Digs the nickname out of the untyped preferences blob. Tolerates it being absent,
        /// null, or not an object at all.
        /// </summary>
        private static string ReadNickname(object preferences)
        {
            if (preferences == null)
            {
                return string.Empty;
            }

            try
            {
                NicknamePreference parsed = JsonConvert.DeserializeObject<NicknamePreference>(preferences.ToString());
                return parsed?.Nickname ?? string.Empty;
            }
            catch (JsonException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// The one field of the preferences blob this projection needs. Deliberately not the whole
        /// preference model: that lives in the Creator Kit package, and depending on it here would
        /// undo the reason this projects from DTOs in the first place.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private sealed class NicknamePreference
        {
            [JsonProperty("nickname")]
            public string Nickname { get; set; }
        }
    }
}
