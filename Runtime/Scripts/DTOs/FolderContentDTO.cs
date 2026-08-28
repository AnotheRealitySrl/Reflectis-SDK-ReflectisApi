using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class FolderContentDTO
    {
        [SerializeField] private FolderDTO folder;
        [SerializeField] private FolderDTO[] children;
        [SerializeField] private AssetDTO[] assets;

        public FolderDTO Folder { get => folder; }
        public FolderDTO[] Children { get => children; }
        public AssetDTO[] Assets { get => assets; }
    }
}
