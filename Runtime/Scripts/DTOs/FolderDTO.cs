using System;

using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class FolderDTO
    {
        [SerializeField] private string name;
        [SerializeField] private int fileCount;
        [SerializeField] private int totalKBytes;

        public string Name => name;
        public int FileCount => fileCount;
        public int TotalKBytes => totalKBytes;
    }
}
