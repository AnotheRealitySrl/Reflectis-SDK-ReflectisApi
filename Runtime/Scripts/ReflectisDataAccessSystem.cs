using Virtuademy.ScriptingApi;
using Newtonsoft.Json;

using Virtuademy.SDK.Core.ApiSystem;
using Virtuademy.SDK.Core.SystemFramework;
using Virtuademy.SDK.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

using Virtuademy.SDK.Core.Authentication;


using SPACS.Utilities;

namespace Virtuademy.SDK.ApiData
{
    [CreateAssetMenu(menuName = "Virtuademy/Systems/ReflectisDataAccessSystem", fileName = "ReflectisDataAccessSystemConfig")]
    public class ReflectisDataAccessSystem : ApiSystemBase
    {
        #region Reaching this client

        private static ReflectisDataAccessSystem installed;

        /// <summary>Whether an application has installed a client.</summary>
        public static bool IsInstalled => installed != null;

        /// <summary>
        /// The platform client. Every caller should reach it through here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This exists so callers stop naming <c>SM</c>. The class is still a
        /// <c>ScriptableObject</c> system today, so with nothing installed this falls back to
        /// resolving one through the framework — which is what makes the migration safe to do a
        /// few call sites at a time: every intermediate state compiles and behaves identically.
        /// </para>
        /// <para>
        /// The fallback is <b>not cached</b>. The framework re-creates its systems, and a static
        /// field holding yesterday's instance is the kind of bug that survives a scene load and
        /// surfaces somewhere unrelated.
        /// </para>
        /// <para>
        /// When the last caller has moved, this type stops deriving from the system base — two
        /// lines, measured — the fallback goes, and <see cref="Install"/> becomes the only way in.
        /// That is the step that lets this package stop referencing <c>Virtuademy.SDK.Core</c>,
        /// which is what an external app needs and cannot have today.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// No client is installed and none is registered with the framework either.
        /// </exception>
        public static ReflectisDataAccessSystem Current
            => installed
               ?? SM.GetSystem<ReflectisDataAccessSystem>()
               ?? throw new InvalidOperationException(
                   "No platform client is available. An application installs one at startup; " +
                   "while this type is still a system, one registered with the framework is used " +
                   "instead. Check ReflectisDataAccessSystem.IsInstalled if this code can run " +
                   "outside a running application.");

        /// <summary>Registers the client. Called once, by the application.</summary>
        public static void Install(ReflectisDataAccessSystem client)
        {
            installed = client ? client : throw new ArgumentNullException(nameof(client));
        }

        #endregion

        /// <summary>
        /// Opts this system into endpoint discovery: its base URL is resolved from
        /// the platform record for the platform REST API rather than from the value serialized into
        /// the build, falling back to that value when discovery has not answered.
        /// See ADR 0024 in the meta-repo.
        /// </summary>
        protected override string DiscoveryApiType => "Application";

        #region Inspector info
        [Header("Reflectis Data Access API Info")]
        // ReflectisDataAccessSystem has no additional serialized fields beyond ApiSystemBase.
        // CacheId is runtime-only state.
        [System.NonSerialized] public int cacheId = -1;
        #endregion
        
        #region Private stuff
        private const string app = "Unity";
        #endregion

        #region Properties
        public string ApiVersion => apiConfig.ApiVersion;
        public int CacheId { get { return cacheId; } set { cacheId = value; } }
        #endregion

        #region Overrides

        public override async Task Init()
        {
            await base.Init();

            //apiConfig = new AppIdentification(apiConfig.Credential,
            //    "https://localhost:12026", apiConfig.ApiVersion);
        }

        #endregion

