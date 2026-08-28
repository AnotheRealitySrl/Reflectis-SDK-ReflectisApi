using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class ExperienceDTO
    {
        [SerializeField] private int id;
        [SerializeField] private string label;
        [SerializeField] private string description;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private EExperienceStatusOption status;
        [SerializeField] private int ownerUserId;
        [SerializeField] private bool spotlight;
        [SerializeField] private int environmentId;
        [SerializeField] private TagDTO[] tags;
        [SerializeField] private object config;
        [SerializeField] private bool canVisualize;
        [SerializeField] private bool canWrite;
        [SerializeField] private EExperienceTypeOption type;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private int lobbyId;

        public int Id { get => id; set => id = value; }
        public string Label { get => label; set => label = value; }
        public string Description { get => description; set => description = value; }
        public EExperienceStatusOption Status { get => status; set => status = value; }
        public int OwnerUserId { get => ownerUserId; set => ownerUserId = value; }
        public bool Spotlight { get => spotlight; set => spotlight = value; }
        public int EnvironmentId { get => environmentId; set => environmentId = value; }
        public object Config { get => config; set => config = value; }
        public bool CanVisualize { get => canVisualize; set => canVisualize = value; }
        public bool CanWrite { get => canWrite; set => canWrite = value; }
        public TagDTO[] Tags { get => tags; set => tags = value; }
        public string ThumbnailUri { get => thumbnailUri; set => thumbnailUri = value; }
        public EExperienceTypeOption Type { get => type; set => type = value; }
        public DateTime CreationDate { get => creationDate; set => creationDate = value; }
        public DateTime LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        public int LobbyId { get => lobbyId; set => lobbyId = value; }

        public enum EExperienceTypeOption
        {
            Core,
            Authored,
            ExternalApp
        }
        public enum EExperienceStatusOption
        {
            Draft,
            Published,
        }
    }
}