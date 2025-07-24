using System;

using UnityEngine;

namespace Reflectis.SDK.DataAccess
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class WorldConfigDTO
    {

        [SerializeField] private string videoChatAppId;
        [SerializeField] private string gptAppId;
        [SerializeField] private bool allowDoubleUserPresence;
        [SerializeField] private bool showFullNickname;
        [SerializeField] private int maxShardCapacity = 20;

        public string VideoChatAppId => videoChatAppId;
        public string GptAppId => gptAppId;
        public bool AllowDoubleUserPresence => allowDoubleUserPresence;
        public bool ShowFullNickname => showFullNickname;
        public int MaxShardCapacity => maxShardCapacity;

    }
}