        #region Experience
        public async Task<ApiResponse<ExperienceDTO>> GetExperience(int worldId, int experienceId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences/{experienceId}");
            await request.SendWebRequest();
            return new ApiResponse<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Sessions
        public async Task<ApiResponse<SessionDTO>> CreateSession(int worldId, int experienceId, NewSessionDTO newEvent)
        {
            Debug.Log("Creating event " + JsonConvert.SerializeObject(newEvent));
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/experiences/{experienceId}/newsession", body: JsonConvert.SerializeObject(newEvent));
            await request.SendWebRequest();

            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<SessionDTO>> GetSession(int worldId, int sessionId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/{sessionId}");
            await request.SendWebRequest();

            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Permissions

        public async Task<ApiResponse<List<string>>> GetMySessionPermissions(int eventId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"sessions/{eventId}/app/{app}/permissions/my");
            await request.SendWebRequest();

            return new ApiResponse<List<string>>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<List<string>>> GetMyWorldPermissions(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/app/{app}/permissions/my");
            await request.SendWebRequest();

            return new ApiResponse<List<string>>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Worlds

        public async Task<ApiResponse<WorldDTO>> GetWorld(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}");
            await request.SendWebRequest();

            return new ApiResponse<WorldDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        /// <summary>
        /// Every world the calling app is published in, one entry per published <c>ExternalApp</c>
        /// experience. How a standalone external app finds out where it may run.
        /// </summary>
        /// <remarks>
        /// <b>No app parameter, by contract.</b> The app is identified by its token's <c>azp</c>
        /// claim, matched server-side against the <c>appObjectId</c> on each experience's config.
        /// So an app cannot ask about another app, and there is no app identity duplicated between
        /// the caller and the record.
        /// <para>
        /// Three answers matter and they are different states, not degrees of the same one:
        /// <b>200</b> with entries, <b>204</b> meaning the app is registered but published in no
        /// world this user can enter — terminal and explainable, not something to retry — and
        /// <b>403</b> meaning the token carries no <c>azp</c> at all, i.e. there is no app asking.
        /// </para>
        /// Contract: <c>contracts/openapi/external-app-worlds.yaml</c> in the meta-repo.
        /// </remarks>
        public async Task<ApiResponseArray<ExternalAppPlacementDTO>> GetExternalAppWorlds()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, "external-app/worlds");
            await request.SendWebRequest();

            return new ApiResponseArray<ExternalAppPlacementDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Users

        public async Task<ApiResponse<UserDTO>> GetUserData(int worldId, int userId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/users/{userId}");
            await request.SendWebRequest();

            return new ApiResponse<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse> UpdateMyPreferences(object newPreferences)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"users/my/preferences", body: JsonConvert.SerializeObject(newPreferences));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse> UpdateMyPreferences(int worldId, object newPreferences)
        {
            //TO DO: REPLACE WITH WORLD API ONCE READY
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"users/my/preferences", body: JsonConvert.SerializeObject(newPreferences));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<UserDTO>> GetMyUserData()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"users/my/profile");
            await request.SendWebRequest();

            return new ApiResponse<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Experience Analytics
        public async Task<ApiResponse> CreateExperienceAnalytic(AnalyticDTO experienceAnalyticDTO)
        {
            if (experienceAnalyticDTO.Statement == null)
            {
                //make sure the object is null so that it won't be sent to the server
                experienceAnalyticDTO.Locale = null;
                using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"online/experience/progress",
                    body: JsonConvert.SerializeObject(experienceAnalyticDTO));
                await request.SendWebRequest();
                return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
            }
            else
            {
                using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"xapi/lrs",
                    body: JsonConvert.SerializeObject(experienceAnalyticDTO));
                await request.SendWebRequest();
                return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
            }
        }
        #endregion

        #region Save Data
        public async Task<ApiResponse<CustomType>> LoadSaveData(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/users/my/data/all");
            await request.SendWebRequest();
            return new ApiResponse<CustomType>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<CustomType>> SetMySaveData(int id, string key, object data)
        {
            CustomType customType = new CustomType();
            customType.Fields[key] = data;

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{id}/users/my/data",
                body: JsonConvert.SerializeObject(customType));

            await request.SendWebRequest();
            return new ApiResponse<CustomType>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<CustomType>> DeleteMySaveData(int id, List<string> keys)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbDELETE, $"worlds/{id}/users/my/data", body: JsonConvert.SerializeObject(keys));
            await request.SendWebRequest();
            return new ApiResponse<CustomType>(request.responseCode, request.error, request.downloadHandler.text);
        }
        #endregion

        #region Leaderboard
        public async Task<ApiResponse> CreateLeaderboardRecord(int worldId, string leaderboardKey, float data)
        {
            LeaderboardRecordDTO record = new LeaderboardRecordDTO(leaderboardKey, data);
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/leaderboards/records", body: JsonConvert.SerializeObject(record));
            await request.SendWebRequest();
            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }
        #endregion

        #region Overrides

        protected override Dictionary<string, string> SetDefaultHeaders(params string[] values)
        {
            Dictionary<string, string> headers = base.SetDefaultHeaders(values);

            if (CacheId > 0)
            {
                headers.Add("Cache-Id", CacheId.ToString());
            }

            return headers;
        }

        #endregion

    }
}