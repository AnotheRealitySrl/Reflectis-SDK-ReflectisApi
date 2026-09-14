namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// What an app receives when the platform launches it, as opposed to what it works out for
    /// itself. Every field is absent when the app was started on its own.
    /// </summary>
    /// <remarks>
    /// This is the input side of the "three inputs, three provenances" split: the app always knows
    /// its own identity, while the world and the session either arrive here or get resolved and
    /// created by the implementation. An app does not read this type — the implementation does, and
    /// that is what keeps the two launch paths indistinguishable from the app's side.
    /// </remarks>
    public sealed class PlatformLaunchData
    {
        public PlatformLaunchData(string authSessionHash, int? worldId, int? experienceId)
        {
            AuthSessionHash = authSessionHash;
            WorldId = worldId;
            ExperienceId = experienceId;
        }

        /// <summary>
        /// The authentication session to restore. Takes precedence over any persisted session — see
        /// <see cref="IPlatformAuthentication"/> for why that order is not negotiable.
        /// </summary>
        public string AuthSessionHash { get; }

        /// <summary>The world to enter, skipping resolution from the app identity.</summary>
        public int? WorldId { get; }

        /// <summary>
        /// The experience to join. An id here means "join the session behind this experience";
        /// its absence means the implementation creates one.
        /// </summary>
        public int? ExperienceId { get; }
    }
}
