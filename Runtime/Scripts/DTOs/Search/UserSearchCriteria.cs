using System;
using UnityEngine;

namespace Reflectis.SDK.DataAccess
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class UserSearchCriteria
    {
        [SerializeField] public string text;
        [SerializeField] public string tag;
    }
}
