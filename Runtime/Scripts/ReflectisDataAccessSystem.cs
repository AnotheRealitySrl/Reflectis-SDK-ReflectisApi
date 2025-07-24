using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Reflectis.CreatorKit.Worlds.Analytics;
using Reflectis.SDK.Authentication;
using Reflectis.SDK.Core.SystemFramework;
using Reflectis.SDK.Core.Utilities;
using Reflectis.SDK.Core.WebSocket;
using Reflectis.SDK.DataAccess;
using Reflectis.SDK.DataAccessLayer;
using Reflectis.SDK.Http;
using Reflectis.SDK.TenantConfiguration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

using static HttpSystem;

namespace Reflectis.DataAccess
{
    [CreateAssetMenu(menuName = "AnotheReality/Systems/ReflectisDataAccessSystem", fileName = "ReflectisDataAccessSystemConfig")]
    public class ReflectisDataAccessSystem : BaseSystem
    {
        #region Inspector variables

        [Header("Tenant configuration")]
        [SerializeField] private string appId;
        [SerializeField] private string appSecret;

        [Header("Profile API settings")]
        [SerializeField] private bool allowUntrustedServers;

        #endregion

        #region Private variables

        private HttpSystem httpSystem;

        private const string app = "Unity";

        public Uri apiBaseUrl;
        private string version;

        private string realtimeBaseUrl;

        private HmacCredential credential;

        private AuthenticationSystem profileSystem;

        private TimeSpan serverTimeOffset;

        private string applicationUrl;
        private string applicationApiUrl;

        #endregion

        #region Properties

        public string ApplicationApiVersion { get; private set; }

        public string AppId { get => appId; set => appId = value; }
        public string AppSecret { get => appSecret; set => appSecret = value; }

        private string OnlineUsersRealtimeApiUrl => $"{realtimeBaseUrl}/websocketconnection/info";
        #endregion

        #region Overrides

        public override Task Init()
        {
            httpSystem = SM.GetSystem<HttpSystem>();

            object test = SM.GetSystem<TenantConfigurationSystem>().TenantConfiguration.Config;
            Debug.Log(test.GetType());

            TenantConfig tenantConfiguration
                = (SM.GetSystem<TenantConfigurationSystem>().TenantConfiguration.Config as JObject).ToObject<TenantConfig>();

            applicationUrl = tenantConfiguration.ApplicationUrl;
            applicationApiUrl = tenantConfiguration.ApplicationApiUrl;
            ApplicationApiVersion = tenantConfiguration.ApplicationApiVersion;

            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException("Missing appId", nameof(appId));
            }

            if (string.IsNullOrEmpty(appSecret))
            {
                throw new ArgumentException("Missing appSecret", nameof(appSecret));
            }

            apiBaseUrl = new Uri(applicationApiUrl);
            version = ApplicationApiVersion ?? "1";

            if (apiBaseUrl is null)
            {
                throw new ArgumentNullException(nameof(apiBaseUrl));
            }

            realtimeBaseUrl = tenantConfiguration.RealtimeApiUrl;

            credential = new HmacCredential()
            {
                Id = new Guid(appId),
                Secret = appSecret
            };

            this.appId = appId.ToString();

            profileSystem = SM.GetSystem<AuthenticationSystem>();

            return base.Init();
        }

        #endregion

        #region ApiServer

        public Uri GetApplicationUri() => !string.IsNullOrEmpty(applicationUrl) ? new Uri(applicationUrl, UriKind.Absolute) : null;

        #endregion

        #region ApiServer

