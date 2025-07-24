using System;

using UnityEngine;

namespace Reflectis.SDK.DataAccess
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class SessionDTO
    {
        [SerializeField] private int id;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private int worldId;
        [SerializeField] private string label;
        [SerializeField] private int ownerUserId;
        [SerializeField] private int experienceId;
        [SerializeField] private EnvironmentDTO environment;
        [SerializeField] private DateTime startDate;
        [SerializeField] private DateTime? endDate;
        [SerializeField] private int capacity;
        [SerializeField] private bool multiplayer;
        [SerializeField] private TagDTO[] tags;
        [SerializeField] private UserDTO[] users;
        [SerializeField] private object template;
        [SerializeField] private bool unlimited;
        [SerializeField] private bool lobby;
        [SerializeField] private bool isLiveNow;
        [SerializeField] private ESessionAccessibility accessibility;
        [SerializeField] private ESessionStatus status;

        public int Id { get => id; set => id = value; }
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public string Label { get => label; set => label = value; }
        public int OwnerUserId { get => ownerUserId; set => ownerUserId = value; }
        public int ExperienceId { get => experienceId; set => experienceId = value; }

        /// <summary>
        /// This DateTime is in UTC time
        /// </summary>
        public DateTime StartDate { get => startDate; set => startDate = value; }
        /// <summary>
        /// This DateTime is in UTC time
        /// </summary>
        public DateTime? EndDate { get => endDate; set => endDate = value; }
        public int Capacity { get => capacity; set => capacity = value; }
        public bool Multiplayer { get => multiplayer; set => multiplayer = value; }
        public UserDTO[] Users { get => users; set => users = value; }
        public object Template { get => template; set => template = value; }
        public bool Unlimited { get => unlimited; set => unlimited = value; }
        public TagDTO[] Tags { get => tags; set => tags = value; }
        public int WorldId { get => worldId; set => worldId = value; }
        public bool Lobby { get => lobby; set => lobby = value; }
        public bool IsLiveNow { get => isLiveNow; set => isLiveNow = value; }
        public ESessionAccessibility Accessibility { get => accessibility; set => accessibility = value; }
        public ESessionStatus Status { get => status; set => status = value; }
    }

    public enum ESessionAccessibility
    {
        Open,
        Closed,
    }

    public enum ESessionStatus
    {
        Scheduled,
        OnTheFly,
        Expired,
    }
}