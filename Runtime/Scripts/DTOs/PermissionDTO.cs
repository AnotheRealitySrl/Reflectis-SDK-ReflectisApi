using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class PermissionDTO
    {
        public enum EPermissionContext
        {
            Event,
            World
        }

        [SerializeField] private int tagId;
        [SerializeField] private int? worldId;
        [SerializeField] private int facetId;
        [SerializeField] private string facetLabel;
        [SerializeField] private string facetIdentifier;
        [SerializeField] private string facetGroup;
        [SerializeField] private EPermissionContext? context;
        [SerializeField] private int eventId;
        [SerializeField] private bool? deny;

        public int TagId { get => tagId; set => tagId = value; }
        public int? WorldId => worldId;
        public int FacetId { get => facetId; set => facetId = value; }
        public string FacetLabel => facetLabel;
        public string FacetIdentifier => facetIdentifier;
        public string FacetGroup => facetGroup;
        public EPermissionContext? Context => context;
        public int EventId { get => eventId; set => eventId = value; }
        public bool? Deny { get => deny; set => deny = value; }
    }
}