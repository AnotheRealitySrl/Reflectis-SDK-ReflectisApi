using System;
using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class XAPIObject
    {
        [SerializeField] private string id;
        [SerializeField] private string label;
    }
}
