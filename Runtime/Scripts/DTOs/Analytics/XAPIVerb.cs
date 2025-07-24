using System;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Analytics
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class XAPIVerb
    {
        [SerializeField] private string id;
        [SerializeField] private string label;
    }
}
