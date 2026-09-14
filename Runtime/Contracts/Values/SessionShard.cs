namespace Virtuademy.SDK.Interface
{
    /// <summary>
    /// One shard of a session: a capacity-bounded slice of participants who share a room.
    /// </summary>
    /// <remarks>
    /// <b>Why this one is still a contract type when the session, the world and the user are the
    /// platform's own DTOs.</b> Its wire form, <c>ShardDTO</c>, lives in the realtime package
    /// beside the WebSocket client — so naming it here would pull the transport into the contracts
    /// assembly, which is the one thing the perimeter forbids. The other four exceptions are of the
    /// same kind or simpler: a permission arrives as a bare <em>string</em> and has no DTO at all,
    /// and the state, the launch data and the login challenge are concepts of this contract with no
    /// wire counterpart to name.
    /// <para>
    /// No per-shard maximum: the wire carries the current occupancy and the closed flag, and the
    /// ceiling is a single global for every shard in the deployment. A per-shard
    /// <c>MaxParticipants</c> was in an earlier draft, copied from a client model that reads that
    /// global through a static field — so it would have looked like shard data while being the same
    /// number everywhere. The session's own <c>Capacity</c> is the number a caller actually wants.
    /// </para>
    /// </remarks>
    public sealed class SessionShard
    {
        public SessionShard(int number, int sessionId, int currentParticipants, bool isClosed)
        {
            Number = number;
            SessionId = sessionId;
            CurrentParticipants = currentParticipants;
            IsClosed = isClosed;
        }

        public int Number { get; }

        /// <summary>
        /// The session this shard belongs to. Supplied by the implementation: the wire does not
        /// repeat it on every shard, because a shard list is always the answer about one session.
        /// </summary>
        public int SessionId { get; }

        public int CurrentParticipants { get; }

        /// <summary>Whether the shard refuses further participants regardless of capacity.</summary>
        public bool IsClosed { get; }
    }
}
