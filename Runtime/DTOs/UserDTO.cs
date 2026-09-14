using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class UserDTO
    {
        [SerializeField] private int id;
        [SerializeField] private string email;
        [SerializeField] private int? code;
        [SerializeField] private object preferences;
        [SerializeField] private TagDTO[] tags;

        public int Id => id;
        public TagDTO[] Tags => tags;
        public string Email => email;
        public object Preferences => preferences;
        public int? Code => code;
    }

}
