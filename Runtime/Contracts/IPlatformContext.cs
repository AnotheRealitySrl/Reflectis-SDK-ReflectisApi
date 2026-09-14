using Virtuademy.SDK.ApiData;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// Picks a world when the platform offers an app more than one. Returning null fails
    /// initialization with <see cref="PlatformContextState.Failed"/>.
    /// </summary>
    /// <remarks>
    /// It fires **zero times** in the two common cases — the launch data already names a world, or
    /// the app's identity resolves to exactly one. An app that remembers the last choice can return
    /// it without showing anything. That is the point: whether the app was opened from the catalog
    /// or started on its own stays invisible to it.
    /// </remarks>
    public delegate Task<ExternalAppPlacementDTO> WorldChooser(IReadOnlyList<ExternalAppPlacementDTO> available);

    /// <summary>
    /// Platform and session state, as an app sees it: who the user is, which world and session they
    /// are in, what they are allowed to do, who else is present, and their own saved data.
    /// </summary>
    /// <remarks>
    /// <b>Scope, and what it deliberately excludes.</b> Avatars, hands, grab, ownership, teleport,
    /// camera, sync vars, RPC, network spawn, nodes and tasks are not here. They belong to other
    /// systems and get their own contracts beside this one. This interface must not become the
    /// container for everything.
    /// <para>
    /// Also excluded, for a different reason: the users directory
    /// (<c>GetAllUsers</c>, <c>SearchUsersByNickname</c>, <c>GetUserData</c>, <c>GetUsersWithTag</c>),
    /// the presence of *other* sessions, and the catalog. An app holding those can enumerate the
    /// tenant, which is not something an app needs to do its job.
    /// </para>
    /// <para>
    /// Every operation here reads state, with one deliberate exception: the save-data members write,
    /// but only the caller's own data.
    /// </para>
    /// <para>
    /// <b>It returns the platform's own wire types, and an earlier draft mirrored them.</b> There
    /// was a parallel set of value types here — <c>PlatformUser</c>, <c>PlatformSession</c> and six
    /// more — with a projection mapping each DTO onto its twin. The argument for it was the
    /// perimeter: naming <c>SessionDTO</c> would make this assembly reference the one holding the
    /// platform client, and a creator installing these contracts would get the client with them.
    /// </para>
    /// <para>
    /// That argument was true only because the DTOs and the client shared an assembly, which was an
    /// accident of layout. They no longer do: the DTOs are their own assembly, referenced here,
    /// while the transport, the credential and the sixty endpoints stay behind in another that
    /// nothing here names. The perimeter the plan calls invariant 4a — no transport and no
    /// credential in the contracts — holds without a single mirrored type.
    /// </para>
    /// <para>
    /// What the mirror cost was paid in full before it came out: four members had to be dropped for
    /// having no wire source at all, two types had to be renamed for colliding with the DTO family
    /// they duplicated, and every field added to a DTO would have had to be added twice. What it
    /// bought that is worth naming is immutability — so the five DTOs this interface hands back are
    /// now read-only, and the one place that wrote to one has been changed to stop.
    /// </para>
    /// </remarks>
    public interface IPlatformContext
    {
        #region Lifecycle

        /// <summary>
        /// Brings the context to <see cref="PlatformContextState.Ready"/>: resolves the world,
        /// opens the realtime connection, joins or creates the session, loads permissions and the
        /// caller's saved data.
        /// </summary>
        /// <param name="chooseWorld">
        /// Consulted only when more than one world is available. Null means "fail instead of
        /// asking", which is the right choice for an app that expects exactly one.
        /// </param>
        /// <remarks>
        /// One call, replacing nine that the caller previously had to know *and order*. An
        /// authenticated user is a precondition — the world list is filtered to that user and the
        /// session is created with an owner — so an app doing its own login authenticates first,
        /// through <see cref="IPlatformAuthentication"/>.
        /// <para>
        /// <b>The app does not pass its own identity, and that is settled rather than pending.</b>
        /// An earlier draft took an app scheme here. The platform reads the app from its token's
        /// <c>azp</c> claim instead and matches it server-side against the identity recorded on each
        /// published experience — so there is no second copy of an app identity to keep in sync, and
        /// an app cannot ask about a different app. A parameter would have been a value the server
        /// ignores, which is worse than no parameter: it looks load-bearing.
        /// </para>
        /// <para>
        /// Failure here is often not exceptional: this app is published in no world this user can
        /// enter, the user cannot access the world named in the launch data, or no world was chosen.
        /// Those end in <see cref="PlatformContextState.Failed"/> with a
        /// <see cref="FailureReason"/>, not in a thrown exception.
        /// </para>
        /// </remarks>
        Task Initialize(WorldChooser chooseWorld = null);

        /// <summary>
        /// How far initialization got, and whether the context is usable.
        /// </summary>
        /// <remarks>
        /// Readiness used to be implicit — getters returned null until they did not, and the
        /// implementation span-waited internally on two of them. An app could not tell "not yet"
        /// from "not available".
        /// </remarks>
        PlatformContextState State { get; }

        /// <summary>
        /// Why <see cref="State"/> is <see cref="PlatformContextState.Failed"/>, or null.
        /// Human-readable; not a code to branch on.
        /// </summary>
        string FailureReason { get; }

        /// <summary>
        /// Raised whenever <see cref="State"/> changes.
        /// </summary>
        /// <remarks>
        /// Being kicked, being disconnected, and a second login on the same account are all state
        /// transitions rather than separate callbacks. They used to be private callbacks handed to
        /// the realtime system, and teardown told nobody at all.
        /// </remarks>
        event Action StateChanged;

        /// <summary>
        /// Leaves the session and closes the realtime connection. The context returns to
        /// <see cref="PlatformContextState.NotReady"/> and may be initialized again.
        /// </summary>
        Task Dispose();

        #endregion

        #region Identity, session, experience, world

        /// <summary>The signed-in user. Null before <see cref="PlatformContextState.Ready"/>.</summary>
        UserDTO LocalUser { get; }

        /// <summary>The session this client is in.</summary>
        SessionDTO Session { get; }

        /// <summary>
        /// This client's realtime connection id, from the handshake.
        /// </summary>
        /// <remarks>
        /// Named for what it is. Three unrelated things were called "session" in the interface this
        /// replaces, in the same file: the session's <c>int</c> id, this string, and the
        /// authentication session hash.
        /// </remarks>
        string ConnectionId { get; }

        /// <summary>Raised when <see cref="Session"/> is replaced.</summary>
        event Action SessionChanged;

        /// <summary>
        /// The experience this app is running as — first-class, because it is the concept an
        /// external app *is* on the platform, not a detail of the session it happens to be in.
        /// </summary>
        ExperienceDTO Experience { get; }

        /// <summary>
        /// The world this session belongs to. Never null once <see cref="PlatformContextState.Ready"/>:
        /// without a world, initialization does not complete.
        /// </summary>
        WorldDTO World { get; }

        #endregion

        #region Permissions

        /// <summary>Whether the local user may do <paramref name="permission"/> here and now.</summary>
        bool IsGranted(PlatformPermission permission);

        /// <summary>
        /// Everything the local user is granted in this session and world, as one list.
        /// </summary>
        /// <remarks>
        /// The platform answers with a session set and a world set; that split is the shape of the
        /// response, not a distinction a consumer has any use for, so it is joined here.
        /// </remarks>
        IReadOnlyList<PlatformPermission> Permissions { get; }

        /// <summary>Raised when the granted set changes.</summary>
        /// <remarks>
        /// An <c>event</c> rather than an assignable delegate. The member this replaces was a
        /// settable <c>Action</c> property, so the last subscriber silently wiped every other one.
        /// </remarks>
        event Action PermissionsChanged;

        #endregion

        #region Participants

        /// <summary>Everyone currently in this session, including the local user.</summary>
        IReadOnlyList<OnlineUserDTO> Participants { get; }

        /// <summary>One participant by platform user id.</summary>
        bool TryGetParticipant(int userId, out OnlineUserDTO participant);

        /// <summary>Raised when someone joins.</summary>
        /// <remarks>
        /// Derived, not received. The platform pushes the whole roster on every change with no diff,
        /// so join and leave are computed by the implementation. An app that wants to react to one
        /// person arriving should not have to compare two lists to find out who.
        /// </remarks>
        event Action<OnlineUserDTO> ParticipantJoined;

        /// <summary>Raised when someone leaves. Also derived — see <see cref="ParticipantJoined"/>.</summary>
        event Action<OnlineUserDTO> ParticipantLeft;

        /// <summary>Raised on any change to <see cref="Participants"/>.</summary>
        event Action ParticipantsChanged;

        #endregion

        #region Shards

        /// <summary>The shard this client is in, or null when the session is not sharded.</summary>
        SessionShard Shard { get; }

        /// <summary>Every shard of this session.</summary>
        IReadOnlyList<SessionShard> Shards { get; }

        /// <summary>Raised when shard membership or capacity changes.</summary>
        event Action ShardsChanged;

        #endregion

        #region Save data

        /// <summary>
        /// Reads one of the local user's saved values.
        /// </summary>
        /// <remarks>
        /// Scoped to <see cref="World"/>, not global — the store is loaded per world during
        /// initialization. Typed, because the untyped predecessor made every cast the caller's
        /// problem.
        /// </remarks>
        bool TryGetSaveData<T>(string key, out T value);

        /// <summary>
        /// Writes one of the local user's saved values.
        /// </summary>
        /// <remarks>
        /// Returns a <see cref="Task"/> so a failed write is observable. Its predecessor was
        /// <c>async void</c>: it updated the local cache optimistically and fired the request, and
        /// if that request failed nobody found out — the cache simply disagreed with the server from
        /// then on.
        /// </remarks>
        Task SetSaveData<T>(string key, T value);

        /// <summary>Deletes one of the local user's saved values.</summary>
        Task DeleteSaveData(string key);

        #endregion
    }
}
