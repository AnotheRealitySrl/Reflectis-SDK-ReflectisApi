using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
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
        [SerializeField] private int[] userIds;
        [SerializeField] private object template;
        [SerializeField] private bool unlimited;
        [SerializeField] private bool lobby;
        [SerializeField] private bool isLiveNow;
        [SerializeField] private ESessionAccessibility accessibility;
        [SerializeField] private ESessionStatus status;
        [SerializeField] private object config;

        public int Id => id;
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public string Label => label;
        public int OwnerUserId => ownerUserId;
        public int ExperienceId => experienceId;

        /// <summary>
        /// This DateTime is in UTC time
        /// </summary>
        public DateTime StartDate => startDate;
        /// <summary>
        /// This DateTime is in UTC time
        /// </summary>
        public DateTime? EndDate => endDate;
        public int Capacity => capacity;
        public bool Multiplayer => multiplayer;
        public int[] UserIds => userIds;
        public object Template => template;
        public bool Unlimited => unlimited;
        public TagDTO[] Tags => tags;
        public int WorldId => worldId;
        public bool Lobby => lobby;
        public bool IsLiveNow => isLiveNow;
        public ESessionAccessibility Accessibility => accessibility;
        public ESessionStatus Status => status;
        public object Config => config;
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
        Empty,
        Persistent,
    }
}