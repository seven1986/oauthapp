using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OAuthApp.Services
{
    #region Basic
    public class AzureApiManagementEntities<T>
    {
        public List<T> value { get; set; }

        public int count { get; set; }

        public string nextLink { get; set; }
    }

    public class AzureApiManagement
    {
        protected string host;

        protected string apiId;

        protected string apiKey;

        protected const string apiversion = "?api-version=2018-01-01";

        #region Token
        string _token { get; set; }
        DateTime tokenExpiry { get; set; }
        protected string token
        {
            get
            {
                if (string.IsNullOrEmpty(_token) ||
                    (tokenExpiry - DateTime.UtcNow).Days < 1)
                {
                    tokenExpiry = DateTime.UtcNow.AddDays(10);

                    using (var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(apiKey)))
                    {
                        var dataToSign = apiId + "\n" + tokenExpiry.ToString("O", CultureInfo.InvariantCulture);

                        var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));

                        var signature = Convert.ToBase64String(hash);

                        var encodedToken = string.Format("SharedAccessSignature uid={0}&ex={1:o}&sn={2}", apiId, tokenExpiry, signature);

                        _token = encodedToken;
                    }
                }

                return _token;
            }
        }
        #endregion

        public AzureApiManagement(string _host,
            string _apiId,
            string _apiKey)
        {
            host = _host;
            apiId = _apiId;
            apiKey = _apiKey;
        }

        #region Request Helpers
        protected Task<HttpResponseMessage> _GetAsync(string url) => RequestAsync(url, HttpMethod.Get.Method);
        protected Task<HttpResponseMessage> _PutAsync(string url) => RequestAsync(url, HttpMethod.Put.Method);
        protected Task<HttpResponseMessage> _PostAsync(string url) => RequestAsync(url, HttpMethod.Post.Method);
        protected Task<HttpResponseMessage> _DeleteAsync(string url) => RequestAsync(url, HttpMethod.Delete.Method);
        protected Task<HttpResponseMessage> _HeadAsync(string url) => RequestAsync(url, HttpMethod.Head.Method);
        #endregion

        /// <summary>
        /// Request for Rest API Management
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="method">/POST/PUT/GET,etc.</param>
        /// <param name="query">query parameters</param>
        /// <param name="content">http content</param>
        /// <param name="headerItems">request Headers</param>
        /// <param name="mediaType">request media type,default is application/json</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> RequestAsync(
           string path,
           string method,
           Dictionary<string, string> query = null,
           HttpContent content = null,
           Dictionary<string, string> headerItems = null,
           string mediaType = "application/json")
        {
            var client = new HttpClient();

            client.Timeout = TimeSpan.FromSeconds(30);

            client.DefaultRequestHeaders.Add("Authorization", token);

            if (!string.IsNullOrWhiteSpace(mediaType))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            }

            if (headerItems != null)
            {
                foreach (var item in headerItems)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }

            var requestUri = host + path + apiversion;

            if (query != null)
            {
                requestUri += "&" + string.Join("&", query.Select(x => $"{x.Key}={x.Value}").ToArray());
            }

            var requestMessage = new HttpRequestMessage(new HttpMethod(method), requestUri);

            if (content != null)
            {
                requestMessage.Content = content;
            }

            var response = await client.SendAsync(requestMessage);

            return response;
        }
    }
    #endregion

    #region AuthorizationServers
    public class AzureApiManagementAuthorizationServerEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string clientRegistrationEndpoint { get; set; }
        public string authorizationEndpoint { get; set; }
        public List<string> authorizationMethods { get; set; }
        public List<string> clientAuthenticationMethod { get; set; }
        public List<string> tokenBodyParameters { get; set; }
        public string tokenEndpoint { get; set; }
        public bool supportState { get; set; }
        public string defaultScope { get; set; }
        public List<string> grantTypes { get; set; }
        public List<string> bearerTokenSendingMethods { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string resourceOwnerUsername { get; set; }
        public string resourceOwnerPassword { get; set; }
    }
    public class AzureApiManagementAuthorizationServers : AzureApiManagement
    {
        public AzureApiManagementAuthorizationServers(string _host,
            string _apiId,
            string _apiKey) : base(_host, _apiId, _apiKey)
        {
        }

        /// <summary>
        /// authorizationServers List
        /// </summary>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>> GetAsync()
        {
            var result = await _GetAsync("/authorizationServers");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }
    }
    #endregion

    #region Product
    public class AzureApiManagementProductEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string terms { get; set; }
        public string subscriptionRequired { get; set; }
        public string approvalRequired { get; set; }
        public string subscriptionsLimit { get; set; }
        public string state { get; set; }
    }
    public class AzureApiManagementProduct : AzureApiManagement
    {
        public AzureApiManagementProduct(string _host,
            string _apiId,
            string _apiKey) : base(_host, _apiId, _apiKey)
        {
        }

        /// <summary>
        /// Product List
        /// </summary>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementProductEntity>> GetAsync(string apiId = "")
        {
            var result = string.IsNullOrWhiteSpace(apiId) ?
                await _GetAsync("/products") :
                await _GetAsync($"/apis/{apiId}/products");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementProductEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add Api To Product
        /// </summary>
        /// <param name="pid">Product id</param>
        /// <param name="aid">Api id</param>
        /// <returns></returns>
        public async Task<bool> AddApiAsync(string pid, string aid)
        {
            var path = $"{pid}/apis/{aid}";

            var result = await _PutAsync(path);

            return result.IsSuccessStatusCode;
        }
    }
    #endregion

    #region Api
    public class AzureApiManagementApi : AzureApiManagement
    {
        AzureApiManagementProduct prdService;

        public AzureApiManagementApi(string _host,
            string _apiId,
            string _apiKey) : base(_host, _apiId, _apiKey)
        {
            prdService = new AzureApiManagementProduct(_host, _apiId, _apiKey);
        }

        /// <summary>
        /// Get Api List
        /// </summary>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementApiEntity>> GetAsync(bool expandApiVersionSet = true)
        {
            var query = new Dictionary<string, string>()
            {
                { "expandApiVersionSet", expandApiVersionSet.ToString() }
            };

            var result = await RequestAsync("/apis", HttpMethod.Get.Method, query);

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementApiEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Api List By Path
        /// </summary>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementApiEntity>> GetByPathAsync(string path)
        {
            var query = new Dictionary<string, string>()
            {
                { "$filter", $"path eq '{path}'" }
            };

            var result = await RequestAsync("/apis", HttpMethod.Get.Method, query);

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementApiEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Api Detail
        /// </summary>
        /// <param name="aid">Api Id</param>
        /// <returns></returns>
        public async Task<AzureApiManagementApiEntity> DetailAsync(string aid)
        {
            var result = await _GetAsync($"/apis/{aid}");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entity = JsonConvert.DeserializeObject<AzureApiManagementApiEntity>(data);

                return entity;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check Api exists
        /// </summary>
        /// <param name="aid">Api id</param>
        /// <returns></returns>
        public async Task<bool> MetadataAsync(string aid)
        {
            var result = await RequestAsync($"/apis/{aid}", HttpMethod.Head.Method);

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Import or Update Api By swagger Url
        /// 1，check Api exists by {aid}
        /// 2，if exists update Api
        /// 3，if not exists import Api
        /// 4，if set authorizationServerId, update Api authenticationSettings
        /// </summary>
        /// <param name="aid">Api id, not null</param>
        /// <param name="suffix">Api service suffix, not null</param>
        /// <param name="swaggerUrl">Swagger doc url, not null</param>
        /// <param name="productIds">Product id collection</param>
        /// <param name="authorizationServerId">authorize server Id</param>
        /// <param name="protocols">protocols</param>
        /// <param name="scope">scope</param>
        /// <param name="openid">openid</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ImportOrUpdateAsync(
            string aid,
            string suffix,
            string swaggerUrl,
            string[] productIds = null,
            string authorizationServerId = null,
            List<string> protocols = null,
            string scope = null,
            string openid = null)
        {
            if (string.IsNullOrWhiteSpace(aid) ||
                string.IsNullOrWhiteSpace(suffix) ||
                string.IsNullOrWhiteSpace(swaggerUrl))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotModified);
            }

            var path = $"/apis/{aid}";

            var queryParams = new Dictionary<string, string>
            {
                { "import", "false" },
                { "path", suffix },
            };

            #region content
            var body = new JObject();
            body["id"] = path;
            body["link"] = swaggerUrl;
            body["protocols"] = JsonConvert.SerializeObject(
                 protocols == null || protocols.Count < 1 ? new List<string> { "https" } : protocols);

            var content = new StringContent(body.ToString(),
                Encoding.UTF8,
                "application/vnd.swagger.link+json");
            #endregion

            #region headerItems
            Dictionary<string, string> headerItems = null;

            if (await MetadataAsync(aid))
            {
                headerItems = new Dictionary<string, string>()
                {
                    { "If-Match", "*" }
                };
            }
            #endregion

            var result = await RequestAsync(path, HttpMethod.Put.Method, queryParams, content, headerItems);

            if (result.IsSuccessStatusCode)
            {
                #region Add Api to Product
                // 如果为空，设置到Unlimited 这个Product里，否则需要带上subkey才能call
                if (productIds != null && productIds.Length > 0)
                {
                    foreach (var productId in productIds)
                    {
                        try
                        {
                            var addApiResult = await prdService.AddApiAsync(productId, aid);
                        }
                        catch { }
                    }
                }
                #endregion

                #region Update Api OAuth2 Settings
                try
                {
                    var oAuth2result = await UpdateOAuth2Async(aid, authorizationServerId, scope, openid);
                }
                catch { }
                #endregion
            }

            return result;
        }

        /// <summary>
        /// Update Api authenticationSettings
        /// </summary>
        /// <param name="aid">Api Id</param>
        /// <param name="authorizationServerId">oauth ServerId</param>
        /// <param name="scope">scopes</param>
        /// <param name="openid">openid</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateOAuth2Async(string aid, string authorizationServerId, string scope = null, string openid = null)
        {
            var body = new JObject();

            body["id"] = $"/apis/{aid}";

            if (!string.IsNullOrWhiteSpace(authorizationServerId))
            {
                body["authenticationSettings"] = JObject.FromObject(new
                {
                    oAuth2 = new
                    {
                        authorizationServerId,
                        scope
                    },

                    openid
                });
            }

            else
            {
                body["authenticationSettings"] = JObject.FromObject(new
                {
                    oAuth2 = ""
                });
            }

            var result = await UpdateAsync(aid, body.ToString());

            return result;
        }

        /// <summary>
        /// Update Api
        /// </summary>
        /// <param name="aid">id</param>
        /// <param name="body">Model</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateAsync(string aid, string body)
        {
            var path = $"/apis/{aid}";

            var method = "PATCH";

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var headerItems = new Dictionary<string, string>() { { "If-Match", "*" } };

            var result = await RequestAsync(path, method, null, content, headerItems);

            return result;
        }

        /// <summary>
        /// Delete Api by id
        /// </summary>
        /// <param name="aid">id</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string aid)
        {
            var path = $"/apis/{aid}";

            var headerItems = new Dictionary<string, string>() { { "If-Match", "*" } };

            var result = await RequestAsync(path, HttpMethod.Delete.Method, null, null, headerItems);

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Delete Api include all revisions
        /// </summary>
        /// <param name="aid">aid</param>
        /// <returns></returns>
        public async Task<bool> DeleteAllAsync(string aid)
        {
            var result = await _DeleteAsync($"/apis/{aid}/revisions");

            if (result.IsSuccessStatusCode)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        #region Policy 策略
        /// <summary>
        /// Get Api Policy
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetPolicyAsync(string aid)
        {
            var result = await RequestAsync($"/apis/{aid}/policy",
                HttpMethod.Get.Method,
                null, null, null, "application/vnd.ms-azure-apim.policy+xml");

            return result;
        }

        /// <summary>
        /// Set Api Policy
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SetPolicyAsync(string aid, string policies)
        {
            var path = $"/apis/{aid}/policy";

            var headerItems = new Dictionary<string, string>()
            {
                { "If-Match", "*" }
            };

            var content = new StringContent(policies, Encoding.UTF8, "application/vnd.ms-azure-apim.policy.raw+xml");

            var result = await RequestAsync(path, HttpMethod.Put.Method, null, content, headerItems);

            return result;
        }

        /// <summary>
        /// Get Policy Snippets
        /// </summary>
        /// <returns></returns>
        public async Task<List<AzureApiManagementApiPolicySnippet>> GetPolicySnippetsAsync()
        {
            var result = await RequestAsync("/policySnippets", HttpMethod.Get.Method);

            if (result.IsSuccessStatusCode)
            {
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync().Result;

                    var entity = JsonConvert.DeserializeObject<List<AzureApiManagementApiPolicySnippet>>(data);

                    return entity;
                }

                else
                {
                    return null;
                }
            }

            else
            {
                return null;
            }
        }
        #endregion

        #region Revisions 修订版本
        /// <summary>
        /// GetRevisions
        /// </summary>
        /// <param name="aid">7</param>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementRevisionEntity>> GetRevisionsAsync(string aid)
        {
            var result = await _GetAsync($"/apis/{aid}/revisions");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementRevisionEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create Revision
        /// </summary>
        /// <param name="aid">7</param>
        /// <param name="apiRevisionDescription">desc</param>
        /// <returns>apiRevision</returns>
        public async Task<long> CreateRevisionFromSourceApiAsync(string aid, string apiRevisionDescription)
        {
            var revisions = await GetRevisionsAsync(aid);

            var lastRevision = revisions.value.OrderBy(x => long.Parse(x.apiRevision)).LastOrDefault();

            var newApiRevision = long.Parse(lastRevision.apiRevision) + 1;

            var path = $"/apis/{aid};rev={newApiRevision}";

            var body = JsonConvert.SerializeObject(new
            {
                sourceApiId = $"/apis/{aid};rev={lastRevision.apiRevision}",
                apiRevisionDescription
            });

            var content = new StringContent(body, Encoding.UTF8, "application/vnd.ms-azure-apim.revisioninfo+json");

            var result = await RequestAsync(path, HttpMethod.Put.Method, null, content);

            return newApiRevision;
        }

        /// <summary>
        /// Create Version from a revision
        /// </summary>
        /// <param name="revisionId">/apis/7;rev=3</param>
        /// <param name="newApiId">default is guid</param>
        /// <param name="newApiName">default is revisionId apiName</param>
        /// <param name="apiVersionName">default is V2</param>
        /// <param name="apiRevisionDescription"></param>
        /// <param name="versioningScheme">default is Query(Segment/Header) </param>
        /// <param name="versionQueryName">default is api-version </param>
        /// <returns></returns>
        public async Task<bool> CreateVersionAsync(
            string revisionId,
            string apiVersionName,
            string newApiId = "",
            string newApiName = "",
            string apiRevisionDescription = "Description",
            string versioningScheme = "Query",
            string versionQueryName = "api-version")
        {
            if (string.IsNullOrWhiteSpace(apiVersionName))
            {
                throw new ArgumentNullException("apiVersionName can not be null");
            }

            apiVersionName = apiVersionName.ToLower();

            if (string.IsNullOrWhiteSpace(newApiId))
            {
                newApiId = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrWhiteSpace(newApiName))
            {
                var detail = await DetailAsync(revisionId);

                if (detail != null && !string.IsNullOrWhiteSpace(detail.name))
                {
                    newApiName = detail.name;
                }
            }

            var path = $"/apis/{newApiId}";

            var body = JsonConvert.SerializeObject(new
            {
                sourceApiId = $"/apis/{revisionId}",
                apiVersionName,
                apiRevisionDescription,

                apiVersionSet = new
                {
                    description = "test.test",
                    name = newApiName,
                    versioningScheme,
                    versionQueryName = "api-version"
                }
            });

            var content = new StringContent(body, Encoding.UTF8, "application/vnd.ms-azure-apim.revisioninfo+json");

            var result = await RequestAsync(path, HttpMethod.Put.Method, null, content);

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Update Revision
        /// </summary>
        /// <param name="aid">7;rev=3</param>
        /// <param name="apiRevisionDescription">desc</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateRevisionAsync(string aid, string apiRevisionDescription)
        {
            var body = JsonConvert.SerializeObject(new
            {
                apiRevisionDescription
            });

            var result = await UpdateAsync(aid, body);

            return result;
        }
        #endregion

        #region Releases 发布版本
        /// <summary>
        /// Get Releases
        /// </summary>
        /// <param name="aid">7</param>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementReleaseEntity>> GetReleasesAsync(string aid)
        {
            var result = await _GetAsync($"/apis/{aid}/releases");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementReleaseEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create Release
        /// </summary>
        /// <param name="aid">7</param>
        /// <param name="notes"></param>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public async Task<bool> CreateReleaseAsync(string aid, string notes, string releaseId = null)
        {
            if (string.IsNullOrWhiteSpace(releaseId)) { releaseId = DateTime.UtcNow.Ticks.ToString(); }

            var path = $"/apis/{aid}/releases/{releaseId}";

            var headerItems = new Dictionary<string, string>() { { "If-Match", "*" } };

            var body = JsonConvert.SerializeObject(new
            {
                apiId = $"/apis/{aid}",
                notes
            });

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var result = await RequestAsync(path, HttpMethod.Put.Method, null, content, headerItems);

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Update Release
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public async Task<bool> UpdateReleaseAsync(string releaseId, string notes)
        {
            var method = "PATCH";

            var headerItems = new Dictionary<string, string>() { { "If-Match", "*" } };

            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                notes
            }), Encoding.UTF8, "application/json");

            var result = await RequestAsync(releaseId, method, null, content, headerItems);

            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Delete Release
        /// </summary>
        /// <param name="releaseId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteReleaseAsync(string releaseId)
        {
            var headerItems = new Dictionary<string, string>() { { "If-Match", "*" } };

            var result = await RequestAsync(releaseId, HttpMethod.Delete.ToString(),
                null, null, headerItems);

            if (result.IsSuccessStatusCode)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region Version Set
        /// <summary>
        /// Get Version Set List
        /// </summary>
        /// <returns></returns>
        public async Task<AzureApiManagementEntities<AzureApiManagementApiVersionSetEntity>> GetVersionSetsAsync()
        {
            var result = await _GetAsync("/api-version-sets");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entities = JsonConvert.DeserializeObject<AzureApiManagementEntities<AzureApiManagementApiVersionSetEntity>>(data);

                return entities;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get Api Detail
        /// </summary>
        /// <param name="id">Api Id</param>
        /// <returns></returns>
        public async Task<AzureApiManagementApiVersionSetEntity> VersionSetDetailAsync(string id)
        {
            var result = await _GetAsync($"/api-version-sets/{id}");

            if (result.IsSuccessStatusCode)
            {
                var data = result.Content.ReadAsStringAsync().Result;

                var entity = JsonConvert.DeserializeObject<AzureApiManagementApiVersionSetEntity>(data);

                return entity;
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Update Version Set
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name">name</param>
        /// <param name="description">description</param>
        /// <returns></returns>
        public async Task<bool> UpdateVersionSetAsync(string id, string name, string description)
        {
            var path = $"/api-version-sets/{id}";

            var method = "PATCH";

            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                name,
                description
            }), Encoding.UTF8, "application/json");

            var headerItems = new Dictionary<string, string>() { { "If-Match", "*" } };

            var result = await RequestAsync(path, method, null, content, headerItems);

            return result.IsSuccessStatusCode;
        }
        #endregion
    }

    public class AzureApiManagementRevisionEntity
    {
        public string apiId { get; set; }
        public string apiRevision { get; set; }
        public string createdDateTime { get; set; }
        public string updatedDateTime { get; set; }
        public string description { get; set; }
        public string privateUrl { get; set; }
        public bool isOnline { get; set; }
        public bool isCurrent { get; set; }
    }

    public class AzureApiManagementReleaseEntity
    {
        public string id { get; set; }
        public string createdDateTime { get; set; }
        public string updatedDateTime { get; set; }
        public string notes { get; set; }
    }

    public class AzureApiManagementApiEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string apiRevision { get; set; }
        public string description { get; set; }
        public string apiVersion { get; set; }
        public string serviceUrl { get; set; }
        public bool isCurrent { get; set; }
        public string path { get; set; }
        public List<string> protocols { get; set; }
        public string apiRevisionDescription { get; set; }
        public string apiVersionSetId { get; set; }
        public JObject authenticationSettings { get; set; }
        public JObject subscriptionKeyParameterNames { get; set; }
    }

    public class AzureApiManagementApiVersionSetEntity
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string versioningScheme { get; set; }
        public string versionQueryName { get; set; }
        public string versionHeaderName { get; set; }
    }

    public class AzureApiManagementApiPolicySnippet
    {
        public string name { get; set; }
        public string content { get; set; }
        public string toolTip { get; set; }
        public string scope { get; set; }
    }
    #endregion

    #region User
    public class AzureApiManagementUser : AzureApiManagement
    {
        public AzureApiManagementUser(string _host,
            string _apiId,
            string _apiKey) : base(_host, _apiId, _apiKey)
        {
        }

        public async Task<string> GenerateSsoUrlAsync(string uid)
        {
            var path = $"/users/{uid}/generateSsoUrl";

            var response = await _PostAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = response.Content.ReadAsStringAsync().Result;

                var signOnURL = JObject.Parse(responseJson)["value"].Value<string>();

                return signOnURL;
            }

            return string.Empty;
        }

        public async Task<bool> AddAsync(string uid, string email, string password)
        {
            if (await MetadataAsync(uid)) { return true; }

            var path = $"/users/{uid}";

            var method = HttpMethod.Put.Method;

            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                firstName = "u",
                lastName = email,
                email,
                password
            }), Encoding.UTF8, "application/json");

            var result = await RequestAsync(path, method, null, content);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> MetadataAsync(string uid)
        {
            var path = $"/users/{uid}";

            var result = await _HeadAsync(path);

            return result.IsSuccessStatusCode;
        }
    }
    #endregion

    /// <summary>
    /// Azure Api Management
    /// </summary>
    public class AzureApiManagementServices
    {
        #region configs
        string host { get; set; }
        string apiId { get; set; }
        string apiKey { get; set; }
        #endregion

        private AzureApiManagement _Management;
        public AzureApiManagement Management
        {
            get
            {

                if (_Management == null)
                {
                    _Management = new AzureApiManagement(host, apiId, apiKey);
                }

                return _Management;
            }
        }

        private AzureApiManagementApi _Apis;
        public AzureApiManagementApi Apis
        {
            get
            {
                if (_Apis == null)
                {
                    _Apis = new AzureApiManagementApi(host, apiId, apiKey);
                }

                return _Apis;
            }
        }

        private AzureApiManagementProduct _Products;
        public AzureApiManagementProduct Products
        {
            get
            {

                if (_Products == null)
                {
                    _Products = new AzureApiManagementProduct(host, apiId, apiKey);
                }

                return _Products;
            }
        }

        private AzureApiManagementUser _Users;
        public AzureApiManagementUser Users
        {
            get
            {

                if (_Users == null)
                {
                    _Users = new AzureApiManagementUser(host, apiId, apiKey);
                }

                return _Users;
            }
        }

        private AzureApiManagementAuthorizationServers _AuthorizationServers;
        public AzureApiManagementAuthorizationServers AuthorizationServers
        {
            get
            {

                if (_AuthorizationServers == null)
                {
                    _AuthorizationServers = new AzureApiManagementAuthorizationServers(host, apiId, apiKey);
                }

                return _AuthorizationServers;
            }
        }

        public AzureApiManagementServices(string _host,
            string _apiId,
            string _apiKey)
        {
            host = _host;
            apiId = _apiId;
            apiKey = _apiKey;
        }
    }
}
