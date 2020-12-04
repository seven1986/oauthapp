using JavaScriptEngineSwitcher.V8;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OAuthApp.Data;
using OAuthApp.Enums;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.PackageController;
using OAuthApp.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// 软件包
    /// </summary>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Package")]
    [SwaggerTag("#### 软件包管理")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PackageController : ApiControllerBase
    {
       private readonly V8JsEngine engine = new V8JsEngine(new V8Settings
        {
            MaxNewSpaceSize = 4,
            MaxOldSpaceSize = 8
        });

        #region Services
        readonly AzureStorageService storageService;
        readonly SdkDbContext sdkDB;
        #endregion

        #region 构造函数
        public PackageController(
            AzureStorageService _storageService, 
            SdkDbContext _sdkDB,
            IStringLocalizer<PackageController> localizer)
        {
            storageService = _storageService;
            sdkDB = _sdkDB;
            l = localizer;
        }
        #endregion

        #region 软件包 - 列表
        /// <summary>
        /// 软件包 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.get")]
        [SwaggerOperation(
            OperationId = "PackageGet",
            Summary = "软件包 - 列表",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.get | oauthapp.package.get |")]
        public async Task<PagingResult<SdkPackage>> Get([FromQuery] PagingRequest<PackageGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<SdkPackage>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = sdkDB.Packages.AsQueryable();

                query = query.Where(x => x.UserID == UserId);

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.name))
            {
                query = query.Where(x => x.Name.Equals(value.q.name));
            }

            if (value.q.expandGenerators)
            {
                query = query.Include(x => x.SdkGenerators);
            }
            #endregion

            #region total
            var result = new PagingResult<SdkPackage>()
            {
                skip = value.skip.Value,
                take = value.take.Value,
                total = await query.CountAsync()
            };
            #endregion

            if (result.total > 0)
            {
                #region orderby
                if (!string.IsNullOrWhiteSpace(value.orderby))
                {
                    if (value.asc.Value)
                    {
                        query = query.OrderBy(value.orderby);
                    }
                    else
                    {
                        query = query.OrderByDescending(value.orderby);
                    }
                }
                #endregion

                #region pagingWithData
                var data = await query.Skip(value.skip.Value).Take(value.take.Value)
                    .ToListAsync();
                #endregion

                result.data = data;
            }

            return result;
        }
        #endregion

        #region 软件包 - 详情
        /// <summary>
        /// 软件包 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.detail")]
        [SwaggerOperation(OperationId = "PackageDetail",
            Summary = "软件包 - 详情",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.detail | oauthapp.package.detail |")]
        public async Task<ApiResult<SdkPackage>> Get(long id)
        {
            var query = sdkDB.Packages.AsQueryable();

            var entity = await query
                .Where(x => x.Id == id && x.UserID == UserId)
                .Include(x => x.SdkGenerators)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<SdkPackage>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<SdkPackage>(entity);
        }
        #endregion

        #region 软件包 - 创建
        /// <summary>
        /// 软件包 - 创建
        /// </summary>
        /// <param name="value">ID</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.post")]
        [SwaggerOperation(
            OperationId = "PackagePost",
            Summary = "软件包 - 创建",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.post | oauthapp.package.post |")]
        public ApiResult<long> Post([FromBody] SdkPackage value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            value.UserID = UserId;

            sdkDB.Add(value);

            try
            {
                sdkDB.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<long>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = 0
                };
            }

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region 软件包 - 更新
        /// <summary>
        /// 软件包 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.put")]
        [SwaggerOperation(
            OperationId = "PackagePut",
            Summary = "软件包 - 更新",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.put | oauthapp.package.put |")]
        public ApiResult<bool> Put([FromBody] SdkPackage value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var Entity = sdkDB.Packages.Where(x => x.Id == value.Id && x.UserID == UserId)
                .Include(x => x.SdkGenerators)
                .FirstOrDefault();

            if (Entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            if (!string.IsNullOrWhiteSpace(value.Name) &&
               !value.Name.Equals(Entity.Name))
            {
                Entity.Name = value.Name;
            }

            if (!string.IsNullOrWhiteSpace(value.PackageName) &&
               !value.PackageName.Equals(Entity.PackageName))
            {
                Entity.PackageName = value.PackageName;
            }

            if (!string.IsNullOrWhiteSpace(value.Description) &&
              !value.Description.Equals(Entity.Description))
            {
                Entity.Description = value.Description;
            }

            if (!string.IsNullOrWhiteSpace(value.Summary) &&
              !value.Description.Equals(Entity.Summary))
            {
                Entity.Summary = value.Summary;
            }

            if (!string.IsNullOrWhiteSpace(value.WebSite) &&
              !value.WebSite.Equals(Entity.WebSite))
            {
                Entity.WebSite = value.WebSite;
            }

            if (!string.IsNullOrWhiteSpace(value.LogoUri) &&
              !value.LogoUri.Equals(Entity.LogoUri))
            {
                Entity.LogoUri = value.LogoUri;
            }

            if (!string.IsNullOrWhiteSpace(value.Tags) &&
              !value.Tags.Equals(Entity.Tags))
            {
                Entity.Tags = value.Tags;
            }

            Entity.Enable = value.Enable;

            #region Templates
            if (Entity.SdkGenerators != null && Entity.SdkGenerators.Count > 0)
            {
                Entity.SdkGenerators.Clear();
            }
            if (value.SdkGenerators != null && value.SdkGenerators.Count > 0)
            {
                Entity.SdkGenerators = value.SdkGenerators
                   .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                   .Select(x => new SdkGenerator()
                   {
                       Description = x.Description,
                       Name = x.Name,
                       Uri = x.Uri,
                       CompiledCode = "",
                       Enable = x.Enable,
                       SdkPackageId = value.Id
                   }).ToList();
            }
            #endregion

            try
            {
                sdkDB.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = false
                };
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 软件包 - 删除
        /// <summary>
        /// 软件包 - 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.delete")]
        [SwaggerOperation(
            OperationId = "PackageDelete",
            Summary = "软件包 - 删除",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.delete | oauthapp.package.delete |")]
        public ApiResult<bool> Delete(long id)
        {
            var entity = sdkDB.Packages.Where(x => x.Id == id && x.UserID == UserId)
                .Include(x => x.SdkGenerators)
                .FirstOrDefault();

            if (entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            sdkDB.Packages.Remove(entity);

            try
            {
                sdkDB.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = false
                };
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 软件包 - 发布记录
        /// <summary>
        /// 软件包 - 发布记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("{id}/ReleaseHistory")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.releasehistory")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.releasehistory")]
        [SwaggerOperation(
            OperationId = "PackageReleaseHistory",
            Summary = "软件包 - 发布记录",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.releasehistory | oauthapp.package.releasehistory |")]
        public async Task<PagingResult<SdkReleaseHistory>> ReleaseHistory([FromRoute]long id, [FromQuery] PagingRequest<ReleaseHistoryGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<SdkReleaseHistory>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = sdkDB.ReleaseHistories.Where(x => x.SdkPackageId == id && x.UserID == UserId).AsQueryable();

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.remark))
            {
                query = query.Where(x => x.Remark.Contains(value.q.remark));
            }
            if (!string.IsNullOrWhiteSpace(value.q.tags))
            {
                query = query.Where(x => x.Tags.Contains(value.q.tags));
            }
            #endregion

            #region total
            var result = new PagingResult<SdkReleaseHistory>()
            {
                skip = value.skip.Value,
                take = value.take.Value,
                total = await query.CountAsync()
            };
            #endregion

            if (result.total > 0)
            {
                #region orderby
                if (!string.IsNullOrWhiteSpace(value.orderby))
                {
                    if (value.asc.Value)
                    {
                        query = query.OrderBy(value.orderby);
                    }
                    else
                    {
                        query = query.OrderByDescending(value.orderby);
                    }
                }
                #endregion

                #region pagingWithData
                var data = await query.Skip(value.skip.Value).Take(value.take.Value)
                    .ToListAsync();
                #endregion

                result.data = data;
            }

            return result;
        }
        #endregion

        #region 软件包 - 发布
        /// <summary>
        /// 软件包 - 发布
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.publish")]
        [SwaggerOperation(
            OperationId = "PackagePublish",
            Summary = "软件包 - 发布",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.package.publish | oauthapp.package.publish |")]
        public ApiResult<bool> Publish([FromBody]PublishRequest value)
        {
            var entity = sdkDB.Packages.Where(x => x.Id == value.id && x.UserID == UserId)
                .Include(x => x.SdkGenerators).FirstOrDefault();

            if (entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            #region version
            Version ReleaseVersion = new Version(1, 0, 0);
            if (!string.IsNullOrWhiteSpace(value.Version))
            {
                ReleaseVersion = Version.Parse(value.Version);
            }
            engine.RemoveVariable("packageVersion");
            engine.SetVariableValue("packageVersion", ReleaseVersion.ToString());
            #endregion

            var SdkRootPath = $"./sdk/{value.id}/"+ DateTime.Now.Ticks.ToString();

            var SdkBuildPath = $"{SdkRootPath}/build";

            if (!Directory.Exists(SdkBuildPath))
            {
                Directory.CreateDirectory(SdkBuildPath);
            }

            #region 生成软件包
            var Codes = BuildPackage(entity);

            foreach (var t in Codes)
            {
                using (var sw = new StreamWriter($"{SdkBuildPath}/{t.Key}", false, Encoding.UTF8))
                {
                    sw.WriteLine(t.Value);
                }
            }
            #endregion

            #region 打包软件包
            var SdkReleasePath = $"{SdkRootPath}/release";
            if (!Directory.Exists(SdkReleasePath))
            {
                Directory.CreateDirectory(SdkReleasePath);
            }

            var SdkRootName = Directory.GetParent(SdkRootPath).Name;
            var PackagePath = $"{SdkReleasePath}/{SdkRootName}.zip";
            ZipFile.CreateFromDirectory(SdkBuildPath, PackagePath);
            #endregion

            #region 上传软件包、加入发布计划
            var blobUrl = string.Empty;

            using (var zipFileStream = new FileStream(PackagePath, FileMode.Open, FileAccess.Read))
            {
                blobUrl = storageService.UploadBlobAsync(zipFileStream, "codegen-npm", entity.PackageName).Result;
            }

            var queueResult = storageService.AddMessageAsync("publish-package-npm", blobUrl).Result;
            #endregion

            #region 清理本地文件
            //try
            //{
            //    Directory.Delete(SdkRootPath, true);
            //}
            //catch { }
            #endregion

            #region 记录发布历史
            sdkDB.ReleaseHistories.Add(new SdkReleaseHistory()
            {
                Description = value.Description,
                SdkPackageId = value.id,
                ReleaseDate = DateTime.UtcNow.AddHours(8).ToString("G"),
                Remark = value.Remark,
                Tags = value.Tags,
                UserID = UserId,
                Version = ReleaseVersion.ToString()
            });
            #endregion

            try
            {
                sdkDB.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = false
                };
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        private Dictionary<string,string> BuildPackage(SdkPackage item)
        {
            var result = new Dictionary<string, string>();

            var swaggerDocument = string.Empty;

            using (var hc = new HttpClient()) 
            {
                swaggerDocument = hc.GetStringAsync(item.SwaggerUri).Result;
            }

            if(string.IsNullOrWhiteSpace(swaggerDocument))
            {
                return result;
            }
            
            engine.RemoveVariable("packageObject");
            engine.SetVariableValue("packageObject", swaggerDocument);

            if (item.SdkGenerators != null && item.SdkGenerators.Count > 0)
            {
                item.SdkGenerators.ForEach(g =>
                {
                    if (!g.Enable) { return; }

                    try
                    {
                        var templateSource = string.Empty;

                        using (var hc = new HttpClient())
                        {
                            templateSource = hc.GetStringAsync(g.Uri).Result;
                        }

                        engine.Execute(templateSource);

                        var CompiledCode = engine.CallFunction<string>("codeCompile");

                        if (!result.ContainsKey(g.Name))
                        {
                            result.Add(g.Name, CompiledCode);
                        }
                    }
                    catch
                    {

                    }
                });
            }

            return result;
        }

        #region 软件包 - 预编译
        /// <summary>
        /// 软件包 - 预编译
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("PreCompile")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:package.precompile")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:package.precompile")]
        [SwaggerOperation(
            OperationId = "PackagePreCompile",
            Summary = "软件包 - 预编译",
            Description = "scope&permission：oauthapp.package.precompile")]
        public ApiResult<string> PreCompile([FromBody] PreCompileRequest value)
        {
            try
            {
                var swaggerDocument = string.Empty;

                using (var hc = new HttpClient())
                {
                    swaggerDocument = hc.GetStringAsync(value.swaggerUri).Result;
                }

                engine.RemoveVariable("packageObject");
                engine.SetVariableValue("packageObject", swaggerDocument);

                if (!string.IsNullOrWhiteSpace(value.packageVersion))
                {
                    engine.RemoveVariable("packageVersion");
                    engine.SetVariableValue("packageVersion", value.packageVersion);
                }

                var templateSource = string.Empty;

                using (var hc = new HttpClient())
                {
                    templateSource = hc.GetStringAsync(value.scriptUri).Result;
                }

                engine.Execute(templateSource);

                var result = engine.CallFunction<string>("codeCompile");

                return new ApiResult<string>(result);
            }
            catch(Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.HasError, ex.Message);
            }
        }
        #endregion
    }
}
