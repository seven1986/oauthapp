using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.ApiResourceController;
using static IdentityServer4.MicroService.AppConstant;
using static IdentityServer4.MicroService.MicroserviceConfig;

namespace IdentityServer4.MicroService.Apis
{
    // ApiResource 根据 userId 来获取列表、或详情、增删改

    /// <summary>
    /// 微服务
    /// </summary>
    [Route("ApiResource")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class ApiResourceController : BasicController
    {
        #region Services
        //Database
        readonly ConfigurationDbContext db;
        readonly IdentityDbContext userDb;
        readonly SwaggerCodeGenService swagerCodeGen;
        #endregion

        public ApiResourceController(
            ConfigurationDbContext _db,
            IdentityDbContext _userDb,
            IStringLocalizer<ApiResourceController> localizer,
            TenantService _tenantService,
            TenantDbContext _tenantDb,
            RedisService _redis,
            SwaggerCodeGenService _swagerCodeGen)
        {
            db = _db;
            userDb = _userDb;
            l = localizer;
            tenantDb = _tenantDb;
            tenantService = _tenantService;
            redis = _redis;
            swagerCodeGen = _swagerCodeGen;
        }

        /// <summary>
        /// 微服务 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

            var query = db.ApiResources.AsQueryable();

            var ApiResourceIds = await userDb.UserApiResources.Where(x => x.UserId == UserId)
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

        /// <summary>
        /// 微服务 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDetail)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDetail)]
        [SwaggerOperation("ApiResource/Detail")]
        public async Task<ApiResult<ApiResource>> Get(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResource>(l, BasicControllerEnums.NotFound);
            }

            var query = db.ApiResources.AsQueryable();

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

        /// <summary>
        /// 微服务 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

            db.Add(value);

            await db.SaveChangesAsync();

            userDb.UserApiResources.Add(new AspNetUserApiResource()
            {
                ApiResourceId = value.Id,
                UserId = UserId
            });

            await userDb.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }

        /// <summary>
        /// 微服务 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

            using (var tran = db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    db.Update(value);
                    db.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await db.ApiResources.Where(x => x.Id == value.Id)
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
                                var sql = string.Format("DELETE ApiClaims WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.UserClaims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ApiClaims SET [Type]=@Type WHERE Id = " + x.Id),
                                  new SqlParameter("@Type", x.Type));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.UserClaims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ApiClaims VALUES (@ApiResourceId,@Type)"),
                                  new SqlParameter("@ApiResourceId", source.Id),
                                  new SqlParameter("@Type", x.Type));
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
                                var sql = string.Format("DELETE ApiSecrets WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Secrets.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                var _params = new SqlParameter[] {
                                  new SqlParameter("@Description", DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@Expiration", DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@Type",  DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@Value",  DBNull.Value) { IsNullable = true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                if (x.Expiration.HasValue) { _params[1].Value = x.Expiration; }
                                if (!string.IsNullOrWhiteSpace(x.Type)) { _params[2].Value = x.Type; }
                                if (!string.IsNullOrWhiteSpace(x.Value)) { _params[3].Value = x.Value; }

                                var sql = new RawSqlString("UPDATE ApiSecrets SET [Description]=@Description,[Expiration]=@Expiration,[Type]=@Type,[Value]=@Value WHERE Id = " + x.Id);

                                db.Database.ExecuteSqlCommand(sql, _params);
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Secrets.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                var _params = new SqlParameter[] {
                                   new SqlParameter("@ApiResourceId", source.Id),
                                   new SqlParameter("@Description", DBNull.Value) { IsNullable = true },
                                   new SqlParameter("@Expiration", DBNull.Value) { IsNullable = true },
                                   new SqlParameter("@Type", DBNull.Value){ IsNullable = true },
                                   new SqlParameter("@Value", DBNull.Value){ IsNullable = true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                if (x.Expiration.HasValue) { _params[1].Value = x.Expiration; }
                                if (!string.IsNullOrWhiteSpace(x.Type)) { _params[2].Value = x.Type; }
                                if (!string.IsNullOrWhiteSpace(x.Value)) { _params[3].Value = x.Value; }

                                var sql = new RawSqlString("INSERT INTO ApiSecrets VALUES (@ApiResourceId,@Description,@Expiration,@Type,@Value)");

                                db.Database.ExecuteSqlCommand(sql, _params);
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
                                var sql = string.Format("DELETE ApiScopeClaims WHERE ApiScopeId IN ({0})",
                                           string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));

                                sql = string.Format("DELETE ApiScopes WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Scopes.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                var _params = new SqlParameter[] {
                                  new SqlParameter("@Description", DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@DisplayName", DBNull.Value) { IsNullable = true },
                                  new SqlParameter("@Emphasize", x.Emphasize),
                                  new SqlParameter("@Name", x.Name),
                                  new SqlParameter("@Required", x.Required),
                                  new SqlParameter("@ShowInDiscoveryDocument", x.ShowInDiscoveryDocument)
                                };

                                if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                if (!string.IsNullOrWhiteSpace(x.DisplayName)) { _params[1].Value = x.DisplayName; }

                                var sql = new RawSqlString("UPDATE ApiScopes SET [Description]=@Description,[DisplayName]=@DisplayName,[Emphasize]=@Emphasize,[Name]=@Name,[Required]=@Required,[ShowInDiscoveryDocument]=@ShowInDiscoveryDocument WHERE Id = " + x.Id);

                                db.Database.ExecuteSqlCommand(sql, _params);

                                db.Database.ExecuteSqlCommand(
                                    new RawSqlString("DELETE ApiScopeClaims WHERE ApiScopeId =" + x.Id));

                                x.UserClaims.ForEach(claim =>
                                {
                                    db.Database.ExecuteSqlCommand(
                                     new RawSqlString("INSERT INTO ApiScopeClaims VALUES (@ApiScopeId,@Type)"),
                                     new SqlParameter("@ApiScopeId", x.Id),
                                     new SqlParameter("@Type", claim.Type));
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

                                db.Database.ExecuteSqlCommand(sql, _params);

                                if (_params[_params.Length-1].Value != null)
                                {
                                    var _ApiScopeId = long.Parse(_params[_params.Length - 1].Value.ToString());

                                    x.UserClaims.ForEach(claim =>
                                    {
                                        db.Database.ExecuteSqlCommand(
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

        /// <summary>
        /// 微服务 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceDelete)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceDelete)]
        [SwaggerOperation("ApiResource/Delete")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            var entity = await db.ApiResources.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            db.ApiResources.Remove(entity);

            await db.SaveChangesAsync();

            return new ApiResult<long>(id);
        }

        #region Api Management

        /// <summary>
        /// 微服务 - 发布
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("Publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublish)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublish)]
        [SwaggerOperation("ApiResource/Publish")]
        public async Task<ApiResult<bool>> Publish([FromBody]ApiResourcePublishRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var result = await AzureApim.Apis.ImportOrUpdateAsync(
                value.id.ToString(),
                value.suffix,
                value.swaggerUrl,
                new string[] { value.productId },
                value.authorizationServerId,
                new List<string>() { "https" },
                value.scope,
                value.openid);

            // 更新微服务策略
            if (result && !string.IsNullOrWhiteSpace(value.policy))
            {
                await AzureApim.Apis.SetPolicyAsync(value.id.ToString(), value.policy);
            }

            var publishKey = $"ApiResource:Publish:{value.id}";

            await redis.SetAsync(publishKey, JsonConvert.SerializeObject(value), null);

            return new ApiResult<bool>(result);
        }

        /// <summary>
        /// 微服务 - 上次发布配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Publish/{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublishSetting)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublishSetting)]
        [SwaggerOperation("ApiResource/PublishSetting")]
        public async Task<ApiResult<ApiResourcePublishRequest>> Publish(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResourcePublishRequest>(l, BasicControllerEnums.NotFound);
            }

            ApiResourcePublishRequest result = null;

            var publishKey = $"ApiResource:Publish:{id}";

            var resultCache = await redis.GetAsync(publishKey);

            if (!string.IsNullOrWhiteSpace(resultCache))
            {
                result = JsonConvert.DeserializeObject<ApiResourcePublishRequest>(resultCache);
            }

            return new ApiResult<ApiResourcePublishRequest>(result);
        }

        /// <summary>
        /// 微服务 - 版本列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Versions/{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceVersions)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceVersions)]
        [SwaggerOperation("ApiResource/Versions")]
        public async Task<PagingResult<AzureApiManagementApiEntity>> Versions(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new PagingResult<AzureApiManagementApiEntity>(l, ApiResourceControllerEnums.Revisions_IdCanNotBeNull);
            }

            var detail = await AzureApim.Apis.DetailAsync(id);

            var response = await AzureApim.Apis.GetByPathAsync(detail.path);

            var result = new PagingResult<AzureApiManagementApiEntity>(response.value, 
                response.count,
                0, 
                response.value.Count);

            return result;
        }

        /// <summary>
        /// 微服务 - 发修订版
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Revisions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublishRevision)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublishRevision)]
        [SwaggerOperation("ApiResource/PublishRevision")]
        public async Task<ApiResult<bool>> Revisions([FromBody]ApiResourcePublishRevisionsRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var ApiRevision = await AzureApim.Apis.CreateRevisionFromSourceApiAsync(value.id.ToString(), value.releaseNote);

            var ApiDetail = await AzureApim.Apis.DetailAsync(value.id.ToString());

            if (ApiDetail == null) { return new ApiResult<bool>(l, ApiResourceControllerEnums.PublishRevision_GetDetailFailed); }

            var RevisionId = $"{value.id};rev={ApiRevision}";

            var ImportResult = await AzureApim.Apis.ImportOrUpdateAsync(RevisionId, ApiDetail.path, value.swaggerUrl);

            if (ImportResult)
            {
                var ReleaseResult = await AzureApim.Apis.CreateReleaseAsync(RevisionId, value.releaseNote);

                if (ReleaseResult)
                {
                    return new ApiResult<bool>(true);
                }
                else
                {
                    return new ApiResult<bool>(l, ApiResourceControllerEnums.PublishRevision_CreateReleaseFailed);
                }
            }

            else
            {
                return new ApiResult<bool>(l, ApiResourceControllerEnums.PublishRevision_PublishFailed);
            }
        }

        /// <summary>
        /// 微服务 - 发新版本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Versions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourcePublishVersion)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourcePublishVersion)]
        [SwaggerOperation("ApiResource/PublishVersion")]
        public async Task<ApiResult<bool>> Versions([FromBody]ApiResourceCreateVersionRequest value)
        {
            if(!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity, ModelErrors());
            }

            var newApiId = Guid.NewGuid().ToString("N");

            var result = await AzureApim.Apis.CreateVersionAsync(value.revisionId, value.apiVersionName, newApiId);

            return new ApiResult<bool>(result);
        }

        /// <summary>
        /// 微服务 - OAuthServers
        /// </summary>
        /// <returns></returns>
        [HttpGet("AuthServers")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.ApiResourceAuthServers)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.ApiResourceAuthServers)]
        [SwaggerOperation("ApiResource/AuthServers")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>> AuthServers()
        {
            var result = await AzureApim.AuthorizationServers.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>(result);
        }

        /// <summary>
        /// 微服务 - 产品组
        /// </summary>
        /// <returns></returns>
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

        #region 微服务 - 错误码表
        /// <summary>
        /// 微服务 - 错误码表
        /// </summary>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation("ApiResource/Codes")]
        public List<ErrorCodeModel> Codes()
        {
            var result = _Codes<ApiResourceControllerEnums>();

            return result;
        }
        #endregion

        const string _ExistsCmd = "SELECT Id FROM AspNetUserApiResources WHERE UserId = {0} AND ApiResourceId = {1}";

        async Task<bool> exists(long id)
        {
            var result = await userDb.ExecuteScalarAsync(string.Format(_ExistsCmd, UserId, id));

            if (result != null)
            {
                long.TryParse(result.ToString(), out long userApiResourceId);

                return userApiResourceId > 0;
            }

            return false;
        }
    }
}