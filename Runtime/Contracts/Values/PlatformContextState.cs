namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// How far <see cref="IPlatformContext.Initialize"/> got.
    /// </summary>
    /// <remarks>
    /// <see cref="Failed"/> exists because initialization has legitimate non-exceptional failures:
    /// no world is registered for this app, the user cannot access the world the launch data named,
    /// or no world was chosen. None of those is a bug, and none should arrive as an exception.
    /// </remarks>
    public enum PlatformContextState
    {
        /// <summary>Nothing has been attempted, or the context was disposed.</summary>
        NotReady = 0,

        /// <summary>Initialization is in progress. No getter is meaningful yet.</summary>
        Connecting = 1,

        /// <summary>Usable. Every getter answers.</summary>
        Ready = 2,

        /// <summary>Initialization did not complete. See <see cref="IPlatformContext.FailureReason"/>.</summary>
        Failed = 3,

        /// <summary>
        /// Was ready and is not any more — the realtime connection dropped, the user was kicked, or
        /// the same account signed in elsewhere.
        /// </summary>
        Disconnected = 4,
    }
}
