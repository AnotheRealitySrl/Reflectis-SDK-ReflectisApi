using UnityEngine;

#nullable enable
namespace Virtuademy.SDK.ApiData
{
    public class EnvironmentSearchCriteria
    {
        [SerializeField] private string? platform;
        [SerializeField] private string? version;
        [SerializeField] private string? label;
        [SerializeField] private ESearchMode platformSearchMode;
        [SerializeField] private ESearchMode versionSearchMode;
        [SerializeField] private ESearchMode labelSearchMode;
        [SerializeField] private int? userId;

        public string? Platform { get => platform; set => platform = value; }
        public string? Version { get => version; set => version = value; }
        public string? Label { get => label; set => label = value; }
        public ESearchMode PlatformSearchMode { get => platformSearchMode; set => platformSearchMode = value; }
        public ESearchMode VersionSearchMode { get => versionSearchMode; set => versionSearchMode = value; }
        public ESearchMode LabelSearchMode { get => labelSearchMode; set => labelSearchMode = value; }
        public int? UserId { get => userId; set => userId = value; }
    }
}
