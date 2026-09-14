using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class NewExperienceDTO
    {
        [SerializeField] private string label;
        [SerializeField] private string description;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private bool spotlight;
        [SerializeField] private int environmentId;
        [SerializeField] private int[] tags;
        [SerializeField] private object config;
        [SerializeField] private int? parentId;
        [SerializeField] private ExperienceDTO.EExperienceStatusOption status = ExperienceDTO.EExperienceStatusOption.Draft;

        public NewExperienceDTO(string label, string description, string thumbnailUri, bool spotlight, int environmentId, int[] tags, object config, int? parentId)
        {
            this.Label = label;
            this.Description = description;
            this.ThumbnailUri = thumbnailUri;
            this.Spotlight = spotlight;
            this.EnvironmentId = environmentId;
            this.Tags = tags;
            this.Config = config;
            this.ParentId = parentId;
            this.Status = ExperienceDTO.EExperienceStatusOption.Draft;
        }

        public string Label { get => label; set => label = value; }
        public string Description { get => description; set => description = value; }
        public string ThumbnailUri { get => thumbnailUri; set => thumbnailUri = value; }
        public bool Spotlight { get => spotlight; set => spotlight = value; }
        public int EnvironmentId { get => environmentId; set => environmentId = value; }
        public int[] Tags { get => tags; set => tags = value; }
        public object Config { get => config; set => config = value; }
        public int? ParentId { get => parentId; set => parentId = value; }
        public ExperienceDTO.EExperienceStatusOption Status { get => status; set => status = value; }
    }
}