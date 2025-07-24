using Reflectis.SDK.Core.ApplicationManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.DataAccess
{

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class RealtimeOnlineUsersMessage
    {
        [SerializeField]
        private ERealtimeOnlineUsersMessageType type;
        [SerializeField]
        private ERealtimeOnlineUsersMessageKey key;
        [SerializeField]
        private object value;

        public ERealtimeOnlineUsersMessageType Type { get => type; set => type = value; }
        public ERealtimeOnlineUsersMessageKey Key { get => key; set => key = value; }
        public object Value { get => value; set => this.value = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class RealtimeResponseDTO
    {
        public enum EResponseStatusCode
        {
            Success = 200,
            JoinFailure = 409,
            EmbodyFailure = 410,
        }

        [SerializeField]
        private EResponseStatusCode statusCode;
        [SerializeField]
        private object content;

        public object Content { get => content; set => content = value; }
        public EResponseStatusCode StatusCode { get => statusCode; set => statusCode = value; }
        public bool IsSuccess => ((int)statusCode >= 200) && ((int)statusCode <= 299);
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class Handshake
    {
        [SerializeField]
        private string connectionId;

        public string ConnectionId { get => connectionId; set => connectionId = value; }
    }

    [Serializable]
    public class EmbodyConnectionDTO
    {
        [SerializeField]
        private EApplicationState applicationState;

        public EApplicationState ApplicationState { get => applicationState; set => applicationState = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class WorldOnlineUsersCountDTO
    {
        [SerializeField] private int worldId;
        [SerializeField] private int multiplayerUsersCount;

        public int WorldId { get => worldId; set => worldId = value; }
        public int MultiplayerUsersCount { get => multiplayerUsersCount; set => multiplayerUsersCount = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class PostJoinWorldDTO
    {
        [SerializeField]
        private int worldId;
        [SerializeField]
        private int sessionId;

        public int WorldId { get => worldId; set => worldId = value; }
        public int SessionId { get => sessionId; set => sessionId = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class PostJoinSessionDTO
    {
        [SerializeField]
        private int sessionIn;
        [SerializeField]
        private int? shardIn;

        public int SessionIn { get => sessionIn; set => sessionIn = value; }
        public int? ShardIn { get => shardIn; set => shardIn = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class BroadcastWorldCCUDTO
    {
        [SerializeField]
        private List<UserDTO> onlineUsers;

        public List<UserDTO> OnlineUsers { get => onlineUsers; set => onlineUsers = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class ShardDTO
    {
        [SerializeField]
        private int shardNumber;
        [SerializeField]
        private int currentCapacity;
        [SerializeField]
        private bool isClosed;

        public int ShardNumber { get => shardNumber; set => shardNumber = value; }
        public bool IsClosed { get => isClosed; set => isClosed = value; }
        public int CurrentCapacity { get => currentCapacity; set => currentCapacity = value; }
    }

    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class ErrorMessageDTO
    {
        [SerializeField]
        private int errorCode;
        [SerializeField]
        private string message;

        public int ErrorCode { get => errorCode; set => errorCode = value; }
        public string Message { get => message; set => message = value; }
    }

    public enum ECloseConnectionReason
    {
        //Triggered when a user starts a new connection even though another connection is active
        MultipleUsersLoggedIn,
        //Triggered when a user logouts
        UserLogout,
        //Triggered when the server disconnects the user
        ServerDisconnection
    }

}