using System;
using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class TagDTO
    {
        [SerializeField] private int id;
        [SerializeField] private int worldId;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string label;
        [SerializeField] private string note;
        [SerializeField] private ETagStatusOption status;
        [SerializeField] private string color;
        [SerializeField] private ETopicOption topic;
        [SerializeField] private bool system;
        [SerializeField] private bool defaultTag;
        [SerializeField] private bool visible;

        public int Id { get => id; set => id = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }
        public DateTime LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        public string Label { get => label; set => label = value; }
        public string Note { get => note; set => note = value; }
        public ETagStatusOption Status { get => status; set => status = value; }
        public string Color { get => color; set => color = value; }
        public ETopicOption Topic { get => topic; set => topic = value; }
        public bool System { get => system; set => system = value; }
        public bool DefaultTag { get => defaultTag; set => defaultTag = value; }
        public bool Visible { get => visible; set => visible = value; }
    }
}
