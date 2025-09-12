using System;
using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class WorldInfo
    {
        [SerializeField] private WorldDTO world;
        [SerializeField] private CategoryInfo[] categories;
    }
}
