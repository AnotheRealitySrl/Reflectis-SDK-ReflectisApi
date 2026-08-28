using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    public class TelemetryDTO
    {
        public class MetricsData
        {
            public string platform;
            public int? averageFps;
            public int? worldId;
            public string sessionLabel;
            public int? shard;
            public string environmentLabel;
            public int? photonPing;
            public long? bytesUploaded = null;
            public long? bytesDownloaded = null;
        }

        [SerializeField] private int? sessionId;
        [SerializeField] private MetricsData metrics;

        public int? SessionId { get => sessionId; set => sessionId = value; }
        public MetricsData Metrics { get => metrics; set => metrics = value; }
    }
}
