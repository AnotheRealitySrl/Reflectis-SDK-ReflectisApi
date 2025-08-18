using System;
using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class XAPIStatement
    {
        [SerializeField]
        private XAPIVerb xApiVerb;
        [SerializeField]
        private XAPIObject xApiObject;
    }
}
