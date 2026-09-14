using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class WorldDTO
    {
        [SerializeField] private int id;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string label;
        [SerializeField] private string note;
        [SerializeField] private string thumbnailUri;
        [SerializeField] private EStatusOption status;
        [SerializeField] private bool visibleByAnonymous;
        [SerializeField] private EVisibilityOption visibilityOption;
        [SerializeField] private bool multiplayer;
        [SerializeField] private int ownerUserId;

        public int Id => id;
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public string Label => label;
        public string Note => note;
        public string ThumbnailUri => thumbnailUri;
        public EStatusOption Status => status;
        public bool VisibleByAnonymous => visibleByAnonymous;
        public EVisibilityOption VisibilityOption => visibilityOption;
        public bool Multiplayer => multiplayer;
        public int OwnerUserId => ownerUserId;
    }
}

