using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class WorldConfigDTO
    {

        [SerializeField] private string videoChatAppId;
        [SerializeField] private string gptAppId;
        [SerializeField] private int maxShardCapacity = 20;
        [SerializeField] private EWorldMode worldMode = EWorldMode.Catalog;

        public string VideoChatAppId => videoChatAppId;
        public string GptAppId => gptAppId;
        public int MaxShardCapacity => maxShardCapacity;
        public EWorldMode WorldMode { get => worldMode; set => worldMode = value; }

        public enum EWorldMode
        {
            Catalog,
            Play
        }

    }
}

