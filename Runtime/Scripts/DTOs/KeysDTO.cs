using System;

using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class KeysDTO
    {
        [SerializeField] private int id;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private int[] keysId;
        [SerializeField] private int userId;
        [SerializeField] private DateTime expiration;
        [SerializeField] private string status;
        [SerializeField] private bool trialAvailable;

        public int Id => id;
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public int[] KeysId => keysId;
        public int UserId => userId;
        public DateTime Expiration => expiration;
        public string Status => status;
        public bool TrialAvailable => trialAvailable;
    }
}

