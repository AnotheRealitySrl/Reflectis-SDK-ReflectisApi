using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    public class ErrorDiagnosticDTO
    {
        [Header("BaseInfo")]
        [SerializeField] private int? userId;
        [SerializeField] private DateTime? userTimestamp;
        [SerializeField] private int? averageFps;
        [SerializeField] private int? latestAverageFps;
        [SerializeField] private float? latestFrame;
        [SerializeField] private string metadata;
        [SerializeField] private string platform;
        [Header("ErrorInfo")]
        [SerializeField] private string errorType;
        [SerializeField] private string errorMessage;
        [SerializeField] private string errorStackTrace;
        [Header("EventData")]
        [SerializeField] private int? worldId;
        [SerializeField] private int? eventId;
        [SerializeField] private string eventLabel;
        [SerializeField] private int? environmentId;
        [SerializeField] private string environmentLabel;
        [SerializeField] private int? shard;
        [SerializeField] private int? eventPlayersCount;
        [SerializeField] private int? placeholdersCount;
        [Header("PhotonData")]
        [SerializeField] private int? photonPlayersCount;
        [SerializeField] private int? networkObjectsCount;
        [SerializeField] private int? photonPing;

        public int? UserId { get => userId; set => userId = value; }
        public DateTime? UserTimestamp { get => userTimestamp; set => userTimestamp = value; }
        public int? AverageFps { get => averageFps; set => averageFps = value; }
        public int? LatestAverageFps { get => latestAverageFps; set => latestAverageFps = value; }
        public float? LatestFrame { get => latestFrame; set => latestFrame = value; }
        public string Metadata { get => metadata; set => metadata = value; }
        public string ErrorType { get => errorType; set => errorType = value; }
        public string ErrorMessage { get => errorMessage; set => errorMessage = value; }
        public string ErrorStackTrace { get => errorStackTrace; set => errorStackTrace = value; }
        public int? WorldId { get => worldId; set => worldId = value; }
        public int? EventId { get => eventId; set => eventId = value; }
        public int? EnvironmentId { get => environmentId; set => environmentId = value; }
        public int? Shard { get => shard; set => shard = value; }
        public int? PlaceholdersCount { get => placeholdersCount; set => placeholdersCount = value; }
        public int? PhotonPlayersCount { get => photonPlayersCount; set => photonPlayersCount = value; }
        public int? NetworkObjectsCount { get => networkObjectsCount; set => networkObjectsCount = value; }
        public int? PhotonPing { get => photonPing; set => photonPing = value; }
        public string EnvironmentLabel { get => environmentLabel; set => environmentLabel = value; }
        public string EventLabel { get => eventLabel; set => eventLabel = value; }
        public int? EventPlayersCount { get => eventPlayersCount; set => eventPlayersCount = value; }
        public string Platform { get => platform; set => platform = value; }
    }
}