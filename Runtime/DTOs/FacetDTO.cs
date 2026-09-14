using System;
using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class FacetDTO
    {
        [SerializeField] private int id;
        [SerializeField] private DateTime creationDate;
        [SerializeField] private DateTime lastUpdate;
        [SerializeField] private string identifier;
        [SerializeField] private string label;
        [SerializeField] private string note;
        [SerializeField] private string facetGroup;
        [SerializeField] private string status;
        [SerializeField] private string[] requiredRoles;

        public int Id => id;
        public DateTime CreationDate => creationDate;
        public DateTime LastUpdate => lastUpdate;
        public string Identifier => identifier;
        public string Label => label;
        public string Note => note;
        public string FacetGroup => facetGroup;
        public string Status => status;
        public string[] RequiredRoles => requiredRoles;
    }
}
