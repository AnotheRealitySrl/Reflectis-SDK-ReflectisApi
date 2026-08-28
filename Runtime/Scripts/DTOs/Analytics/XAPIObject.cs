using System;
using UnityEngine;

namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class XAPIObject
    {
        [SerializeField] private string id;
        [SerializeField] private string label;
    }
}
