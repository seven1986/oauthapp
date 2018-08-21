using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Mappers;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.ApiResourceController;
using IdentityServer4.MicroService.Models.Apis.CodeGenController;
using static IdentityServer4.MicroService.AppConstant;
using static IdentityServer4.MicroService.MicroserviceConfig;
using static IdentityServer4.MicroService.AppDefaultData;

namespace IdentityServer4.MicroService.Apis
{
    // ApiResource 根据 userId 来获取列表、或详情、增删改

    /// <summary>
    /// 微服务
    /// </summary>
    /// <remarks>为微服务提供版本管理、网关集成都功能。</remarks>
    [Route("ApiResource")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class ApiResourceController : BasicController
    {
        #region Services
        //Database
        readonly ConfigurationDbContext configDb;
        readonly SwaggerCodeGenService swagerCodeGen;
        readonly AzureStorageService storageService;
        readonly EmailService email;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_configDb"></param>
        /// <param name="_userDb"></param>
        /// <param name="localizer"></param>
        /// <param name="_tenantService"></param>
        /// <param name="_tenantDb"></param>
        /// <param name="_redis"></param>
        /// <param name="_swagerCodeGen"></param>
        /// <param name="_storageService"></param>
        /// <param name="_email"></param>
        /// <param name="_provider"></param>
        public ApiResourceController(
            ConfigurationDbContext _configDb,
            IdentityDbContext _userDb,
            IStringLocalizer<ApiResourceController> localizer,
            TenantService _tenantService,
            TenantDbContext _tenantDb,
            RedisService _redis,
            SwaggerCodeGenService _swagerCodeGen,
            AzureStorageService _storageService,
            EmailService _email,
            IDataProtectionProvider _provider)
        {
            configDb = _configDb;
            db = _userDb;
            l = localizer;
            tenantDb = _tenantDb;
            tenantService = _tenantService;
            redis = _redis;
            swagerCodeGen = _swagerCodeGen;
            storageService = _storageService;
            email = _email;
            protector = _provider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();
        }
        #endregion

        #region 微服务
        #region 微服务 - 列表
        /// <summary>
        /// 微服务 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.get</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.get</code>
        /// </remarks>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceGet)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceGet)]
        [SwaggerOperation("ApiResource/Get")]
        public async Task<PagingResult<ApiResource>> Get(PagingRequest<ApiResourceGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<ApiResource>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = configDb.ApiResources.AsQueryable();

            var ApiResourceIds = await db.UserApiResources.Where(x => x.UserId == UserId)
              .Select(x => x.ApiResourceId).ToListAsync();

            if (ApiResourceIds.Count > 0)
            {
                query = query.Where(x => ApiResourceIds.Contains(x.Id));
            }

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.name))
            {
                query = query.Where(x => x.Name.Equals(value.q.name));
            }

            if (value.q.expandScopes)
            {
                query = query.Include(x => x.Scopes);
            }

            if (value.q.expandClaims)
            {
                query = query.Include(x => x.UserClaims);
            }
            #endregion

            #region total
            var result = new PagingResult<ApiResource>()
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

        #region 微服务 - 详情
        /// <summary>
        /// 微服务 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.detail</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.detail</code>
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDetail)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDetail)]
        [SwaggerOperation("ApiResource/Detail")]
        public async Task<ApiResult<ApiResource>> Get(long id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResource>(l, BasicControllerEnums.NotFound);
            }

            var query = configDb.ApiResources.AsQueryable();

            var entity = await query
                .Where(x => x.Id == id)
                .Include(x => x.Scopes).ThenInclude(x => x.UserClaims)
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<ApiResource>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<ApiResource>(entity);
        }
        #endregion

        #region 微服务 - 创建
        /// <summary>
        /// 微服务 - 创建
        /// </summary>
        /// <param name="value">ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.post</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.post</code>
        /// </remarks>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePost)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePost)]
        [SwaggerOperation("ApiResource/Post")]
        public async Task<ApiResult<long>> Post([FromBody]ApiResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            configDb.Add(value);

            await configDb.SaveChangesAsync();

            db.UserApiResources.Add(new AspNetUserApiResource()
            {
                ApiResourceId = value.Id,
                UserId = UserId
            });

            await db.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region 微服务 - 更新
        /// <summary>
        /// 微服务 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.put</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.put</code>
        /// </remarks>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePut)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePut)]
        [SwaggerOperation("ApiResource/Put")]
        public async Task<ApiResult<long>> Put([FromBody]ApiResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.Id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            using (var tran = configDb.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    configDb.Update(value);
                    configDb.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await configDb.ApiResources.Where(x => x.Id == value.Id)
                                     .Include(x => x.Scopes).ThenInclude(x => x.UserClaims)
                                     .Include(x => x.Secrets)
                                     .Include(x => x.UserClaims)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync();
                    #endregion

                    #region Update Entity.Claims
                    if (value.UserClaims != null && value.UserClaims.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.UserClaims.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.UserClaims.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE ApiClaims WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                configDb.Database.ExecuteSqlCommand($"DELETE ApiClaims WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.UserClaims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                configDb.Database.ExecuteSqlCommand($"UPDATE ApiClaims SET [Type]={x.Type} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.UserClaims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                configDb.Database.ExecuteSqlCommand($"INSERT INTO ApiClaims VALUES ({source.Id},{x.Type})");
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Secrets
                    if (value.Secrets != null && value.Secrets.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Secrets.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Secrets.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE ApiSecrets WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                configDb.Database.ExecuteSqlCommand($"DELETE ApiSecrets WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Secrets.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                //var _params = new SqlParameter[] {
                                //  new SqlParameter("@Description", DBNull.Value) { IsNullable = true },
                                //  new SqlParameter("@Expiration", DBNull.Value) { IsNullable = true },
                                //  new SqlParameter("@Type",  DBNull.Value) { IsNullable = true },
                                //  new SqlParameter("@Value",  DBNull.Value) { IsNullable = true },
                                //};

                                //if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                //if (x.Expiration.HasValue) { _params[1].Value = x.Expiration; }
                                //if (!string.IsNullOrWhiteSpace(x.Type)) { _params[2].Value = x.Type; }
                                //if (!string.IsNullOrWhiteSpace(x.Value)) { _params[3].Value = x.Value; }

                                //var sql = new RawSqlString("UPDATE ApiSecrets SET [Description]=@Description,[Expiration]=@Expiration,[Type]=@Type,[Value]=@Value WHERE Id = " + x.Id);

                                configDb.Database.ExecuteSqlCommand($"UPDATE ApiSecrets SET [Description]={x.Description},[Expiration]={x.Expiration},[Type]={x.Type},[Value]={x.Value} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Secrets.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                //var _params = new SqlParameter[] {
                                //   new SqlParameter("@ApiResourceId", source.Id),
                                //   new SqlParameter("@Description", DBNull.Value) { IsNullable = true },
                                //   new SqlParameter("@Expiration", DBNull.Value) { IsNullable = true },
                                //   new SqlParameter("@Type", DBNull.Value){ IsNullable = true },
                                //   new SqlParameter("@Value", DBNull.Value){ IsNullable = true },
                                //};

                                //if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                //if (x.Expiration.HasValue) { _params[1].Value = x.Expiration; }
                                //if (!string.IsNullOrWhiteSpace(x.Type)) { _params[2].Value = x.Type; }
                                //if (!string.IsNullOrWhiteSpace(x.Value)) { _params[3].Value = x.Value; }

                                //var sql = new RawSqlString("INSERT INTO ApiSecrets VALUES (@ApiResourceId,@Description,@Expiration,@Type,@Value)");

                                configDb.Database.ExecuteSqlCommand($"INSERT INTO ApiSecrets VALUES ({source.Id},{x.Description},{x.Expiration},{x.Type},{x.Value})");
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Scopes
                    if (value.Scopes != null && value.Scopes.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Scopes.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Scopes.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE ApiScopeClaims WHERE ApiScopeId IN ({0})",
                                //           string.Join(",", DeleteEntities));

                                configDb.Database.ExecuteSqlCommand($"DELETE ApiScopeClaims WHERE ApiScopeId IN ({string.Join(",", DeleteEntities)})");

                                //sql = string.Format("DELETE ApiScopes WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                configDb.Database.ExecuteSqlCommand($"DELETE ApiScopes WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Scopes.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                //var _params = new SqlParameter[] {
                                //  new SqlParameter("@Description", DBNull.Value) { IsNullable = true },
                                //  new SqlParameter("@DisplayName", DBNull.Value) { IsNullable = true },
                                //  new SqlParameter("@Emphasize", x.Emphasize),
                                //  new SqlParameter("@Name", x.Name),
                                //  new SqlParameter("@Required", x.Required),
                                //  new SqlParameter("@ShowInDiscoveryDocument", x.ShowInDiscoveryDocument)
                                //};

                                //if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                //if (!string.IsNullOrWhiteSpace(x.DisplayName)) { _params[1].Value = x.DisplayName; }

                                //var sql = new RawSqlString("UPDATE ApiScopes SET [Description]=@Description,[DisplayName]=@DisplayName,[Emphasize]=@Emphasize,[Name]=@Name,[Required]=@Required,[ShowInDiscoveryDocument]=@ShowInDiscoveryDocument WHERE Id = " + x.Id);

                                configDb.Database.ExecuteSqlCommand($"UPDATE ApiScopes SET [Description]={x.Description},[DisplayName]={x.DisplayName},[Emphasize]={x.Emphasize},[Name]={x.Name},[Required]={x.Required},[ShowInDiscoveryDocument]={x.ShowInDiscoveryDocument} WHERE Id = {x.Id}");

                                configDb.Database.ExecuteSqlCommand($"DELETE ApiScopeClaims WHERE ApiScopeId = {x.Id}");

                                x.UserClaims.ForEach(claim =>
                                {
                                    configDb.Database.ExecuteSqlCommand($"INSERT INTO ApiScopeClaims VALUES ({x.Id},{claim.Type})");
                                });
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Scopes.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                var _params = new SqlParameter[]
                                {
                                  new SqlParameter("@Description",DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@DisplayName",DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@Emphasize",x.Emphasize),
                                  new SqlParameter("@Name", x.Name),
                                  new SqlParameter("@Required", x.Required),
                                  new SqlParameter("@ShowInDiscoveryDocument", x.ShowInDiscoveryDocument),
                                  new SqlParameter() { Direction = ParameterDirection.ReturnValue },
                                };

                                if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                if (!string.IsNullOrWhiteSpace(x.DisplayName)) { _params[1].Value = x.DisplayName; }

                                var sql = new RawSqlString("INSERT INTO ApiScopes VALUES (@ApiResourceId,@Description,@DisplayName,@Emphasize,@Name,@Required,@ShowInDiscoveryDocument)\r\n" +
                                  "SELECT @@identity");

                                configDb.Database.ExecuteSqlCommand(sql, _params);

                                if (_params[_params.Length - 1].Value != null)
                                {
                                    var _ApiScopeId = long.Parse(_params[_params.Length - 1].Value.ToString());

                                    x.UserClaims.ForEach(claim =>
                                    {
                                        configDb.Database.ExecuteSqlCommand(
                                        new RawSqlString("INSERT INTO ApiScopeClaims VALUES (@ApiScopeId,@Type)"),
                                        new SqlParameter("@ApiScopeId", _ApiScopeId),
                                        new SqlParameter("@Type", claim.Type));
                                    });
                                }
                            });
                        }
                        #endregion
                    }
                    #endregion

                    tran.Commit();
                }

                catch (Exception ex)
                {
                    tran.Rollback();

                    return new ApiResult<long>(l,
                        BasicControllerEnums.ExpectationFailed,
                        ex.Message);
                }
            }

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region 微服务 - 删除
        /// <summary>
        /// 微服务 - 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.delete</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.delete</code>
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDelete)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDelete)]
        [SwaggerOperation("ApiResource/Delete")]
        public async Task<ApiResult<long>> Delete(long id)
        {
            if (!await exists(id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            var entity = await configDb.ApiResources.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            configDb.ApiResources.Remove(entity);

            await db.SaveChangesAsync();

            return new ApiResult<long>(id);
        }
        #endregion

        #region 微服务 - 权限代码
        /// <summary>
        /// 微服务 - 权限代码
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.scopes</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.scopes</code>
        /// </remarks>
        [HttpGet("Scopes")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDelete)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDelete)]
        [SwaggerOperation("ApiResource/Scopes")]
        public async Task<ApiResult<Dictionary<string, List<ApiResourceScopeResponse>>>> Scopes()
        {
            var entities = new List<ApiResourceScopeEntity>();

            var cmd = @"SELECT 
            B.[Name] as API, 
            A.[Name] as Code,
            A.DisplayName as [Name],
            A.[Description],
            A.Emphasize 
            FROM [dbo].[ApiScopes] A
            INNER JOIN ApiResources B ON A.ApiResourceId = B.Id
            WHERE ShowInDiscoveryDocument = 1";

            using (var r = await db.ExecuteReaderAsync(cmd))
            {
                while (r.Read())
                {
                    var item = new ApiResourceScopeEntity()
                    {
                        Api = r["Api"].ToString(),
                        Code = r["Code"].ToString(),
                        Description = r["Description"].ToString(),
                        Emphasize = bool.Parse(r["Emphasize"].ToString()),
                        Name = r["Name"].ToString()
                    };

                    entities.Add(item);
                }
            }

            var result = entities.GroupBy(x => x.Api).ToDictionary(
                k => k.Key,
                v => v.Select(x => new ApiResourceScopeResponse()
                {
                    Code = x.Code,
                    Description = x.Description,
                    Emphasize = x.Emphasize,
                    Name = x.Name
                }).ToList());

            return new ApiResult<Dictionary<string, List<ApiResourceScopeResponse>>>(result);
        }
        #endregion

        #region 微服务 - 错误码表
        /// <summary>
        /// 微服务 - 错误码表
        /// </summary>
        /// <remarks>微服务代码对照表</remarks>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation("ApiResource/Codes")]
        public List<ErrorCodeModel> Codes()
        {
            var result = _Codes<ApiResourceControllerEnums>();

            return result;
        }
        #endregion
        #endregion

        #region 微服务 - 网关
        #region 微服务 - 网关 - 发布或更新版本
        /// <summary>
        /// 微服务 - 网关 - 发布或更新版本
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.publish</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.publish</code>
        /// </remarks>
        [HttpPut("{id}/Publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublish)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublish)]
        [SwaggerOperation("ApiResource/Publish")]
        public async Task<ApiResult<bool>> Publish(long id, [FromBody]ApiResourcePublishRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var result = await AzureApim.Apis.ImportOrUpdateAsync(
                value.apiId,
                value.suffix,
                value.swaggerUrl,
                value.productIds,
                value.authorizationServerId,
                new List<string>() { "https" },
                value.scope,
                value.openid);

            // 更新微服务策略
            if (result.IsSuccessStatusCode)
            {
                #region CacheConfigurations
                try
                {
                    var publishKey = $"ApiResource:Publish:{id}";

                    var cacheResult = await redis.SetAsync(publishKey, JsonConvert.SerializeObject(value), null);
                }

                catch (Exception ex)
                {
                    return new ApiResult<bool>(l, ApiResourceControllerEnums.Publish_PublishFailed, ex.Message);
                }
                #endregion

                #region UpdatePolicy
                if (!string.IsNullOrWhiteSpace(value.policy))
                {
                    var policyResult = await AzureApim.Apis.SetPolicyAsync(value.apiId, value.policy);

                    if (!policyResult.IsSuccessStatusCode)
                    {
                        var errorMessage = await policyResult.Content.ReadAsStringAsync();

                        return new ApiResult<bool>(l, ApiResourceControllerEnums.Publish_PublishFailed, errorMessage);
                    }
                }
                #endregion

                #region UpdateName
                if (!string.IsNullOrWhiteSpace(value.name))
                {
                    var body = new JObject();

                    body["name"] = value.name;

                    if (!string.IsNullOrWhiteSpace(value.description))
                    {
                        body["description"] = value.description;
                    }

                    var updateNameResult = await AzureApim.Apis.UpdateAsync(value.apiId, body.ToString());

                    if (!updateNameResult.IsSuccessStatusCode)
                    {
                        var errorMessage = await updateNameResult.Content.ReadAsStringAsync();

                        return new ApiResult<bool>(l, ApiResourceControllerEnums.Publish_PublishFailed, errorMessage);
                    }
                }
                #endregion

                #region Publish message to subscribers
                await storageService.AddMessageAsync("apiresource-publish", id.ToString());
                #endregion

                return new ApiResult<bool>(true);
            }

            else
            {
                var errorMessage = await result.Content.ReadAsStringAsync();

                return new ApiResult<bool>(l, ApiResourceControllerEnums.Publish_PublishFailed, errorMessage);
            }
        }
        #endregion

        #region 微服务 - 网关 - 创建修订版
        /// <summary>
        /// 微服务 - 网关 - 创建修订版
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.publishrevision</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.publishrevision</code>
        /// </remarks>
        [HttpPost("{id}/PublishRevision")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublishRevision)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublishRevision)]
        [SwaggerOperation("ApiResource/PublishRevision")]
        public async Task<ApiResult<bool>> PublishRevision(long id,
            [FromBody]ApiResourcePublishRevisionsRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var ApiRevision = await AzureApim.Apis.CreateRevisionFromSourceApiAsync(value.apiId, value.releaseNote);

            var ApiDetail = await AzureApim.Apis.DetailAsync(value.apiId);

            if (ApiDetail == null)
            {
                return new ApiResult<bool>(l,
                    ApiResourceControllerEnums.PublishRevision_GetDetailFailed);
            }

            var RevisionId = $"{value.apiId};rev={ApiRevision}";

            var ImportResult = await AzureApim.Apis.ImportOrUpdateAsync(RevisionId, ApiDetail.path, value.swaggerUrl);

            if (ImportResult.IsSuccessStatusCode)
            {
                return new ApiResult<bool>(true);
            }
            else
            {
                var errorMessage = await ImportResult.Content.ReadAsStringAsync();

                return new ApiResult<bool>(l, ApiResourceControllerEnums.PublishRevision_PublishFailed, errorMessage);
            }
        }
        #endregion

        #region 微服务 - 网关 - 创建新版本
        /// <summary>
        /// 微服务 - 网关 - 创建新版本
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.publishversion</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.publishversion</code>
        /// </remarks>
        [HttpPost("{id}/PublishVersion")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublishVersion)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublishVersion)]
        [SwaggerOperation("ApiResource/PublishVersion")]
        public async Task<ApiResult<bool>> PublishVersion(long id, [FromBody]ApiResourceCreateVersionRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity, ModelErrors());
            }

            if (!await exists(id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var newApiId = Guid.NewGuid().ToString("N");

            var result = await AzureApim.Apis.CreateVersionAsync(value.revisionId, value.apiVersionName, newApiId);

            var pcts = AzureApim.Products.GetAsync(value.revisionId).Result;

            foreach (var v in pcts.value)
            {
                bool resultx = await AzureApim.Products.AddApiAsync(v.id, newApiId);
            }

            return new ApiResult<bool>(result);
        }
        #endregion

        #region 微服务 - 网关 - 上次发布配置
        /// <summary>
        /// 微服务 - 网关 - 上次发布配置
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.publishconfiguration</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.publishconfiguration</code>
        /// </remarks>
        [HttpGet("{id}/PublishConfiguration")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublishConfiguration)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublishConfiguration)]
        [SwaggerOperation("ApiResource/PublishConfiguration")]
        public async Task<ApiResult<ApiResourcePublishRequest>> PublishConfiguration(long id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResourcePublishRequest>(l, BasicControllerEnums.NotFound);
            }

            try
            {
                var result = await _PublishConfiguration(id);

                return new ApiResult<ApiResourcePublishRequest>(result);
            }
            catch (Exception ex)
            {
                return new ApiResult<ApiResourcePublishRequest>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source);
            }
        }

        async Task<ApiResourcePublishRequest> _PublishConfiguration(long id)
        {
            ApiResourcePublishRequest result = null;

            var publishKey = $"ApiResource:Publish:{id}";

            var resultCache = await redis.GetAsync(publishKey);

            if (!string.IsNullOrWhiteSpace(resultCache))
            {
                result = JsonConvert.DeserializeObject<ApiResourcePublishRequest>(resultCache);
            }

            return result;
        }
        #endregion

        #region 微服务 - 网关 - 版本列表
        /// <summary>
        /// 微服务 - 网关 - 版本列表
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.versions</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.versions</code>
        /// </remarks>
        [HttpGet("{id}/Versions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceVersions)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceVersions)]
        [SwaggerOperation("ApiResource/Versions")]
        [ResponseCache(Duration = 60)]
        public async Task<PagingResult<ApiResourceVersionsResponse>> Versions(long id)
        {
            var detail = await AzureApim.Apis.DetailAsync(id.ToString());

            if (detail == null)
            {
                return new PagingResult<ApiResourceVersionsResponse>(l,
                    ApiResourceControllerEnums.Versions_GetDetailFailed);
            }

            var response = await AzureApim.Apis.GetByPathAsync(detail.path);

            if (response == null)
            {
                return new PagingResult<ApiResourceVersionsResponse>(l,
                    ApiResourceControllerEnums.Versions_GetVersionListFailed);
            }

            var apiVersions = new List<ApiResourceVersionsResponse>();

            foreach (var v in response.value)
            {
                var apiItem = v.ToModel();

                var apiRevisions = await AzureApim.Apis.GetRevisionsAsync(v.id.Replace("/apis/", string.Empty));

                apiItem.revisions = apiRevisions.value;

                apiVersions.Add(apiItem);
            }

            var result = new PagingResult<ApiResourceVersionsResponse>(apiVersions,
                response.count,
                0,
                response.value.Count);

            return result;
        }
        #endregion

        #region 微服务 - 网关 - 上线指定版本
        /// <summary>
        /// 微服务 - 网关 - 上线指定版本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="revisionId"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.setonlineversion</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.setonlineversion</code>
        /// </remarks>
        [HttpPost("{id}/Versions/{revisionId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceSetOnlineVersion)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceSetOnlineVersion)]
        [SwaggerOperation("ApiResource/SetOnlineVersion")]
        public async Task<ApiResult<bool>> SetOnlineVersion(long id, string revisionId)
        {
            if (!await exists(id) || string.IsNullOrWhiteSpace(revisionId))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var ReleaseResult = await AzureApim.Apis.CreateReleaseAsync(revisionId, string.Empty);

            if (ReleaseResult)
            {
                return new ApiResult<bool>(true);
            }
            else
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.SetOnlineVersion_PostFailed);
            }
        }
        #endregion

        #region 微服务 - 网关 - OAuthServers
        /// <summary>
        /// 微服务 - 网关 - OAuthServers
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.authservers</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.authservers</code>
        /// </remarks>
        [HttpGet("AuthServers")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceAuthServers)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceAuthServers)]
        [SwaggerOperation("ApiResource/AuthServers")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>> AuthServers()
        {
            var result = await AzureApim.AuthorizationServers.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>(result);
        }
        #endregion

        #region 微服务 - 网关 - 产品包列表
        /// <summary>
        /// 微服务 - 网关 - 产品包列表
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.products</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.products</code>
        /// </remarks>
        [HttpGet("Products")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceProducts)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceProducts)]
        [SwaggerOperation("ApiResource/Products")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementProductEntity>>> Products()
        {
            var result = await AzureApim.Products.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementProductEntity>>(result);
        }
        #endregion
        #endregion

        #region 微服务 - 修订内容
        #region 微服务 - 修订内容 - 列表
        /// <summary>
        /// 微服务 - 修订内容 - 列表
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="apiId">Api的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.releases</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.releases</code>
        /// </remarks>
        [HttpGet("{id}/Releases")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceReleases)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceReleases)]
        [SwaggerOperation("ApiResource/Releases")]
        public async Task<PagingResult<AzureApiManagementReleaseEntity>> Releases(long id, string apiId)
        {
            if (string.IsNullOrWhiteSpace(apiId))
            {
                return new PagingResult<AzureApiManagementReleaseEntity>(l,
                    ApiResourceControllerEnums.Releases_IdCanNotBeNull);
            }

            var response = await AzureApim.Apis.GetReleasesAsync(apiId);

            if (response == null)
            {
                return new PagingResult<AzureApiManagementReleaseEntity>(l,
                    ApiResourceControllerEnums.Releases_GetVersionListFailed);
            }

            var result = new PagingResult<AzureApiManagementReleaseEntity>(response.value,
                response.count,
                0,
                response.value.Count);

            return result;
        }
        #endregion

        #region 微服务 - 修订内容 - 发布
        /// <summary>
        /// 微服务 - 修订内容 - 发布
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.postrelease</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.postrelease</code>
        /// </remarks>
        [HttpPost("{id}/Releases")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePostRelease)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePostRelease)]
        [SwaggerOperation("ApiResource/PostRelease")]
        public async Task<ApiResult<bool>> PostRelease(long id, [FromBody]ApiResourcePostReleaseRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var result = await AzureApim.Apis.CreateReleaseAsync(value.aid, value.notes);

            return new ApiResult<bool>(result);
        }
        #endregion

        #region 微服务 - 修订内容 - 更新
        /// <summary>
        /// 微服务 - 修订内容 - 更新
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="releaseId">修订内容的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.putrelease</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.putrelease</code>
        /// </remarks>
        [HttpPut("{id}/Releases/{releaseId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePutRelease)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePutRelease)]
        [SwaggerOperation("ApiResource/PutRelease")]
        public async Task<ApiResult<bool>> PutRelease(long id, string releaseId, [FromBody]ApiResourcePutReleaseRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (string.IsNullOrWhiteSpace(releaseId))
            {
                return new ApiResult<bool>(l,
                    ApiResourceControllerEnums.Releases_IdCanNotBeNull);
            }

            var result = await AzureApim.Apis.UpdateReleaseAsync($"/apis/{id}/releases/{releaseId}", value.notes);

            return new ApiResult<bool>(result);
        }
        #endregion

        #region 微服务 - 修订内容 - 删除
        /// <summary>
        /// 微服务 - 修订内容 - 删除
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="releaseId">修订内容的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.deleterelease</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.deleterelease</code>
        /// </remarks>
        [HttpDelete("{id}/Releases/{releaseId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDeleteRelease)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDeleteRelease)]
        [SwaggerOperation("ApiResource/DeleteRelease")]
        public async Task<ApiResult<bool>> DeleteRelease(long id, string releaseId)
        {
            if (string.IsNullOrWhiteSpace(releaseId))
            {
                return new ApiResult<bool>(l,
                    ApiResourceControllerEnums.Releases_IdCanNotBeNull);
            }

            var result = await AzureApim.Apis.DeleteReleaseAsync($"/apis/{id}/releases/{releaseId}");

            return new ApiResult<bool>(result);
        }
        #endregion
        #endregion

        #region 微服务 - 订阅者
        #region 微服务 - 订阅者 - 列表
        /// <summary>
        /// 微服务 - 订阅者 - 列表
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.subscriptions</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.subscriptions</code>
        /// </remarks>
        [HttpGet("{id}/Subscriptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceSubscriptions)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceSubscriptions)]
        [SwaggerOperation("ApiResource/Subscriptions")]
        public async Task<PagingResult<ApiResourceSubscriptionEntity>> Subscriptions(long id)
        {
            if (!await exists(id))
            {
                return new PagingResult<ApiResourceSubscriptionEntity>(l, BasicControllerEnums.NotFound);
            }

            var tb = await storageService.CreateTableAsync("ApiResourceSubscriptions");

            var query = new TableQuery<ApiResourceSubscriptionEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

            var result = await storageService.ExecuteQueryAsync(tb, query);

            return new PagingResult<ApiResourceSubscriptionEntity>()
            {
                data = result,
                skip = 0,
                take = result.Count,
                total = result.Count
            };
        }
        #endregion

        #region 微服务 - 订阅者 - 添加
        /// <summary>
        /// 微服务 - 订阅者 - 添加
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="code">邮箱校验加密字符串</param>
        /// <returns></returns>
        [HttpGet("{id}/AddSubscription")]
        [AllowAnonymous]
        [SwaggerOperation("ApiResource/AddSubscription")]
        public async Task<ApiResult<bool>> AddSubscription(long id,
            [FromQuery]string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    "无效的订阅验证码");
            }

            string Email = string.Empty;

            try
            {
                Email = Unprotect(code);
            }
            catch
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_VerfifyCodeFailed);
            }

            var tb = await storageService.CreateTableAsync("ApiResourceSubscriptions");

            try
            {
                var result = await storageService.TableInsertAsync(tb, new ApiResourceSubscriptionEntity(id.ToString(), Email));

                if (result.FirstOrDefault().Result != null)
                {
                    return new ApiResult<bool>(true);
                }

                else
                {
                    return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_PostFailed);
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_PostFailed, ex.Message);
            }
        }
        #endregion

        #region 微服务 - 订阅者 - 取消
        /// <summary>
        /// 微服务 - 订阅者 - 取消
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="code">邮箱校验加密字符串</param>
        /// <returns></returns>
        [HttpGet("{id}/DelSubscription")]
        [AllowAnonymous]
        [SwaggerOperation("ApiResource/DelSubscription")]
        public async Task<ApiResult<bool>> DelSubscription(long id,
            [FromQuery]string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    "无效的订阅验证码");
            }

            string Email = string.Empty;

            try
            {
                Email = Encoding.UTF8.GetString(Convert.FromBase64String(code));
            }
            catch
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_DelSubscriptionFailed);
            }

            var tb = await storageService.CreateTableAsync("ApiResourceSubscriptions");

            try
            {
                var retrieveOperation = TableOperation.Retrieve<ApiResourceSubscriptionEntity>(id.ToString(), Email);

                var retrievedResult = await tb.ExecuteAsync(retrieveOperation);

                if (retrievedResult.Result != null)
                {
                    var deleteEntity = (ApiResourceSubscriptionEntity)retrievedResult.Result;

                    var deleteOperation = TableOperation.Delete(deleteEntity);

                    var result = await tb.ExecuteAsync(deleteOperation);

                    if (result.Result != null)
                    {
                        return new ApiResult<bool>(true);
                    }

                    else
                    {
                        return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_DelSubscriptionFailed);
                    }
                }

                return new ApiResult<bool>(true);

            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_DelSubscriptionFailed, ex.Message);
            }
        }
        #endregion

        #region 微服务 - 订阅者 - 验证邮箱
        /// <summary>
        /// 微服务 - 订阅者 - 验证邮箱
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.verifyemail</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.verifyemail</code>
        /// </remarks>
        [HttpPost("{id}/Subscriptions/VerifyEmail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceVerifyEmail)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceVerifyEmail)]
        [SwaggerOperation("ApiResource/VerifyEmail")]
        public async Task<ApiResult<bool>> VerifyEmail(long id, [FromBody]ApiResourceSubscriptionsVerifyEmailRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 微服务是否存在
            var apiEntity = configDb.ApiResources.FirstOrDefault(x => x.Id == id);
            if (apiEntity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }
            #endregion

            #region 邮箱是否已经订阅
            var tb = await storageService.CreateTableAsync("ApiResourceSubscriptions");

            try
            {
                var retrieveOperation = TableOperation.Retrieve<ApiResourceSubscriptionEntity>(id.ToString(), value.email);

                var result = await tb.ExecuteAsync(retrieveOperation);

                if (result.Result != null)
                {
                    return new ApiResult<bool>(l, ApiResourceControllerEnums.VerifyEmail_AddEmailFailed);
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.VerifyEmail_AddEmailFailed, ex.Message);
            }
            #endregion

            #region 发送频次校验

            #endregion

            try
            {
                var code = Protect(value.email, TimeSpan.FromSeconds(UserControllerKeys.VerifyCode_Expire_Email));

                #region 订阅地址
                var callbackUrl = Url.Action(
                            "AddSubscription",
                            "ApiResource",
                           new { code },
                           protocol: HttpContext.Request.Scheme);
                #endregion

                #region 取消订阅
                var DelSubscritionUrl = Url.Action(
                           "DelSubscription",
                           "ApiResource",
                          new { code = Convert.ToBase64String(Encoding.UTF8.GetBytes(value.email)) },
                          protocol: HttpContext.Request.Scheme);
                #endregion

                #region serviceName
                var serviceName = string.Empty;
                var pubConfig = await _PublishConfiguration(id);
                if (pubConfig != null && !string.IsNullOrWhiteSpace(pubConfig.name))
                {
                    serviceName = pubConfig.name;
                }
                else if (!string.IsNullOrWhiteSpace(apiEntity.DisplayName))
                {
                    serviceName = apiEntity.DisplayName;
                }
                else
                {
                    serviceName = apiEntity.Name;
                }
                #endregion

                var result = await email.SendEmailAsync(
                    SendCloudMailTemplates.verify_apiresource_subscription,
                   new string[] { value.email },
                    new Dictionary<string, string[]>()
                    {
                        { "%SubscritionUrl%", new string[] { callbackUrl } },
                        { "%DelSubscritionUrl%", new string[] { DelSubscritionUrl } },
                        { "%apiId%", new string[] { id.ToString() } },
                        { "%serviceName%", new string[] { serviceName } },
                    });

                if (result)
                {
                    return new ApiResult<bool>(true);
                }

                else
                {
                    return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_VerifyEmailFailed);
                }

            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Subscription_VerifyEmailFailed, ex.Message);
            }
        }
        #endregion
        #endregion

        #region 微服务 - 包市场
        #region 微服务 - 包市场 - 列表
        /// <summary>
        /// 微服务 - 包市场 - 列表
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.packages</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.packages</code>
        /// </remarks>
        [HttpGet("{id}/Packages")]
        [SwaggerOperation("ApiResource/Packages")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePackages)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePackages)]
        public async Task<PagingResult<ApiResourceSDKEntity>> Packages(string id)
        {
            var tb = await storageService.CreateTableAsync("ApiResourcePackages");

            var query = new TableQuery<ApiResourceSDKEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id));

            var result = await storageService.ExecuteQueryAsync(tb, query);

            return new PagingResult<ApiResourceSDKEntity>()
            {
                data = result,
                skip = 0,
                take = result.Count,
                total = result.Count
            };
        }
        #endregion

        #region 微服务 - 包市场 - 添加
        /// <summary>
        /// 微服务 - 包市场 - 添加
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.postpackages</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.postpackages</code>
        /// </remarks>
        [HttpPost("{id}/Packages")]
        [SwaggerOperation("ApiResource/PostPackage")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePostPackage)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePostPackage)]
        public async Task<ApiResult<bool>> PostPackage(string id, [FromBody]ApiResourceSDKRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var tb = await storageService.CreateTableAsync("ApiResourcePackages");

            try
            {
                var Entity = new ApiResourceSDKEntity(id.ToString(), value.name)
                {
                    Description = value.Description,
                    Icon = value.Icon,
                    Language = value.Language,
                    Link = value.Link,
                    PackagePlatform = value.PackagePlatform,
                    Publisher = value.Publisher,
                    ShowIndex = value.ShowIndex,
                    Tags = value.Tags
                };

                var result = await storageService.TableInsertAsync(tb, Entity);

                if (result.FirstOrDefault().Result != null)
                {
                    return new ApiResult<bool>(true);
                }

                else
                {
                    return new ApiResult<bool>(l, ApiResourceControllerEnums.Packages_PostFailed);
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Packages_PostFailed, ex.Message);
            }
        }
        #endregion

        #region 微服务 - 包市场 - 删除
        /// <summary>
        /// 微服务 - 包市场 - 删除
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="packageId">包的ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.deletepackage</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.deletepackage</code>
        /// </remarks>
        [HttpDelete("{id}/Packages/{packageId}")]
        [SwaggerOperation("ApiResource/DeletePackage")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDeletePackage)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDeletePackage)]
        public async Task<ApiResult<bool>> DeletePackage(string id, string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    "无效的包ID");
            }

            var tb = await storageService.CreateTableAsync("ApiResourcePackages");

            try
            {
                var retrieveOperation = TableOperation.Retrieve<ApiResourceSDKEntity>(id.ToString(), packageId);

                var retrievedResult = await tb.ExecuteAsync(retrieveOperation);

                if (retrievedResult.Result != null)
                {
                    var deleteEntity = (ApiResourceSDKEntity)retrievedResult.Result;

                    var deleteOperation = TableOperation.Delete(deleteEntity);

                    var result = await tb.ExecuteAsync(deleteOperation);

                    if (result.Result != null)
                    {
                        return new ApiResult<bool>(true);
                    }

                    else
                    {
                        return new ApiResult<bool>(l, ApiResourceControllerEnums.Packages_DelPackageFailed);
                    }
                }

                return new ApiResult<bool>(true);

            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Packages_DelPackageFailed, ex.Message);
            }
        }
        #endregion 

        #region 微服务 - 包市场 - 更新
        /// <summary>
        /// 微服务 - 包市场 - 更新
        /// </summary>
        /// <param name="id">微服务的ID</param>
        /// <param name="packageId">包的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.apiresource.deletepackage</code>
        /// <label>User Permissions：</label><code>ids4.ms.apiresource.deletepackage</code>
        /// </remarks>
        [HttpPut("{id}/Packages/{packageId}")]
        [SwaggerOperation("ApiResource/PutPackage")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePutPackage)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePutPackage)]
        public async Task<ApiResult<bool>> PutPackage(string id, string packageId, [FromBody]ApiResourceSDKRequest value)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    "无效的包ID");
            }

            var tb = await storageService.CreateTableAsync("ApiResourcePackages");

            try
            {
                var retrieveOperation = TableOperation.Retrieve<ApiResourceSDKEntity>(id.ToString(), packageId);

                var retrievedResult = await tb.ExecuteAsync(retrieveOperation);

                if (retrievedResult.Result != null)
                {
                    var updateEntity = (ApiResourceSDKEntity)retrievedResult.Result;

                    updateEntity.Description = value.Description;
                    updateEntity.Language = value.Language;
                    updateEntity.Link = value.Link;
                    updateEntity.Icon = value.Icon;
                    updateEntity.PackagePlatform = value.PackagePlatform;
                    updateEntity.Publisher = value.Publisher;
                    updateEntity.ShowIndex = value.ShowIndex;
                    updateEntity.Tags = value.Tags;

                    var updateOperation = TableOperation.Replace(updateEntity);

                    var result = await tb.ExecuteAsync(updateOperation);

                    if (result.Result != null)
                    {
                        return new ApiResult<bool>(true);
                    }

                    else
                    {
                        return new ApiResult<bool>(l, ApiResourceControllerEnums.Packages_PutPackageFailed);
                    }
                }

                return new ApiResult<bool>(true);

            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.Packages_PutPackageFailed, ex.Message);
            }
        }
        #endregion 
        #endregion

        #region 辅助方法
        const string _ExistsCmd = "SELECT Id FROM AspNetUserApiResources WHERE UserId = {0} AND ApiResourceId = {1}";
        async Task<bool> exists(long id)
        {
            var result = await db.ExecuteScalarAsync(string.Format(_ExistsCmd, UserId, id));

            if (result != null)
            {
                long.TryParse(result.ToString(), out long userApiResourceId);

                return userApiResourceId > 0;
            }

            return false;
        } 
        #endregion
    }
}