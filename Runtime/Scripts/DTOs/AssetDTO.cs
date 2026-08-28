using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class AssetDTO
    {
        [SerializeField] private int id;
        [SerializeField] private int ownerUserId;
        [SerializeField] private bool isPublic;
        [SerializeField] private string folder;
        [SerializeField] private string label;
        [SerializeField] private string extension;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string contentUri;
        [SerializeField] private EAssetDataType contentType;
        [SerializeField] private long contentBytes;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private EAssetDataType thumbnailType;
        [SerializeField] private long thumbnailBytes;
        [SerializeField] private object metadata;

        public int Id => id;
        public int OwnerUserId => ownerUserId;
        public bool IsPublic => isPublic;
        public string Folder => folder;
        public string Label => label;
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public string ContentUri => contentUri;
        public EAssetDataType ContentType => contentType;
        public long ContentBytes => contentBytes;
        public string ThumbnailUri => thumbnailUri;
        public EAssetDataType ThumbnailType => thumbnailType;
        public long ThumbnailBytes => thumbnailBytes;
        public object Metadata => metadata;

        public string Extension { get => extension; set => extension = value; }
    }
}
