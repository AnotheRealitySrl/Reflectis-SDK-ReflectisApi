using System;

using UnityEngine;

namespace Reflectis.SDK.DataAccess
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

        public int Id { get => id; set => id = value; }
        public TagDTO[] Tags { get => tags; set => tags = value; }
        public string Email { get => email; set => email = value; }
        public object Preferences { get => preferences; set => preferences = value; }
        public int? Code { get => code; set => code = value; }
    }

}
