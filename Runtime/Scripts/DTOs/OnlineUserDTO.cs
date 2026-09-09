using Newtonsoft.Json;
using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable, JsonObject(MemberSerialization.Fields)]
    public class OnlineUserDTO
    {
        [SerializeField] private int userId;
        [SerializeField] private string platform;
        [SerializeField] private int shardNumber;
        [SerializeField] private int sessionId;
        [SerializeField] private int worldId;
        [SerializeField] private string connectionId;
        [SerializeField] private object preferences;
        [SerializeField] private string email;
        [SerializeField] private int? code;
        [SerializeField] private TagDTO[] tags;

        public int UserId => userId;
        public string Platform => platform;
        public int ShardNumber => shardNumber;
        public int SessionId => sessionId;
        public int WorldId => worldId;
        public string ConnectionId => connectionId;
        public object Preferences => preferences;
        public string Email => email;
        public int? Code => code;
        public TagDTO[] Tags => tags;
    }
}
