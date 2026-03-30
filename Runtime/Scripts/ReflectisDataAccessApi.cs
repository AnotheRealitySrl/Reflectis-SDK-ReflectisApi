using Reflectis.SDK.Core.ApiSystem;
using Reflectis.SDK.Core.Utilities;
using Reflectis.SDK.Http;

using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine.Networking;

namespace Reflectis.SDK.ReflectisApi
{
    /// <summary>
    /// Static API class for Reflectis data access operations.
    /// Contains the same methods as ReflectisDataAccessSystem.
    /// New code should use this class directly instead of SM.GetSystem.
    ///
    /// NOTE: This is a shell with representative methods.
    /// Additional methods should follow the same pattern.
    /// </summary>
    public class ReflectisDataAccessApi : ApiBase<ReflectisDataAccessData>
    {
        private const string app = "Unity";

        public static int CacheId
        {
            get => Data.cacheId;
            set => Data.cacheId = value;
        }

        public static string ApiVersion => Data.ApiConfig.ApiVersion;

        #region Worlds

        public static async Task<ApiResponseArray<WorldDTO>> GetWorlds()
        {
            using UnityWebRequest request = ApiHelper.BuildRequest(
                UnityWebRequest.kHttpVerbGET, "worlds", Data.ApiConfig,
                jwtToken: Data.JwtToken, serverTimeOffset: Data.ServerTimeOffset);
            await request.SendWebRequest();
            return new ApiResponseArray<WorldDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public static async Task<ApiResponse<WorldDTO>> GetWorld(int worldId)
        {
            using UnityWebRequest request = ApiHelper.BuildRequest(
                UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}", Data.ApiConfig,
                jwtToken: Data.JwtToken, serverTimeOffset: Data.ServerTimeOffset);
            await request.SendWebRequest();
            return new ApiResponse<WorldDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Assets

        public static async Task<ApiResponse<AssetDTO>> GetAssetDetails(int worldId, int assetId)
        {
            Dictionary<string, string> queryParams = new()
            {
                { "buildSasContentUrl", "true" },
                { "buildSasThumbnailUrl", "true" }
            };

            using UnityWebRequest request = ApiHelper.BuildRequest(
                UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/{assetId}", Data.ApiConfig,
                queryParams: queryParams,
                jwtToken: Data.JwtToken, serverTimeOffset: Data.ServerTimeOffset);
            await request.SendWebRequest();
            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Environments

        public static async Task<ApiResponseArray<EnvironmentDTO>> GetEnvironments(int worldId)
        {
            using UnityWebRequest request = ApiHelper.BuildRequest(
                UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/environments", Data.ApiConfig,
                jwtToken: Data.JwtToken, serverTimeOffset: Data.ServerTimeOffset);
            await request.SendWebRequest();
            return new ApiResponseArray<EnvironmentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        // TODO: Add remaining methods from ReflectisDataAccessSystem following the same pattern.
        // Each method should:
        // 1. Use ApiHelper.BuildRequest() with Data.ApiConfig, Data.JwtToken, Data.ServerTimeOffset
        // 2. Await request.SendWebRequest()
        // 3. Return the appropriate ApiResponse type
    }
}
