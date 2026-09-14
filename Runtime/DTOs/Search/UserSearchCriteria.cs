using System;
using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class UserSearchCriteria
    {
        [SerializeField] public string text;
        [SerializeField] public string tag;
    }
}
