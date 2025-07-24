using System;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Analytics
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
