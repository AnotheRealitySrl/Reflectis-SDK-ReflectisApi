using System;

using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class UsersInvitationDTO
    {
        [SerializeField] private string notificationMessage;
        [SerializeField] private int[] userIds;

        public string NotificationMessage { get => notificationMessage; set => notificationMessage = value; }
        public int[] UserIds { get => userIds; set => userIds = value; }
    }
}