using Virtuademy.SDK.Interface;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    /// <summary>
    /// The one projection the platform context still needs.
    /// </summary>
    /// <remarks>
    /// This class used to map every wire type onto a contract twin — a user, a session, a world, an
    /// experience, a participant, three enums. All of that is gone: <c>IPlatformContext</c> returns
    /// the DTOs themselves, now that they live in an assembly the contracts can reference without
    /// dragging the client along.
    /// <para>
    /// What is left is the one case where there is genuinely nothing to return. The permission
    /// endpoints answer with a list of identifier <b>strings</b>; there is no DTO, and a caller
    /// cannot be handed raw text and be expected to compare it correctly. So the strings become
    /// <see cref="PlatformPermission"/> members, and the mapping is a real conversion rather than a
    /// re-wrap.
    /// </para>
    /// </remarks>
    public static class PlatformContextProjection
    {
        /// <summary>
        /// The granted set, from the facet identifiers the platform returns.
        /// </summary>
        /// <remarks>
        /// <b>Matched by name, not by number,</b> and case-sensitively. Renaming a
        /// <see cref="PlatformPermission"/> member therefore breaks this silently — the name stops
        /// matching, the permission stops being granted, and it reads as a permissions bug rather
        /// than as a rename. That is not hypothetical: the member for the leaderboard facet was
        /// singular where the platform's identifier is plural, so that one permission had never
        /// resolved in any client until it was measured against a live session.
        /// <para>
        /// An unrecognised identifier is logged and dropped, which is the existing behaviour and
        /// the right one: a facet this client has never heard of should not grant anything, and it
        /// should not stop the others from being read either.
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
        /// Whether a session status is one a client may be told about.
        /// </summary>
        /// <remarks>
        /// <c>Expired</c> and <c>Empty</c> are server-side bookkeeping and must never describe a
        /// session a client is in. With the wire enum now exposed directly, there is no mapping step
        /// left to filter them in — so the check is explicit, and the caller refuses rather than
        /// relabels. The conversion this replaces handled three of the five values in a
        /// <c>switch</c> with no <c>default</c> over an initializer of <c>Persistent</c>, so those
        /// two reached the client disguised as a permanent session.
        /// </remarks>
        public static bool IsClientFacing(ESessionStatus status)
        {
            return status == ESessionStatus.OnTheFly
                || status == ESessionStatus.Scheduled
                || status == ESessionStatus.Persistent;
        }
    }
}
