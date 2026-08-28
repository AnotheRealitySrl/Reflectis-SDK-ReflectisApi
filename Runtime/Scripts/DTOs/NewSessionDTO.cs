using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class NewSessionDTO
    {
        [SerializeField] private string label;
        [SerializeField] private ESessionAccessibility accessibility;
        [SerializeField] private int ownerUserId;
        [SerializeField] private DateTime? startDate;
        [SerializeField] private DateTime? endDate;
        [SerializeField] private bool multiplayer;
        [SerializeField] private int[] tagIds;
        [SerializeField] private int[] userIds;
        [SerializeField] private ESessionStatus status;

        public string Label { get => label; set => label = value; }
        public int OwnerUserId { get => ownerUserId; set => ownerUserId = value; }
        /// <summary>
        /// This DateTime is in UTC time
        /// </summary>
        public DateTime? StartDate { get => startDate; set => startDate = value; }
        /// <summary>
        /// This DateTime is in UTC time
        /// </summary>
        public DateTime? EndDate { get => endDate; set => endDate = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public int[] TagIds { get => tagIds; set => tagIds = value; }
        public ESessionAccessibility Accessibility { get => accessibility; set => accessibility = value; }
        public int[] UserIds { get => userIds; set => userIds = value; }
        public ESessionStatus Status { get => status; set => status = value; }
    }

}