        public async Task<bool> IsAlive()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, "health", authentication: EAuthentication.None);
            _ = await request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;

            if (success)
            {
                ApiResponse<DateTime?> serverTimeResponse = new(request.responseCode, request.error, request.downloadHandler.text);
                DateTime? serverTime = serverTimeResponse.Content;

                if (serverTime.HasValue)
                {
                    serverTimeOffset = DateTime.UtcNow - serverTime.Value;
                    Debug.Log($"Server time: {serverTime.Value}, client time offset: {serverTimeOffset}");
                }
                else
                {
                    Debug.LogWarning($"Unable to retrieve server time");
                }
            }

            return success;
        }

        #endregion

        #region Assets

        public async Task<ApiResponse<AssetDTO>> DeleteAsset(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbDELETE, $"worlds/{worldId}/assets/{assetId}");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> GetAssetDetails(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/{assetId}");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> DeleteAssetContent(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbDELETE, $"worlds/{worldId}/assets/{assetId}/content");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<string>> GetAssetContent(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/{assetId}/content");
            await request.SendWebRequest();

            return new ApiResponse<string>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> UpdateAssetFolder(int worldId, int assetId, string folderName)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/assets/{assetId}/folder", body: folderName);
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> UpdateAssetLabel(int worldId, int assetId, string newLabel)
        {
            Dictionary<string, string> queryParams = new()
        {
            { "value" , newLabel}
        };
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/assets/{assetId}/label", queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> UpdateAssetOwner(int worldId, int assetId, int newOwner)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/assets/{assetId}/owner", body: newOwner.ToString());
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> SetAssetPrivate(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/assets/{assetId}/setprivate");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> SetAssetPublic(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/assets/{assetId}/setpublic");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<AssetDTO>> DeleteAssetThumbnail(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbDELETE, $"worlds/{worldId}/assets/{assetId}/thumbnail");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<string>> GetAssetThumbnail(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/{assetId}/thumbnail");
            await request.SendWebRequest();

            return new ApiResponse<string>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<FolderDTO>> GetWorldFolders(int worldId, string order = "name", bool? includeDetails = null, int currentPage = 1, int pageSize = 8)
        {
            Dictionary<string, string> queryParams = new()
            {
                { "currentPage" , currentPage.ToString()},
                { "pageSize" , pageSize.ToString()},
                { "order" , order},
            };
            if (includeDetails != null)
            {
                queryParams.Add("includeDetails", includeDetails.ToString().ToLower());
            }

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/folders", queryParams: queryParams);
            await request.SendWebRequest();

            int totalCount = 0;
            if (request.GetResponseHeaders().TryGetValue("x-total-count", out var count))
            {
                totalCount = int.Parse(count.ToString());
            }

            return new ApiResponseArray<FolderDTO>(request.responseCode, request.error, request.downloadHandler.text, totalCount);
        }

        public async Task<ApiResponseArray<AssetDTO>> GetWorldAssets(int worldId, bool? buildSasContentUrl = null, bool? buildSasThumbnailUrl = null, int startItem = 1, int pageSize = 2000000000, string order = "label", string filterExtensions = null, string filterFolder = null)
        {
            Dictionary<string, string> queryParams = new()
            {
                { "startItem" , startItem.ToString()},
                { "pageSize" , pageSize.ToString()},
                { "order" , order}
            };

            if (buildSasContentUrl != null)
            {
                queryParams.Add("buildSasContentUrl", buildSasContentUrl.ToString().ToLower());
            }
            if (buildSasThumbnailUrl != null)
            {
                queryParams.Add("buildSasThumbnailUrl", buildSasThumbnailUrl.ToString().ToLower());
            }
            if (filterExtensions != null)
            {
                queryParams.Add("filterExtensions", filterExtensions);
            }
            if (filterFolder != null)
            {
                queryParams.Add("filterFolder", filterFolder);
            }

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets", queryParams: queryParams);
            await request.SendWebRequest();

            int totalCount = 0;
            if (request.GetResponseHeaders().TryGetValue("x-total-count", out var count))
            {
                totalCount = int.Parse(count.ToString());
            }

            return new ApiResponseArray<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text, totalCount);
        }

        //API details at https://sharing.clickup.com/t/h/c/2525524/REFL-2324/QPORG0ADY0HDK20
        public async Task<ApiResponseArray<AssetDTO>> GetEventAssets(int worldId, int eventId, int startItem, int pageSize, string filterExtensions = null, string filterFolder = null, string order = "label")
        {
            Dictionary<string, string> queryParams = new()
        {
            { "startItem" , startItem.ToString()},
            { "pageSize" , pageSize.ToString() },
            { "order", order },
            { "includeAllVisible", true.ToString() },
            { "buildSasContentUrl", false.ToString() },
            { "buildSasThumbnailUrl", true.ToString() }
        };
            if (filterExtensions != null)
            {
                queryParams.Add("filterExtensions", filterExtensions);
            }
            if (filterFolder != null)
            {
                queryParams.Add("filterFolder", filterFolder);
            }

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/assets", queryParams, allowEmptyQueryValues: true);
            await request.SendWebRequest();
            int totalCount = 0;
            if (request.GetResponseHeaders().TryGetValue("x-total-count", out var count))
            {
                totalCount = int.Parse(count.ToString());
            }
            return new ApiResponseArray<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text, totalCount);
        }

        //API details at https://sharing.clickup.com/t/h/c/2525524/REFL-2324/QPORG0ADY0HDK20
        public async Task<ApiResponseArray<FolderDTO>> GetEventAssetsFolders(int worldId, int eventId, int startItem, int pageSize, string filterExtensions = null, string order = "name")
        {
            Dictionary<string, string> queryParams = new()
        {
            { "startItem" , startItem.ToString()},
            { "pageSize" , pageSize.ToString() },
            { "order", order },
            { "includeAllVisible", true.ToString() },
        };
            if (filterExtensions != null)
            {
                queryParams.Add("filterExtensions", filterExtensions);
            }

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/assets/folders", queryParams);
            await request.SendWebRequest();
            int totalCount = 0;
            if (request.GetResponseHeaders().TryGetValue("x-total-count", out var count))
            {
                totalCount = int.Parse(count.ToString());
            }

            return new ApiResponseArray<FolderDTO>(request.responseCode, request.error, request.downloadHandler.text, totalCount);
        }


        public async Task<ApiResponseArray<AssetDTO>> SearchAssets(int worldId, AssetSearchCriteria assetSearchCriteria, string order = "label", int startItem = 1, int pageSize = 2000000000)
        {
            Dictionary<string, string> queryParams = new()
            {
                { "startItem" , startItem.ToString()},
                { "pageSize" , pageSize.ToString()},
                { "order" , order},
            };
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"search/assets/{worldId}", body: JsonConvert.SerializeObject(assetSearchCriteria), queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponseArray<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<string>> DownloadAssetContent(int worldId, int? id = null, int? ts = null, int? sig = null)
        {
            Dictionary<string, string> queryParams = new()
        {
            { "id" , id.ToString()},
            { "ts" , ts.ToString()},
            { "sig" , sig.ToString()}
        };
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/downloads/contents", queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponse<string>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<string>> DownloadAssetThumbnail(int worldId, int? id = null, int? ts = null, int? sig = null)
        {
            Dictionary<string, string> queryParams = new()
        {
            { "id" , id.ToString()},
            { "ts" , ts.ToString()},
            { "sig" , sig.ToString()}
        };
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/downloads/thumbnails", queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponse<string>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Enviroments

        public async Task<ApiResponseArray<EnvironmentDTO>> GetEnvironments(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/environments");
            await request.SendWebRequest();

            return new ApiResponseArray<EnvironmentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<EnvironmentDTO>> GetEnvironment(int worldId, int environmentId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/environments/{environmentId}");
            await request.SendWebRequest();

            return new ApiResponse<EnvironmentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<EnvironmentDTO>> GetEnvironmentStorageFiles(int worldId, string filename)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/environments/download/{filename}");
            await request.SendWebRequest();

            return new ApiResponse<EnvironmentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseSearch<EnvironmentDTO>> SearchEnvironment(int worldId, EnvironmentSearchCriteria environmentSearchCriteria, string order, int currentPage = 1, int pageSize = 2000000000)
        {
            Dictionary<string, string> queryParams = new()
        {
            { "currentPage" , currentPage.ToString()},
            { "pageSize" , pageSize.ToString()},
        };
            if (order != null)
            {
                queryParams.Add("order", order.ToString());
            }
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/environments/search", body: JsonUtility.ToJson(environmentSearchCriteria), queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponseSearch<EnvironmentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Experience
        public async Task<ApiResponseArray<ExperienceDTO>> GetWorldExperiences(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences");
            await request.SendWebRequest();
            //var envs = await GetEnvironments(worldId);
            //List<ExperienceDTO> experiences = new();
            //foreach (var env in envs.Content)
            //{
            //    experiences.Add(new ExperienceDTO()
            //    {
            //        Id = env.Id,
            //        Label = env.Label,
            //        Description = env.Description,
            //        EnvironmentId = env.Id,
            //        OwnerUserId = env.OwnerUserId,
            //        Spotlight = true,
            //        Tags = env.Tags,
            //        Status = ExperienceDTO.EExperienceStatusOption.Published,
            //        Type = ExperienceDTO.EExperienceTypeOption.Core,
            //        ThumbnailUri = env.ThumbnailUri,
            //        Config = "",
            //    });
            //}
            return new ApiResponseArray<ExperienceDTO>(request.responseCode, request.error,/* JsonConvert.SerializeObject(experiences)*/ request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<ExperienceDTO>> GetMyExperiences(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences/my");
            await request.SendWebRequest();
            return new ApiResponseArray<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }


        public async Task<ApiResponse> DeleteExperience(int worldId, int expId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbDELETE, $"worlds/{worldId}/experiences/{expId}");
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }
        public async Task<ApiResponse<ExperienceDTO>> DuplicateExperience(int worldId, int expId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/experiences/authored/{expId}/duplicate");
            await request.SendWebRequest();
            return new ApiResponse<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }
        public async Task<ApiResponse<ExperienceDTO>> UpdateExperience(int worldId, ExperienceDTO exp)
        {
            List<IMultipartFormSection> formDataSections = new List<IMultipartFormSection>();
            if (!string.IsNullOrEmpty(exp.Label))
            {
                formDataSections.Add(new MultipartFormDataSection("label", exp.Label));
            }
            if (!string.IsNullOrEmpty(exp.Description))
            {
                formDataSections.Add(new MultipartFormDataSection("description", exp.Description));
            }
            if (exp.Tags != null && exp.Tags.Length > 0)
            {
                string tagIds = "";
                foreach (var tag in exp.Tags)
                {
                    tagIds += tag.Id + ",";
                }
                formDataSections.Add(new MultipartFormDataSection("tagIds", tagIds));
            }

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/experiences/{exp.Id}/metadata", requestBodyType: ERequestBodyType.MultipartFormData, body: formDataSections);

            await request.SendWebRequest();

            return new ApiResponse<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse> ToggleExperienceStatus(int worldId, int expId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/experiences/{expId}/togglestatus");
            await request.SendWebRequest();
            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
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

        public async Task<ApiResponseArray<SessionDTO>> GetLiveSessions(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/livenow");
            await request.SendWebRequest();
            return new ApiResponseArray<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }
        public async Task<ApiResponseArray<SessionDTO>> GetMonthSessions(int worldId, int monthOffset)
        {
            Dictionary<string, string> queryParams = new()
            {
                { "monthOffset" , monthOffset.ToString()},
            };
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/month", queryParams);
            await request.SendWebRequest();
            return new ApiResponseArray<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }


        public async Task<ApiResponseArray<SessionDTO>> GetMySessions(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/my");
            await request.SendWebRequest();
            return new ApiResponseArray<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }


        public async Task<ApiResponse<SessionDTO>> DeleteSession(int worldId, int eventId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbDELETE, $"worlds/{worldId}/sessions/{eventId}");
            await request.SendWebRequest();
            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<SessionDTO>> GetSession(int worldId, int sessionId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/{sessionId}");
            await request.SendWebRequest();

            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<SessionDTO>> UpdateSessionMetadata(int worldId, int sessionId, NewSessionDTO eventChanges)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/sessions/{sessionId}/metadata", body: JsonConvert.SerializeObject(eventChanges));
            await request.SendWebRequest();

            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }



        public async Task<ApiResponse<SessionDTO>> ShareEventAsset(int worldId, int sessionId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/sessions/{sessionId}/assets/{assetId}/share");
            await request.SendWebRequest();

            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }


        public async Task<ApiResponse> UpdateExperienceSaveData(int worldId, int eventId, object assetData)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/sessions/{eventId}/experienceConfig", body: JsonConvert.SerializeObject(assetData));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<ExperienceDTO>> CreateAuthoredExperience(int worldId, NewExperienceDTO newExperienceDTO)
        {
            // 1. Create a list to hold all your multipart form sections
            List<IMultipartFormSection> formDataSections = new List<IMultipartFormSection>();

            // 2. Add simple string and integer fields using MultipartFormDataSection
            if (!string.IsNullOrEmpty(newExperienceDTO.Label))
            {
                formDataSections.Add(new MultipartFormDataSection("label", newExperienceDTO.Label));
            }
            if (!string.IsNullOrEmpty(newExperienceDTO.Description))
            {
                formDataSections.Add(new MultipartFormDataSection("description", newExperienceDTO.Description));
            }
            formDataSections.Add(new MultipartFormDataSection("spotlight", newExperienceDTO.Spotlight.ToString())); // Convert bool to string
            formDataSections.Add(new MultipartFormDataSection("environmentId", newExperienceDTO.EnvironmentId.ToString()));
            formDataSections.Add(new MultipartFormDataSection("status", newExperienceDTO.Status.ToString())); // Convert enum to string

            // Handle nullable ParentId
            if (newExperienceDTO.ParentId.HasValue)
            {
                formDataSections.Add(new MultipartFormDataSection("parentId", newExperienceDTO.ParentId.Value.ToString()));
            }

            if (newExperienceDTO.Tags != null && newExperienceDTO.Tags.Length > 0)
            {
                string tagIds = "";
                foreach (var tagId in newExperienceDTO.Tags)
                {
                    tagIds += tagId + ",";
                }
                formDataSections.Add(new MultipartFormDataSection("tagIds", tagIds));
            }

            if (newExperienceDTO.Config != null)
            {
                formDataSections.Add(new MultipartFormDataSection("config", JsonConvert.SerializeObject(newExperienceDTO.Config)));
            }

            // 3. Call BuildRequest with the correct ERequestBodyType and the list of sections
            using UnityWebRequest request = await BuildRequest(
                UnityWebRequest.kHttpVerbPOST,
                $"worlds/{worldId}/experiences/authored",
                requestBodyType: HttpSystem.ERequestBodyType.MultipartFormData, // Specify the new enum value
                body: formDataSections // Pass the list of IMultipartFormSection as the body
            );

            // 4. Send the request and process the response
            await request.SendWebRequest();

            return new ApiResponse<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse> UpdateEventAssets(int worldId, int eventId, int[] assetIds)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/sessions/{eventId}/updateassets", body: JsonConvert.SerializeObject(assetIds));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }


        public async Task<ApiResponse> UpdateEventInvitedUsers(int worldId, int eventId, UsersInvitationDTO usersInvitationPostDTO)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/sessions/{eventId}/users", body: JsonConvert.SerializeObject(usersInvitationPostDTO));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Facets

        public async Task<ApiResponseArray<FacetDTO>> GetFacets()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"facets/app/{app}");
            await request.SendWebRequest();

            return new ApiResponseArray<FacetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Schedule

        public async Task<ApiResponse<ScheduleDTO>> GetScheduleOfDayName(string dayName)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"schedule/{dayName}");
            await request.SendWebRequest();

            return new ApiResponse<ScheduleDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Keys

        public async Task<ApiResponse<KeysDTO>> GetKeys()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"keys");
            await request.SendWebRequest();

            return new ApiResponse<KeysDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Permissions

        public async Task<ApiResponse<List<string>>> GetMySessionPermissions(int eventId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"sessions/{eventId}/app/{app}/permissions/my");
            await request.SendWebRequest();

            return new ApiResponse<List<string>>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<PermissionDTO>> GetEventPermissionsByTag(int eventId, int tagId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"sessions/{eventId}/app/{app}/permissions/tags/{tagId}");
            await request.SendWebRequest();

            return new ApiResponseArray<PermissionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<PermissionDTO>> GetAllPermissionByTag(int tagId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"sessions/app/{app}/permissions/tags/{tagId}");
            await request.SendWebRequest();

            return new ApiResponseArray<PermissionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<PermissionDTO>> CreateEventPermission(PermissionDTO permissionPostDTO)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"sessions/permissions", body: JsonConvert.SerializeObject(permissionPostDTO));
            await request.SendWebRequest();

            return new ApiResponse<PermissionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<PermissionDTO>> CreateEventPermissions(List<PermissionDTO> permissionPostDTO)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"sessions/permissions/multiple", body: JsonConvert.SerializeObject(permissionPostDTO));
            await request.SendWebRequest();

            return new ApiResponse<PermissionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<List<string>>> GetMyWorldPermissions(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/app/{app}/permissions/my");
            await request.SendWebRequest();

            return new ApiResponse<List<string>>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Online Presence

        public async Task<ApiResponse<string>> OnlineClientPing(OnlineUserDTO onlineInfo)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, "online/client/ping", body: JsonConvert.SerializeObject(onlineInfo));
            await request.SendWebRequest();

            ApiResponse<string> retval = new(request.responseCode, request.error, request.downloadHandler.text);

            return retval;
        }

        public async Task<ApiResponse> OnlineUserPing(OnlineUserDTO onlineInfo)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, "online/ping", body: JsonConvert.SerializeObject(onlineInfo));
            await request.SendWebRequest();

            ApiResponse retval = new(request.responseCode, request.error, request.downloadHandler.text);

            return retval;
        }

        public async Task<ApiResponse<List<OnlineUserDTO>>> GetOnlineUsers(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/online/users");
            await request.SendWebRequest();

            ApiResponse<List<OnlineUserDTO>> retval = new(request.responseCode, request.error, request.downloadHandler.text);

            return retval;
        }


        #endregion

        #region Tags


        public async Task<ApiResponseArray<TagDTO>> GetUsersTags(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/tags/all");
            await request.SendWebRequest();

            return new ApiResponseArray<TagDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<TagDTO>> GetContentTags(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/tags/content/all");
            await request.SendWebRequest();

            return new ApiResponseArray<TagDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseSearch<TagDTO>> SearchTags(TagSearchCriteria tagSearchCriteria, string order, int currentPage = 1, int pageSize = 2000000000)
        {
            Dictionary<string, string> queryParams = new()
        {
            { "currentPage" , currentPage.ToString()},
            { "pageSize" , pageSize.ToString()},
            { "order" , order},
        };
            Debug.Log("tag search " + JsonConvert.SerializeObject(tagSearchCriteria));
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"search/tags", body: JsonConvert.SerializeObject(tagSearchCriteria), queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponseSearch<TagDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<UserDTO>> GetUsersWithTag(int worldId, int tagId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/tags/{tagId}/taggedusers");
            await request.SendWebRequest();

            return new ApiResponseArray<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<TagDTO>> GetUserTags(int worldId, int userId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/tags/user/{userId}");
            await request.SendWebRequest();

            return new ApiResponseArray<TagDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Worlds

        public async Task<ApiResponseArray<WorldDTO>> GetWorlds()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds");
            await request.SendWebRequest();

            return new ApiResponseArray<WorldDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<Dictionary<int, int?>>> GetWorldLimits()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/user/limits");
            await request.SendWebRequest();

            return new ApiResponse<Dictionary<int, int?>>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<WorldDTO>> GetWorld(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}");
            await request.SendWebRequest();

            return new ApiResponse<WorldDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<WorldConfigDTO>> GetWorldConfig(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/config");
            await request.SendWebRequest();

            return new ApiResponse<WorldConfigDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<SessionDTO>> GetWorldDefaultEvent(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/sessions/default");
            await request.SendWebRequest();

            return new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<CatalogDTO>> GetWorldCatalogs(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/catalogs");
            await request.SendWebRequest();

            return new ApiResponseArray<CatalogDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }
        #endregion

        #region Users

        public async Task<ApiResponse<UserDTO>> GetUserData(int worldId, int userId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/users/{userId}");
            await request.SendWebRequest();

            return new ApiResponse<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<object>> GetUserPreferences(int worldId, int userId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/users/{userId}/preferences");
            await request.SendWebRequest();

            return new ApiResponse<object>(request.responseCode, request.error, request.downloadHandler.text);
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

        public async Task<ApiResponse<UserDTO>> GetMyUserData(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/users/my/profile");
            await request.SendWebRequest();

            return new ApiResponse<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<UserDTO>> GetMyUserData()
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"users/my/profile");
            await request.SendWebRequest();

            return new ApiResponse<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseSearch<UserDTO>> SearchUser(int worldId, UserSearchCriteria userSearchCriteria, string order, int currentPage = 1, int pageSize = 2000000000)
        {
            Dictionary<string, string> queryParams = new()
        {
            { "currentPage" , currentPage.ToString()},
            { "pageSize" , pageSize.ToString()},
            { "order" , order},
        };
            Debug.Log("user search " + JsonUtility.ToJson(userSearchCriteria));
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"search/users/{worldId}", body: JsonUtility.ToJson(userSearchCriteria), queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponseSearch<UserDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }


        internal async Task<ApiResponse<bool>> CheckMaxTenantCCU(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/join");
            await request.SendWebRequest();

            return new ApiResponse<bool>(request.responseCode, request.error, request.downloadHandler.text);
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

        #region ErrorDiagnostic

        public async Task CreateErrorDiagnostic(ErrorDiagnosticDTO errorDiagnostic)
        {
            Debug.LogError("New diagnostic: " + JsonConvert.SerializeObject(errorDiagnostic));
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"diagnostics",
                body: JsonConvert.SerializeObject(errorDiagnostic), authentication: EAuthentication.Hmac);
            await request.SendWebRequest();
            var response = new ApiResponse<SessionDTO>(request.responseCode, request.error, request.downloadHandler.text);
            Debug.Log("New diagnostic: " + JsonConvert.SerializeObject(response));
        }

        #endregion

        #region TelemetryDiagnostics

        public async Task CreateTelemetryData(TelemetryDTO telemetryDTO)
        {
            //Debug.LogError("New telemetry data: " + JsonConvert.SerializeObject(telemetryDTO));
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"online/telemetry",
            body: JsonConvert.SerializeObject(telemetryDTO));
            await request.SendWebRequest();
            var response = new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
            //Debug.LogError("New telemetry dto: " + JsonConvert.SerializeObject(dto));
        }

        #endregion

        #region HTTP management private methods

        /// <summary>
        /// Builds a UnityWebRequest, handling query parameters, authentication, and different body types.
        /// </summary>
        /// <param name="method">The HTTP method (GET, POST, PUT, etc.).</param>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="queryParams">Query parameters for the request.</param>
        /// <param name="requestBodyType">The type of body to send (None, RawString, RawBytes, MultipartFormData).</param>
        /// <param name="body">The request body data, its type must match requestBodyType.</param>
        /// <param name="authentication">Authentication flags (Bearer, Hmac).</param>
        /// <param name="allowEmptyQueryValues">If true, includes query parameters with empty or null values.</param>
        /// <returns>A configured UnityWebRequest.</returns>
        /// <exception cref="Exception">Thrown if no valid token is found for Bearer authentication.</exception>
        public async Task<UnityWebRequest> BuildRequest(string method,
                                                        string endpoint,
                                                        Dictionary<string, string> queryParams = null,
                                                        HttpSystem.ERequestBodyType requestBodyType = HttpSystem.ERequestBodyType.RawString,
                                                        object body = null, // Changed to object
                                                        EAuthentication authentication = EAuthentication.BearerAndHmac,
                                                        bool allowEmptyQueryValues = false)
        {
            queryParams ??= new Dictionary<string, string>();

            // Filter null or empty query parameters
            // Assuming string.IsNullOrWhiteSpace is used or an extension method is properly imported
            queryParams = queryParams.Where(x => allowEmptyQueryValues ? x.Value != null : !string.IsNullOrWhiteSpace(x.Value))
                                     .ToDictionary(x => x.Key, x => x.Value);
            queryParams.Add("api-version", version);

            (string timestamp, string hmac) = httpSystem.CalculateHmacHeader(credential, DateTime.UtcNow - serverTimeOffset);

            Dictionary<string, string> headers = new()
        {
            { "AppId", appId },
            { "Timestamp", timestamp }
        };

            // Removed the Content-Type: application/json here, as it's now handled by CreateHttpRequest
            // or explicitly by the caller via headers.

            if (authentication.HasFlag(EAuthentication.Bearer))
            {
                JwtToken token = profileSystem.UserTokens.FirstOrDefault(el => el.ApiLabel == SM.GetSystem<TenantConfigurationSystem>().TenantConfiguration.Label);
                if (token == null || token.IsExpired(serverTimeOffset))
                {
                    await profileSystem.RefreshTokens();
                    token = profileSystem.UserTokens.FirstOrDefault(el => el.ApiLabel == SM.GetSystem<TenantConfigurationSystem>().TenantConfiguration.Label);
                }

                if (token == null)
                {
                    throw new Exception("No valid token found!");
                }

                headers.Add("Authorization", $"Bearer {token.Bearer}");
            }

            if (authentication.HasFlag(EAuthentication.Hmac))
            {
                headers.Add("Hmac", hmac);
            }

            CertificateHandler certificateHandler = allowUntrustedServers ? new AcceptAllCertificates() : default;

            // --- Use the new CreateHttpRequest ---
            UnityWebRequest request = httpSystem.CreateHttpRequest(
                method,
                $"{apiBaseUrl}{endpoint}",
                requestBodyType, // Pass the new enum
                body,            // Pass the object body directly
                queryParams,
                headers,
                certificateHandler);

            // No need for separate header application loop here, as CreateHttpRequest handles it
            // and its internal logic decides if Content-Type should be set/overridden.

            return request;
        }

        // Retain this method if it's used elsewhere, otherwise it's redundant with HttpSystem.AddQueryString
        private string BuildQueryParams(Dictionary<string, string> queryParams)
        {
            if (queryParams == null || queryParams.Count == 0) return "";
            var queryString = string.Join("&", queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            return $"?{queryString}";
        }

        #endregion HTTP management private methods

        #region RealtimeOnlineUsers

        /// <summary>
        /// Realtime section
        /// All responses are implemented using actions since we cannot use async methods in the websocket listener
        /// because of concurrency issues in webGL listeners
        /// </summary>


        /// <param name="browserClientId"></param>
        /// <param name="onConnectionEmbodied"> Action called when the connection has been embodied using the clientId as paramenter</param>
        /// <param name="onOnlinePresenceUpdate"></param>
        public void EmbodyConnection(string browserClientId, Action onConnectionEmbodied, Action onError)
        {
            Action<RealtimeResponseDTO> onResponseReceived = (response) =>
            {
                if (response.IsSuccess)
                {
                    onConnectionEmbodied();
                }
                else
                {
                    onError();
                    Debug.LogError($"Error registering to worlds CCU: {response.Content}");
                }
            };
            SendRequestToReflectisRealtime(ERealtimeOnlineUsersMessageKey.EmbodyUserConnection, browserClientId, onResponseReceived);
        }


        /// <summary>
        /// Register to receive updates on the online users per world
        /// </summary>
        /// <param name="onWorldCCUUpdate"></param>
        public void RegisterToWorldsCCU(Action<List<WorldOnlineUsersCountDTO>> onWorldCCUUpdate)
        {
            Action<object> onUpdate = (response) =>
            {
                onWorldCCUUpdate(JsonConvert.DeserializeObject<List<WorldOnlineUsersCountDTO>>(response.ToString()));
            };
            OpenOnlineUsersChannel(ERealtimeOnlineUsersMessageKey.UsersCountPerWorld, null, onUpdate);
        }

        public void DisconnectFromOnlineUsersPerWorld()
        {
            CloseOnlineUsersChannel(ERealtimeOnlineUsersMessageKey.UsersCountPerWorld, null);
        }

        #region Join/Leave World

        public void JoinWorld(int worldId, int eventId, Action<int> onWorldJoined)
        {
            Action<RealtimeResponseDTO> onResponseReceived = (response) =>
            {
                if (response.IsSuccess)
                {
                    int joinWorld = JsonConvert.DeserializeObject<int>(response.Content.ToString());
                    onWorldJoined(joinWorld);
                }
                else
                {
                    Debug.LogError($"Error on world join: {response.Content}");
                }
            };
            PostJoinWorldDTO joinWorldDTO = new PostJoinWorldDTO()
            {
                SessionId = eventId,
                WorldId = worldId,
            };
            SendRequestToReflectisRealtime(ERealtimeOnlineUsersMessageKey.JoinWorld, joinWorldDTO, onResponseReceived);
        }

        public void DisconnectFromWorld()
        {
            ClearOnlineRealtimeCallbacks();
            SendTriggerToReflectisRealtime(ERealtimeOnlineUsersMessageKey.LeaveWorld, null);
        }

        #endregion

        #region WorldOnlineUsers
        public void RegisterToWorldData(int worldId, Action<OnlineUserDTO[]> onWorldDataUpdate)
        {
            Action<object> onUpdate = (response) =>
            {
                onWorldDataUpdate(JsonConvert.DeserializeObject<OnlineUserDTO[]>(response.ToString()));
            };
            OpenOnlineUsersChannel(ERealtimeOnlineUsersMessageKey.WorldMultiplayerUsers, worldId, onUpdate);
        }

        public void UnregisterFromWorldData()
        {
            CloseOnlineUsersChannel(ERealtimeOnlineUsersMessageKey.WorldMultiplayerUsers, null);
        }

        #endregion

        #region JoinEvent

        public void JoinEvent(int eventId, int? shardId, Action<int> onEventJoined, Action onFail)
        {
            Action<RealtimeResponseDTO> onResponseReceived = (response) =>
            {
                if (response.IsSuccess)
                {
                    int shard = JsonConvert.DeserializeObject<int>(response.Content.ToString());
                    onEventJoined(shard);
                }
                else
                {
                    onFail();
                }
            };

            PostJoinSessionDTO joinEventDTO = new PostJoinSessionDTO()
            {
                SessionIn = eventId,
                ShardIn = shardId
            };
            SendRequestToReflectisRealtime(ERealtimeOnlineUsersMessageKey.JoinSession, joinEventDTO, onResponseReceived);
        }

        public void RegisterToSessionData(int eventId, Action<ShardDTO[]> onShardUpdate)
        {
            Action<object> onUpdate = (response) =>
            {
                onShardUpdate(JsonConvert.DeserializeObject<ShardDTO[]>(response.ToString()));
            };
            OpenOnlineUsersChannel(ERealtimeOnlineUsersMessageKey.SessionShardsInfo, eventId, onUpdate);
        }

        public void UnregisterFromSessionData(int eventId)
        {
            CloseOnlineUsersChannel(ERealtimeOnlineUsersMessageKey.SessionShardsInfo, eventId);
        }

        #endregion


        public async Task KickPlayer(string kickedUserSession)
        {
            bool kicked = false;
            Action<RealtimeResponseDTO> onResponseReceived = (response) =>
            {
                if (!response.IsSuccess)
                {
                    Debug.LogError($"Error kicking player: {response.Content}");
                }
                kicked = true;
            };
            SendRequestToReflectisRealtime(ERealtimeOnlineUsersMessageKey.KickPlayer, kickedUserSession, null);
            while (!kicked)
            {
                await Task.Yield();
            }
        }



        #region Shard
        public async Task EnableShard(bool enable)
        {
            bool responseReceived = false;
            Action<RealtimeResponseDTO> onResponseReceived = (x) =>
            {
                responseReceived = true;
            };
            bool isClose = !enable;
            SendRequestToReflectisRealtime(ERealtimeOnlineUsersMessageKey.UpdateShardStatus, isClose, onResponseReceived);
            while (!responseReceived)
            {
                await Task.Yield();
            }
        }

        #endregion

        #region Channel management
        private void SendTriggerToReflectisRealtime(ERealtimeOnlineUsersMessageKey key, object data)
        {
            string message = JsonConvert.SerializeObject(new RealtimeOnlineUsersMessage()
            {
                Type = ERealtimeOnlineUsersMessageType.Trigger,
                Key = key,
                Value = data
            });
            SendMessageToRealtimeWebSocket(message);
        }

        #region Request/Response management
        private Action<ERealtimeOnlineUsersMessageKey, RealtimeResponseDTO> onOnlineUsersRealtimeResponseReceived;

        private void SendRequestToReflectisRealtime(ERealtimeOnlineUsersMessageKey key, object data, Action<RealtimeResponseDTO> onResponseReceived)
        {
            string message = JsonConvert.SerializeObject(new RealtimeOnlineUsersMessage()
            {
                Type = ERealtimeOnlineUsersMessageType.Request,
                Key = key,
                Value = data
            });

            if (onResponseReceived != null)
            {
                Action<ERealtimeOnlineUsersMessageKey, RealtimeResponseDTO> callback = null;

                callback = (responseKey, value) =>
                {
                    if (responseKey == key)
                    {
                        onResponseReceived(value);
                        onOnlineUsersRealtimeResponseReceived -= callback;
                    }
                };

                onOnlineUsersRealtimeResponseReceived += callback;
            }
            SendMessageToRealtimeWebSocket(message);
        }
        #endregion



        private Dictionary<ERealtimeOnlineUsersMessageKey, Action<object>> onlineUsersRealtimeChannels = new Dictionary<ERealtimeOnlineUsersMessageKey, Action<object>>();

        private void OpenOnlineUsersChannel(ERealtimeOnlineUsersMessageKey key, object data, Action<object> onMessageReceived)
        {
            string message = JsonConvert.SerializeObject(new RealtimeOnlineUsersMessage()
            {
                Type = ERealtimeOnlineUsersMessageType.Subscribe,
                Key = key,
                Value = data
            });

            if (onMessageReceived != null)
            {
                Action<ERealtimeOnlineUsersMessageKey, RealtimeResponseDTO> callback = null;
                //First we subscribe to dto listener to wait for the first dto
                //Then we remove the listener and open the channel that will receive broadcast messages
                callback = (responseKey, value) =>
                {
                    if (responseKey == key)
                    {
                        if (value.IsSuccess)
                        {
                            onMessageReceived(value.Content);
                            onlineUsersRealtimeChannels.TryAdd(key, onMessageReceived);
                            onOnlineUsersRealtimeResponseReceived -= callback;
                        }
                        else
                        {
                            Debug.LogError($"Error subscribing to channel {key}: {value.Content}");
                        }
                    }
                };

                onOnlineUsersRealtimeResponseReceived += callback;
            }

            SendMessageToRealtimeWebSocket(message);
        }

        private void CloseOnlineUsersChannel(ERealtimeOnlineUsersMessageKey key, object data)
        {
            onlineUsersRealtimeChannels.Remove(key);

            string message = JsonConvert.SerializeObject(new RealtimeOnlineUsersMessage()
            {
                Type = ERealtimeOnlineUsersMessageType.Unsubscribe,
                Key = key,
                Value = data
            });

            SendMessageToRealtimeWebSocket(message);
        }

        private void SendMessageToRealtimeWebSocket(string message)
        {
            SM.GetSystem<IWebSocketSystem>().SendMessageAsync(OnlineUsersRealtimeApiUrl, message);
            Debug.Log("<color=green>[Sent message to Reflectis Realtime]: </color>" + message);
        }

        #endregion

        #region Realtime Socket Management
        private Action<ECloseConnectionReason> onOnlineUsersCloseSession;

        private Action<Handshake> onUserSessionStarted;

        private Action<object> onUserKick;

        private Action onDoubleSessionLogin;

        private void ClearOnlineRealtimeCallbacks()
        {
            onOnlineUsersRealtimeResponseReceived = null;
            onlineUsersRealtimeChannels.Clear();
            onOnlineUsersCloseSession = null;
            onUserSessionStarted = null;
            onUserKick = null;
        }

        /// <summary>
        /// Start session with Reflectis Realtime API
        /// on session start the onSessionStarted action is called with the clientId as parameter
        /// </summary>
        /// <param name="onSessionStarted"></param>
        public async void ConnectToReflectisRealtime(Action<Handshake> onSessionStarted, Action<ECloseConnectionReason> onDisconnect, Action<object> onKick, Action onDoubleSessionLogin)
        {
            var worldCCUListener = new BaseWebSocketListener();
            worldCCUListener.onMessageReceived += OnRealtimeMessageReceived;
            worldCCUListener.onClose += () =>
            {
                if (Application.isPlaying)
                {
                    onOnlineUsersCloseSession?.Invoke(ECloseConnectionReason.ServerDisconnection);
                }
            };

            Action<ECloseConnectionReason> onDisconnectAndRemove = null;
            onDisconnectAndRemove = (reason) =>
            {
                onDisconnect?.Invoke(reason);
                onOnlineUsersCloseSession -= onDisconnectAndRemove;
                ClearOnlineRealtimeCallbacks();
            };

            onOnlineUsersCloseSession += onDisconnectAndRemove;

            this.onDoubleSessionLogin += onDoubleSessionLogin;

            onUserKick += onKick;
            IWebSocketSystem webSocketSystem = SM.GetSystem<IWebSocketSystem>();
            if (webSocketSystem != null)
            {
                webSocketSystem.AddListener(OnlineUsersRealtimeApiUrl, worldCCUListener);

                void onSessionStartedAndRemove(Handshake x)
                {
                    onSessionStarted?.Invoke(x);
                    onUserSessionStarted -= onSessionStartedAndRemove;
                }

                onUserSessionStarted += onSessionStartedAndRemove;

                await ConnectToWebSocket(OnlineUsersRealtimeApiUrl, (x) =>
                {
                    onDisconnect(ECloseConnectionReason.ServerDisconnection);
                    Debug.LogError("Cannot connect to websocket " + OnlineUsersRealtimeApiUrl + "! \nMessage: " + x);
                });
            }
        }
        public async Task DisconnectFromReflectisRealtime()
        {
            //Debug.LogError("Disconnect");

            //SendTriggerToReflectisRealtime(ERealtimeOnlineUsersMessageKey.EndUserSession, null);

            ClearOnlineRealtimeCallbacks();
            await SM.GetSystem<IWebSocketSystem>()?.DisconnectAsync(OnlineUsersRealtimeApiUrl);
        }

        private void OnRealtimeMessageReceived(string obj)
        {
            Debug.Log("<color=yellow>[Received message from Reflectis Realtime]: </color>" + obj);
            RealtimeOnlineUsersMessage realtimeOnlineUsersMessage = JsonConvert.DeserializeObject<RealtimeOnlineUsersMessage>(obj);

            switch (realtimeOnlineUsersMessage.Type)
            {
                case ERealtimeOnlineUsersMessageType.Response:
                    onOnlineUsersRealtimeResponseReceived?.Invoke(realtimeOnlineUsersMessage.Key, JsonConvert.DeserializeObject<RealtimeResponseDTO>(realtimeOnlineUsersMessage.Value.ToString()));
                    break;
                case ERealtimeOnlineUsersMessageType.Request:
                    Debug.Log("Server request: " + realtimeOnlineUsersMessage.Key);
                    break;
                case ERealtimeOnlineUsersMessageType.Subscribe:
                    Debug.LogError("Server sent a subscribed message: " + realtimeOnlineUsersMessage.Key);
                    break;
                case ERealtimeOnlineUsersMessageType.Broadcast:
                    if (onlineUsersRealtimeChannels.ContainsKey(realtimeOnlineUsersMessage.Key))
                    {
                        onlineUsersRealtimeChannels[realtimeOnlineUsersMessage.Key]?.Invoke(realtimeOnlineUsersMessage.Value);
                    }
                    else
                    {
                        Debug.LogError("No action registered for broadcast message: " + realtimeOnlineUsersMessage.Key);
                    }
                    break;
                case ERealtimeOnlineUsersMessageType.Unsubscribe:
                    onlineUsersRealtimeChannels.Remove(realtimeOnlineUsersMessage.Key);
                    break;
                case ERealtimeOnlineUsersMessageType.Trigger:
                    switch (realtimeOnlineUsersMessage.Key)
                    {
                        case ERealtimeOnlineUsersMessageKey.EndUserSession:
                            if (Enum.TryParse(typeof(ECloseConnectionReason), realtimeOnlineUsersMessage.Value.ToString(), out object reason))
                            {
                                onOnlineUsersCloseSession?.Invoke((ECloseConnectionReason)reason);
                            }
                            break;
                        case ERealtimeOnlineUsersMessageKey.Handshake:
                            onUserSessionStarted?.Invoke(new Handshake() { ConnectionId = realtimeOnlineUsersMessage.Value.ToString() });
                            break;
                        case ERealtimeOnlineUsersMessageKey.Kicked:
                            onUserKick?.Invoke(realtimeOnlineUsersMessage.Value);
                            break;
                        case ERealtimeOnlineUsersMessageKey.NewConnectionLogin:
                            onDoubleSessionLogin?.Invoke();
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region SocketManagement

        private async Task<bool> ConnectToWebSocket(string url, Action<string> onWebSocketError = null)
        {
            JwtToken token = SM.GetSystem<AuthenticationSystem>().UserTokens.FirstOrDefault(x => x.ApiLabel == SM.GetSystem<TenantConfigurationSystem>().TenantConfiguration.Label + "Realtime");
            if (token == null)
            {
                throw new Exception("No token found for realtime API");
            }

            Dictionary<string, string> queryParams = new Dictionary<string, string>()
            {
                {
                    "token",
                    token.Bearer
                }
            };
            return await SM.GetSystem<IWebSocketSystem>()?.ConnectAsync(url, queryParams);
        }

        #endregion

        #endregion
    }
}