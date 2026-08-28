using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CatalogDTO
    {
        [SerializeField] private string baseUrl;
        [SerializeField] private string catalogName;
        [SerializeField] private string catalogFullName;

        public string BaseUrl => baseUrl;
        public string CatalogName => catalogName;
        public string CatalogFullName => catalogFullName;
    }
}
