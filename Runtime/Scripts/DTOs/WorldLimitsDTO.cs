using System;
using UnityEngine;
namespace Virtuademy.SDK.PlatformApi
{
    [Serializable]
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.Fields)]
    public class WorldLimitsDTO
    {
        [SerializeField] private int assetStorageQuotaKb;
        [SerializeField] private int assetOwnerStorageQuotaKb;
        [SerializeField] private int assetEgressQuotaKb;
        [SerializeField] private int singleEnvQuotaKb;
        [SerializeField] private int envStorageQuotaKb;
        [SerializeField] private int envStorageMaxCount;
        [SerializeField] private int? maxConcurrentUsers;
        [SerializeField] private int maxRegisteredUsers;
        [SerializeField] private int usedAssetStorageKb;
        [SerializeField] private int usedAssetOwnerStorageKb;
        [SerializeField] private int assetEgressKb;
        [SerializeField] private int assetOwnerEgressKb;
        [SerializeField] private int usedEnvStorageKb;
        [SerializeField] private int envCount;
        [SerializeField] private int concurrentUsers;
        [SerializeField] private int registeredUsers;

        public int AssetStorageQuotaKb => assetStorageQuotaKb;
        public int AssetOwnerStorageQuotaKb => assetOwnerStorageQuotaKb;
        public int AssetEgressQuotaKb => assetEgressQuotaKb;
        public int SingleEnvQuotaKb => singleEnvQuotaKb;
        public int EnvStorageQuotaKb => envStorageQuotaKb;
        public int EnvStorageMaxCount => envStorageMaxCount;
        public int? MaxConcurrentUsers => maxConcurrentUsers;
        public int MaxRegisteredUsers => maxRegisteredUsers;
        public int UsedAssetStorageKb => usedAssetStorageKb;
        public int UsedAssetOwnerStorageKb => usedAssetOwnerStorageKb;
        public int AssetEgressKb => assetEgressKb;
        public int AssetOwnerEgressKb => assetOwnerEgressKb;
        public int UsedEnvStorageKb => usedEnvStorageKb;
        public int EnvCount => envCount;
        public int ConcurrentUsers => concurrentUsers;
        public int RegisteredUsers => registeredUsers;
    }
}