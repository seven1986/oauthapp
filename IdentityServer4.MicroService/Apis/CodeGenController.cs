using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.CodeGenController;
using IdentityServer4.MicroService.Models.Apis.ApiResourceController;
using static IdentityServer4.MicroService.AppConstant;
using Microsoft.Azure.Cosmos.Table;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// 代码生成
    /// </summary>
    [Produces("application/json")]
    [ApiExplorerSettingsDynamic("CodeGen")]
    [SwaggerTag("代码生成")]
    public class CodeGenController : ApiControllerBase
    {
        #region Services
        readonly SwaggerCodeGenService swagerCodeGen;
        readonly INodeServices nodeServices;
        // azure Storage
        readonly AzureStorageService storageService;
        readonly IDistributedCache cache;

        //sql cache options
        readonly DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions() {
             AbsoluteExpiration = DateTimeOffset.MaxValue
        };
        #endregion

        #region 构造函数
        public CodeGenController(
            AzureStorageService _storageService,
            IStringLocalizer<CodeGenController> localizer,
            SwaggerCodeGenService _swagerCodeGen,
            INodeServices _nodeServices,
            //RedisService _redis,
            IDistributedCache _cache)
        {
            l = localizer;
            swagerCodeGen = _swagerCodeGen;
            nodeServices = _nodeServices;
            storageService = _storageService;
            //redis = _redis;
            cache = _cache;
        }
        #endregion

        #region 代码生成 - 客户端列表
        /// <summary>
        /// 代码生成 - 客户端列表
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        [HttpGet("Clients")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.get")]
        [SwaggerOperation(OperationId = "CodeGenClients",
            Summary = "代码生成 - 客户端列表",
            Description = "scope：isms.codegen.clients")]
        public ApiResult<List<SwaggerCodeGenItem>> Clients(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ClientItemsCache : swagerCodeGen.ClientItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }
        #endregion

        #region 代码生成 - 服务端列表
        /// <summary>
        /// 代码生成 - 服务端列表
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        [HttpGet("Servers")]
        [SwaggerOperation(OperationId = "CodeGenServers",
            Summary = "代码生成 - 服务端列表",
            Description = "scope：isms.codegen.servers")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.servers")]
        public ApiResult<List<SwaggerCodeGenItem>> Servers(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ServerItemsCache : swagerCodeGen.ServerItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }
        #endregion

        #region 代码生成 - NPM - 设置
        /// <summary>
        ///  代码生成 - NPM - 设置
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="language">语言</param>
        /// <returns></returns>
        [HttpGet("{id}/NpmOptions/{language}")]
        [SwaggerOperation(OperationId = "CodeGenNpmOptions",
            Summary = "代码生成 - NPM - 设置",
            Description = "scope：isms.codegen.npmoptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.npmoptions")]
        public async Task<ApiResult<CodeGenNpmOptionsModel>> NpmOptions(string id, Language language)
        {
            var result = await GetNpmOptions(language, id);

            var key = CodeGenControllerKeys.NpmOptions + id;

            var cacheResult = await cache.GetStringAsync(key); //redis.GetAsync(key);

            if (result != null)
            {
                return new ApiResult<CodeGenNpmOptionsModel>(result);
            }
            else
            {
                return new ApiResult<CodeGenNpmOptionsModel>(l, CodeGenControllerEnums.NpmOptions_GetOptionsFialed);
            }
        }
        async Task<CodeGenNpmOptionsModel> GetNpmOptions(Language lan, string id)
        {
            var key = CodeGenControllerKeys.NpmOptions + Enum.GetName(typeof(Language), lan) + ":" + id;
            
            var cacheResult = await cache.GetStringAsync(key);  //await redis.GetAsync(key);

            if (!string.IsNullOrWhiteSpace(cacheResult))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<CodeGenNpmOptionsModel>(cacheResult);

                    return result;
                }

                catch
                {
                    return null;
                }
            }

            return null;
        }
        #endregion

        #region 代码生成 - NPM - 更新设置
        /// <summary>
        ///  代码生成 - NPM - 更新设置
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="language">语言</param>
        /// <param name="value">package.json的内容字符串</param>
        /// <returns></returns>
        [HttpPut("{id}/NpmOptions/{language}")]
        [SwaggerOperation(OperationId = "CodeGenPutNpmOptions",
            Summary = "代码生成 - NPM - 更新设置",
            Description = "scope：isms.codegen.putnpmoptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.putnpmoptions")]
        public async Task<ApiResult<bool>> NpmOptions(string id, Language language, [FromBody]CodeGenNpmOptionsModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var result = await SetNpmOptions(id, language, value);

            return new ApiResult<bool>(result);
        }
        async Task<bool> SetNpmOptions(string id, Language lan, CodeGenNpmOptionsModel value)
        {
            var key = CodeGenControllerKeys.NpmOptions + Enum.GetName(typeof(Language), lan) + ":" + id;

            var cacheResult = JsonConvert.SerializeObject(value);

            await cache.SetStringAsync(key, cacheResult, cacheOptions); // redis.SetAsync(key, cacheResult, null);

            return true;
        }
        #endregion

        #region 代码生成 - Github - 设置
        /// <summary>
        ///  代码生成 - Github - 设置
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/GithubOptions")]
        [SwaggerOperation(OperationId = "CodeGenGithubOptions",
            Summary = "代码生成 - Github - 设置",
            Description = "scope：isms.codegen.githuboptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.githuboptions")]
        public async Task<ApiResult<ApiResourceGithubPublishRequest>> GithubOptions(string id)
        {
            var result = await GetGithubOptions(id);

            var key = CodeGenControllerKeys.GithubOptions + id;

            var cacheResult = await cache.GetStringAsync(key); //redis.GetAsync(key);

            if (result != null)
            {
                return new ApiResult<ApiResourceGithubPublishRequest>(result);
            }
            else
            {
                return new ApiResult<ApiResourceGithubPublishRequest>(l, CodeGenControllerEnums.GithubOptions_GetOptionsFialed);
            }
        }
        async Task<ApiResourceGithubPublishRequest> GetGithubOptions(string id)
        {
            var key = CodeGenControllerKeys.GithubOptions + id;

            var cacheResult = await cache.GetStringAsync(key);// redis.GetAsync(key);

            if (!string.IsNullOrWhiteSpace(cacheResult))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<ApiResourceGithubPublishRequest>(cacheResult);

                    return result;
                }

                catch
                {
                    return null;
                }
            }

            return null;
        }
        #endregion

        #region 代码生成 - Github - 更新设置
        /// <summary>
        ///  代码生成 - Github - 更新设置
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}/GithubOptions")]
        [SwaggerOperation(OperationId = "CodeGenPutGithubOptions",
            Summary = "代码生成 - Github - 更新设置",
            Description = "scope：isms.codegen.putgithuboptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.putgithuboptions")]
        public async Task<ApiResult<bool>> GithubOptions(string id, [FromBody]ApiResourceGithubPublishRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var result = await SetGithubOptions(id, value);

            return new ApiResult<bool>(result);
        }
        async Task<bool> SetGithubOptions(string id, ApiResourceGithubPublishRequest value)
        {
            var key = CodeGenControllerKeys.GithubOptions + id;

            var cacheResult = JsonConvert.SerializeObject(value);

            await cache.SetStringAsync(key, cacheResult, cacheOptions); // redis.SetAsync(key, cacheResult, null);

            return true;
        }
        #endregion

        #region 代码生成 - Github - 同步
        /// <summary>
        ///  代码生成 - Github - 同步
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpPut("{id}/SyncGithub")]
        [SwaggerOperation(OperationId = "CodeGenSyncGithub",
            Summary = "代码生成 - Github - 同步",
            Description = "scope：isms.codegen.putgithuboptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.syncgithub")]
        public async Task<ApiResult<bool>> SyncGithub(string id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var githubConfiguration = await GetGithubOptions(id);

            if (githubConfiguration != null && !string.IsNullOrWhiteSpace(githubConfiguration.token))
            {
                if (githubConfiguration.syncLabels)
                {
                    await storageService.AddMessageAsync("apiresource-publish-github",
                        JsonConvert.SerializeObject(githubConfiguration));
                }

                if (githubConfiguration.syncDocs)
                {
                    await storageService.AddMessageAsync("apiresource-publish-github-readthedocs",
                        JsonConvert.SerializeObject(githubConfiguration));
                }
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 代码生成 - 基本设置 - 获取
        /// <summary>
        ///  代码生成 - 基本设置 - 获取
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/CommonOptions")]
        [SwaggerOperation(OperationId = "CodeGenCommonOptions",
            Summary = "代码生成 - 基本设置 - 获取",
            Description = "scope：isms.codegen.commonoptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.commonoptions")]
        public async Task<ApiResult<CodeGenCommonOptionsModel>> CommonOptions(string id)
        {
            var result = await _CommonOptions(id);

            if (result != null)
            {
                return new ApiResult<CodeGenCommonOptionsModel>(result);
            }
            else
            {
                return new ApiResult<CodeGenCommonOptionsModel>(l, CodeGenControllerEnums.CommonOptions_GetOptionsFialed);
            }
        }
        async Task<CodeGenCommonOptionsModel> _CommonOptions(string id)
        {
            var key = CodeGenControllerKeys.CommonOptions + id;

            var cacheResult = await cache.GetStringAsync(key); //redis.GetAsync(key);

            if (!string.IsNullOrWhiteSpace(cacheResult))
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<CodeGenCommonOptionsModel>(cacheResult);

                    return result;
                }

                catch
                {
                    return null;
                }
            }

            return null;
        }
        #endregion

        #region 代码生成 - 基本设置 - 更新
        /// <summary>
        ///  代码生成 - 基本设置 - 更新
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value">package.json的内容字符串</param>
        /// <returns></returns>
        [HttpPut("{id}/CommonOptions")]
        [SwaggerOperation(OperationId = "CodeGenPutCommonOptions",
            Summary = "代码生成 - 基本设置 - 更新",
            Description = "scope：isms.codegen.putcommonoptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.putcommonoptions")]
        public async Task<ApiResult<bool>> PutCommonOptions(string id, [FromBody]CodeGenCommonOptionsModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var result = await _PutCommonOptions(id, value);

            return new ApiResult<bool>(result);
        }
        async Task<bool> _PutCommonOptions(string id, CodeGenCommonOptionsModel value)
        {
            var key = CodeGenControllerKeys.CommonOptions + id;

            var cacheResult = JsonConvert.SerializeObject(value);

            await cache.SetStringAsync(key, cacheResult, cacheOptions); //redis.SetAsync(key, cacheResult, null);

            return true;
        }
        #endregion

        #region 代码生成 - SDK - 发布
        /// <summary>
        /// 代码生成 - SDK - 发布
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("ReleaseSDK")]
        [SwaggerOperation(OperationId = "CodeGenReleaseSDK",
            Summary = "代码生成 - SDK - 发布",
            Description = "scope：isms.codegen.releasesdk")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:codegen.releasesdk")]
        public async Task<ApiResult<bool>> ReleaseSDK([FromBody]GenerateClientRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var platformPath = $"./Node/sdkgen/{Enum.GetName(typeof(PackagePlatform), value.platform)}/";

            try
            {
                var swaggerDoc = string.Empty;

                using (var hc = new HttpClient())
                {
                    swaggerDoc = await hc.GetStringAsync(value.swaggerUrl);
                }

                if (string.IsNullOrWhiteSpace(swaggerDoc))
                {
                    return new ApiResult<bool>(l, CodeGenControllerEnums.GenerateClient_GetSwaggerFialed);
                }

                var templateDirectory = platformPath + Enum.GetName(typeof(Language), value.language) + "_" + DateTime.Now.Ticks.ToString();

                if (!Directory.Exists(templateDirectory))
                {
                    Directory.CreateDirectory(templateDirectory);
                }

                #region 重置swagger.json >> info >> title节点
                // 获取的文档中，info-title节点是网关上的API名称，
                // 生成的class时，命名一般都是英文的
                var options = await GetNpmOptions(value.language, value.apiId);
                if (options != null && !string.IsNullOrWhiteSpace(options.sdkName))
                {
                    try
                    {
                        var swaggerDocJson = JsonConvert.DeserializeObject<JObject>(swaggerDoc);
                        swaggerDocJson["info"]["title"] = options.sdkName;
                        swaggerDoc = swaggerDocJson.ToString();
                    }
                    catch
                    {

                    }
                }
                #endregion

                var SdkCode = await nodeServices.InvokeAsync<string>(platformPath + value.language,
                    swaggerDoc, value.apiId);

                switch (value.platform)
                {
                    case PackagePlatform.npm:

                        var PublishResult = ReleasePackage_NPM(templateDirectory, value.language, SdkCode, value.apiId, value.swaggerUrl, value.tags);

                        break;

                    case PackagePlatform.nuget:

                        break;

                    case PackagePlatform.spm:

                        break;

                    case PackagePlatform.gradle:

                        break;

                    default:
                        break;
                }

                return new ApiResult<bool>(true);
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.ExpectationFailed,
                    ex.Message);
            }
        }
        async Task<bool> ReleasePackage_NPM(string templateDirectory, Language lan, string SdkCode, string apiId,string swaggerUrl,string tags="")
        {
            #region 写SDK文件
            var fileName = string.Empty;

            switch (lan)
            {
                case Language.angular2:
                    fileName = templateDirectory + "/index.ts";
                    break;

                case Language.jQuery:
                    fileName = templateDirectory + "/index.js";
                    break;

                default:
                    break;
            }

            using (var sw = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                sw.WriteLine(SdkCode);
            }
            #endregion

            #region 更新包信息
            var configFilePath = templateDirectory + "/package.json";

            var options = await GetNpmOptions(lan, apiId);

            var JsonDoc = new JObject();

            if (!string.IsNullOrWhiteSpace(options.name))
            {
                JsonDoc["name"] = options.name;
            }

            if (!string.IsNullOrWhiteSpace(options.homepage))
            {
                JsonDoc["homepage"] = options.homepage;
            }

            if (!string.IsNullOrWhiteSpace(options.author))
            {
                JsonDoc["author"] = options.author;
            }

            if (options.keywords != null && options.keywords.Length > 0)
            {
                JsonDoc["keywords"] = JToken.FromObject(options.keywords);
            }

            if (!string.IsNullOrWhiteSpace(options.description))
            {
                JsonDoc["description"] = options.description;
            }

            if (string.IsNullOrWhiteSpace(options.version)) { options.version = "0.0.0"; }

            if (!string.IsNullOrWhiteSpace(options.license))
            {
                JsonDoc["license"] = options.license;
            }

            #region version

            var CurrentVersion = Version.Parse(options.version);

            var newVersion = string.Empty;

            if (CurrentVersion.Build + 1 < int.MaxValue)
            {
                newVersion = $"{CurrentVersion.Major}.{CurrentVersion.Minor}.{CurrentVersion.Build + 1}";
            }

            else if (CurrentVersion.Minor + 1 < int.MaxValue)
            {
                newVersion = $"{CurrentVersion.Major}.{CurrentVersion.Minor + 1}.{CurrentVersion.Build}";
            }

            else if (CurrentVersion.Major + 1 < int.MaxValue)
            {
                newVersion = $"{CurrentVersion.Major + 1}.{CurrentVersion.Minor}.{CurrentVersion.Build}";
            }

            JsonDoc["version"] = newVersion;
            #endregion

            using (var sw = new StreamWriter(configFilePath, false, Encoding.UTF8))
            {
                await sw.WriteLineAsync(JsonDoc.ToString());
            }
            #endregion

            var npmrcFilePath = templateDirectory + "/.npmrc";
            if (!string.IsNullOrWhiteSpace(options.token))
            {
                using (var sw = new StreamWriter(npmrcFilePath, false, Encoding.UTF8))
                {
                    await sw.WriteLineAsync("//registry.npmjs.org/:_authToken=" + options.token);
                }
            }

            var readmeFilePath = templateDirectory + "/README.md";
            if (!string.IsNullOrWhiteSpace(options.README))
            {
                var releaseREAME = options.README.Replace("<%SdkCode%>", SdkCode);

                using (var sw = new StreamWriter(readmeFilePath, false, Encoding.UTF8))
                {
                    await sw.WriteLineAsync(releaseREAME);
                }
            }

            #region 打包SDK文件为.zip
            var releaseDirectory = templateDirectory + "_release/";

            if (!Directory.Exists(releaseDirectory))
            {
                Directory.CreateDirectory(releaseDirectory);
            }

            var templateName = Directory.GetParent(templateDirectory).Name;
            var languageName = Path.GetFileName(templateDirectory);
            var packageFileName = $"{templateName}.{languageName}_{newVersion}.zip";

            ZipFile.CreateFromDirectory(templateDirectory, releaseDirectory + packageFileName);
            #endregion

            #region 上传.zip，发消息到发包队列
            var blobUrl = string.Empty;

            using (var zipFileStream = new FileStream(releaseDirectory + packageFileName, FileMode.Open, FileAccess.Read))
            {
                blobUrl = await storageService.UploadBlobAsync(zipFileStream, "codegen-npm", packageFileName);
            }

            await storageService.AddMessageAsync("publish-package-npm", blobUrl);
            #endregion

            #region 清理本地文件
            try
            {
                Directory.Delete(releaseDirectory, true);
                Directory.Delete(templateDirectory, true);
            }
            catch { }
            #endregion

            options.version = newVersion;

            await SetNpmOptions(apiId, lan, options);

            await PostHistory(apiId, new CodeGenHistoryRequest()
            {
                language = Enum.GetName(typeof(Language), lan),
                releaseDate = DateTime.UtcNow.ToString(),
                sdkName = options.name,
                swaggerUrl = swaggerUrl,
                tags = tags,
                version = newVersion
            });

            return true;
        }
        #endregion

        #region 代码生成 - SDK - 预览生成代码
        /// <summary>
        /// 代码生成 - SDK - 预览生成代码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Gen")]
        [SwaggerOperation(OperationId = "CodeGenGen",
            Summary = "代码生成 - SDK - 预览生成代码",
            Description = "")]
        [AllowAnonymous]
        //[Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.CodeGenGen)]
        public async Task<ApiResult<string>> Gen([FromBody]CodeGenGenRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            try
            {
                var swaggerDoc = string.Empty;

                using (var hc = new HttpClient())
                {
                    swaggerDoc = await hc.GetStringAsync(value.swaggerUrl);
                }

                if (string.IsNullOrWhiteSpace(swaggerDoc))
                {
                    return new ApiResult<string>(l, CodeGenControllerEnums.GenerateClient_GetSwaggerFialed);
                }

                var platformPath = $"./Node/";

                var result = await nodeServices.InvokeAsync<string>(platformPath + value.genName,
                   swaggerDoc, new { value.swaggerUrl });

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l,
                    BasicControllerEnums.ExpectationFailed,
                    ex.Message);
            }
        }
        #endregion

        #region 代码生成 - SDK - 发布记录
        /// <summary>
        /// 代码生成 - SDK - 发布记录
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/History")]
        [SwaggerOperation(OperationId = "CodeGenHistory",
            Summary = "代码生成 - SDK - 发布记录",
            Description = "scope：isms.apiresource.history")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.history")]
        public async Task<PagingResult<CodeGenHistoryEntity>> History(string id)
        {
            var tb = await storageService.CreateTableAsync("CodeGenHistories");

            var query = new TableQuery<CodeGenHistoryEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));

            var result = await storageService.ExecuteQueryAsync(tb, query);

            return new PagingResult<CodeGenHistoryEntity>()
            {
                data = result,
                skip = 0,
                take = result.Count,
                total = result.Count
            };
        }
        #endregion

        #region 代码生成 - SDK - 添加记录
        /// <summary>
        /// 代码生成 - SDK - 添加记录
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/PostHistory")]
        [SwaggerOperation(OperationId = "CodeGenPostHistory",
            Summary = "代码生成 - SDK - 添加记录",
            Description = "")]
        [AllowAnonymous]
        public async Task<ApiResult<bool>> PostHistory(string id, [FromBody]CodeGenHistoryRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var tb = await storageService.CreateTableAsync("CodeGenHistories");

            try
            {
                var Entity = new CodeGenHistoryEntity(id.ToString(), value.sdkName + "@" + value.language + "@" + value.version)
                {
                    language = value.language,
                    releaseDate = value.releaseDate,
                    swaggerUrl = value.swaggerUrl,
                    tags = value.tags,
                    version = value.version
                };

                var result = await storageService.TableInsertAsync(tb, Entity);

                if (result.FirstOrDefault().Result != null)
                {
                    return new ApiResult<bool>(true);
                }

                else
                {
                    return new ApiResult<bool>(l, CodeGenControllerEnums.History_PostFailed);
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, CodeGenControllerEnums.History_PostFailed, ex.Message);
            }
        }
        #endregion
    }
}
