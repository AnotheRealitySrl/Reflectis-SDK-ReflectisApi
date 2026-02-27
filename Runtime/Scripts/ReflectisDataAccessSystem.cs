using Newtonsoft.Json;

using Reflectis.SDK.Core.ApiSystem;
using Reflectis.SDK.Core.SystemFramework;
using Reflectis.SDK.Core.Utilities;
using Reflectis.SDK.Http;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

using static HttpSystem;
using static Reflectis.SDK.Core.Authentication.IAuthenticationSystem;

namespace Reflectis.SDK.ReflectisApi
{
    [CreateAssetMenu(menuName = "AnotheReality/Systems/ReflectisDataAccessSystem", fileName = "ReflectisDataAccessSystemConfig")]
    public class ReflectisDataAccessSystem : ApiSystemBase
    {
        #region Private variables

        private const string app = "Unity";

        #endregion

        #region Properties

        public string ApiVersion => apiConfig.ApiVersion;

        #endregion

        #region Overrides

        public override async Task Init()
        {
            httpSystem = SM.GetSystem<HttpSystem>();

            await base.Init();

            //apiConfig = new AppIdentification(apiConfig.Credential,
            //    "https://localhost:12026", apiConfig.ApiVersion);
        }

        #endregion

        #region ApiServer

        public Uri GetApplicationUri() => !string.IsNullOrEmpty(apiConfig.ApiBaseUrl) ? new Uri(apiConfig.ApiBaseUrl, UriKind.Absolute) : null;

        public int CacheId { get; set; } = -1;

        #endregion

        #region Assets

        public async Task<ApiResponse<AssetDTO>> GetAssetDetails(int worldId, int assetId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/assets/{assetId}");
            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
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


        public async Task<ApiResponse<FolderContentDTO>> GetFolderContent(int worldId, int folderId, string filterExtensions = null, bool? buildSasContentUrl = null, bool? buildSasThumbnailUrl = null, int startItem = 1, int pageSize = 50, string order = "label")
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

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/folders/{folderId}/content", queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponse<FolderContentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponseSearch<AssetDTO>> GetFolderAssets(int worldId, int folderId, string filterExtensions = null, bool? buildSasContentUrl = null, bool? buildSasThumbnailUrl = null, int startItem = 1, int pageSize = 50, string order = "label")
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

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/folders/{folderId}/assets", queryParams: queryParams);
            await request.SendWebRequest();

            return new ApiResponseSearch<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        //API details at https://sharing.clickup.com/t/h/c/2525524/REFL-2324/QPORG0ADY0HDK20
        public async Task<ApiResponseArray<AssetDTO>> GetSessionAssets(int worldId, int sessionId, int startItem, int pageSize, string filterExtensions = null, string filterFolder = null, string order = "label")
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
        public async Task<ApiResponseArray<FolderDTO>> GetSessionAssetsFolders(int worldId, int sessionId, int startItem, int pageSize, string filterExtensions = null, string order = "name")
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

        public async Task<ApiResponse<AssetDTO>> CreateNew3dAsset(int worldId, byte[] data, string label)
        {
            List<IMultipartFormSection> formDataSections = new List<IMultipartFormSection>();

            string fileName = label + ".glb";
            string mimeType = "application/octet-stream";

            formDataSections.Add(new UnityEngine.Networking.MultipartFormFileSection("contentData", data, fileName, mimeType));

            formDataSections.Add(new MultipartFormDataSection("metadata", "{ \"contextualMenuSettings\":{ \"contextualMenuOptions\":[\"ColorPicker\", \"Explodable\", \"NonProportionalScale\", \"LockTransform\"]},\"unscaledSize\":true,\"scaleFactor\":1}"));

            formDataSections.Add(new MultipartFormDataSection("label", label));

            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/assets", requestBodyType: ERequestBodyType.MultipartFormData, body: formDataSections);

            await request.SendWebRequest();

            return new ApiResponse<AssetDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Enviroments

        public async Task<ApiResponseArray<EnvironmentDTO>> GetEnvironments(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/environments");
            await request.SendWebRequest();

            return new ApiResponseArray<EnvironmentDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        #endregion

        #region Experience
        public async Task<ApiResponseArray<ExperienceDTO>> GetWorldExperiences(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences");
            await request.SendWebRequest();

            return new ApiResponseArray<ExperienceDTO>(request.responseCode, request.error,/* JsonConvert.SerializeObject(experiences)*/ request.downloadHandler.text);
        }

        public async Task<ApiResponseArray<ExperienceDTO>> GetMyExperiences(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences/my");
            await request.SendWebRequest();
            return new ApiResponseArray<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse<ExperienceDTO>> GetExperience(int worldId, int experienceId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences/{experienceId}");
            await request.SendWebRequest();
            return new ApiResponse<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
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

        public async Task<ApiResponse> ShareExperienceAssets(int worldId, int experienceId, List<int> assetsId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/experiences/{experienceId}/assets/share", body: JsonConvert.SerializeObject(assetsId));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }
        public async Task<ApiResponse> ShareSessionAssets(int worldId, int sessionId, List<int> assetsId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/sessions/{sessionId}/assets/share", body: JsonConvert.SerializeObject(assetsId));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }

        public async Task<ApiResponse> UpdateExperienceSaveData(int worldId, int eventId, object assetData)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/sessions/{eventId}/experienceConfig", body: JsonConvert.SerializeObject(assetData));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
        }
        public async Task<ApiResponse> UpdateSessionSaveData(int worldId, int sessionId, object assetData)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPUT, $"worlds/{worldId}/sessions/{sessionId}/sessionConfig", body: JsonConvert.SerializeObject(assetData));
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

        public async Task<ApiResponse> UpdateEventInvitedUsers(int worldId, int eventId, UsersInvitationDTO usersInvitationPostDTO)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbPOST, $"worlds/{worldId}/sessions/{eventId}/users", body: JsonConvert.SerializeObject(usersInvitationPostDTO));
            await request.SendWebRequest();

            return new ApiResponse(request.responseCode, request.error, request.downloadHandler.text);
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

        public async Task<ApiResponse<ExperienceDTO>> GetDefaultExperience(int worldId)
        {
            using UnityWebRequest request = await BuildRequest(UnityWebRequest.kHttpVerbGET, $"worlds/{worldId}/experiences/default");
            await request.SendWebRequest();
            return new ApiResponse<ExperienceDTO>(request.responseCode, request.error, request.downloadHandler.text);
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