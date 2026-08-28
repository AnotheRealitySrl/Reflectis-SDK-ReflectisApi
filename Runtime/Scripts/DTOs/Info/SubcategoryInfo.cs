using System;
using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class SubcategoryInfo
    {
        [SerializeField] private int id;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string label;
        [SerializeField] private string description;
        [SerializeField] private EStatusOption status;
        [SerializeField] private bool visibleByAnonymous;
        [SerializeField] private EVisibilityOption visibilityOption;
        [SerializeField] private int categoryId;
        [SerializeField] private int ownerUserId;
        [SerializeField] private SessionDTO[] events;

        public int Id { get => id; set => id = value; }
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }
        public DateTime LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        public string Label { get => label; set => label = value; }
        public string Description { get => description; set => description = value; }
        public EStatusOption Status { get => status; set => status = value; }
        public bool VisibleByAnonymous { get => visibleByAnonymous; set => visibleByAnonymous = value; }
        public EVisibilityOption VisibilityOption { get => visibilityOption; set => visibilityOption = value; }
        public int CategoryId { get => categoryId; set => categoryId = value; }
        public int OwnerUserId { get => ownerUserId; set => ownerUserId = value; }
        public SessionDTO[] Events { get => events; set => events = value; }
    }
}
