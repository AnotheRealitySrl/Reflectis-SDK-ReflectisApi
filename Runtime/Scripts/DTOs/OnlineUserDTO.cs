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

        public int UserId { get => userId; set => userId = value; }
        public string Platform { get => platform; set => platform = value; }
        public int ShardNumber { get => shardNumber; set => shardNumber = value; }
        public int SessionId { get => sessionId; set => sessionId = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public string ConnectionId { get => connectionId; set => connectionId = value; }
        public object Preferences { get => preferences; set => preferences = value; }
        public string Email { get => email; set => email = value; }
        public int? Code { get => code; set => code = value; }
        public TagDTO[] Tags { get => tags; set => tags = value; }
    }
}
