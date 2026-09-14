using System;
using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class CategoryDTO
    {
        [SerializeField] private int id;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string label;
        [SerializeField] private string description;
        [SerializeField] private EStatusOption status;
        [SerializeField] private bool visibleByAnonymous;
        [SerializeField] private EVisibilityOption visibilityOption;
        [SerializeField] private int worldId;
        [SerializeField] private int ownerUserId;

        public int Id => id;
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public string Label => label;
        public string Description => description;
        public EStatusOption Status => status;
        public bool VisibleByAnonymous => visibleByAnonymous;
        public EVisibilityOption VisibilityOption => visibilityOption;
        public int WorldId => worldId;
        public int OwnerUserId => ownerUserId;
    }
}