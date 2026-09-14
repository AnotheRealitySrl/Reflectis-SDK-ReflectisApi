using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    public class AssetSearchCriteria
    {
        [SerializeField] private string label;
        [SerializeField] private ESearchMode labelSearchMode;
        [SerializeField] private string folder;
        [SerializeField] private ESearchMode? folderSearchMode;
        [SerializeField] private int? userId;
        [SerializeField] private DateTime? createdAfter;
        [SerializeField] private DateTime? createdBefore;
        [SerializeField] private DateTime? updatedAfter;
        [SerializeField] private DateTime? updatedBefore;
        [SerializeField] private long? totalBytesFrom;
        [SerializeField] private long? totalBytesTo;
        [SerializeField] private bool? buildSasUrls;

        public string Label { get => label; set => label = value; }
        public ESearchMode LabelSearchMode { get => labelSearchMode; set => labelSearchMode = value; }
        public string Folder { get => folder; set => folder = value; }
        public ESearchMode? FolderSearchMode { get => folderSearchMode; set => folderSearchMode = value; }
        public int? UserId { get => userId; set => userId = value; }
        public DateTime? CreatedAfter { get => createdAfter; set => createdAfter = value; }
        public DateTime? CreatedBefore { get => createdBefore; set => createdBefore = value; }
        public DateTime? UpdatedAfter { get => updatedAfter; set => updatedAfter = value; }
        public DateTime? UpdatedBefore { get => updatedBefore; set => updatedBefore = value; }
        public long? TotalBytesFrom { get => totalBytesFrom; set => totalBytesFrom = value; }
        public long? TotalBytesTo { get => totalBytesTo; set => totalBytesTo = value; }
        public bool? BuildSasUrls { get => buildSasUrls; set => buildSasUrls = value; }
    }
}
