using Reflectis.SDK.Core.ApiSystem;

using UnityEngine;

namespace Reflectis.SDK.ReflectisApi
{
    [CreateAssetMenu(menuName = "Reflectis/ApiData/ReflectisDataAccessData", fileName = "ReflectisDataAccessData")]
    public class ReflectisDataAccessData : ApiDataBase
    {
        // ReflectisDataAccessSystem has no additional serialized fields beyond ApiSystemBase.
        // CacheId is runtime-only state.
        [System.NonSerialized] public int cacheId = -1;
    }
}
