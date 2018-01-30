using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
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
using IdentityServer4.MicroService.ApiCodes;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.ApiResourceModels;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    // ApiResource 根据 userId 来获取列表、或详情、增删改

    [Route("ApiResource")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class ApiResourceController : BasicController
    {
        #region Services
        //Database
        readonly ConfigurationDbContext db;
        readonly ApplicationDbContext userDb;
        readonly SwaggerCodeGenService swagerCodeGen;
        #endregion

        public ApiResourceController(
            ConfigurationDbContext _db,
            ApplicationDbContext _userDb,
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

        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("ApiResource/Get")]
        public async Task<PagingResult<ApiResource>> Get(PagingRequest<ApiResourceQuery> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<ApiResource>()
                {
                    code = (int)BasicControllerCodes.UnprocessableEntity,
                    error_msg = ModelErrors()
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
            if (!string.IsNullOrWhiteSpace(value.q.Name))
            {
                query = query.Where(x => x.Name.Equals(value.q.Name));
            }
            #endregion

            #region total
            var result = new PagingResult<ApiResource>()
            {
                skip = value.skip,
                take = value.take,
                total = await query.CountAsync()
            };
            #endregion

            if (result.total > 0)
            {
                #region orderby
                if (!string.IsNullOrWhiteSpace(value.orderby))
                {
                    if (value.asc)
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
                var data = await query.Skip(value.skip).Take(value.take)
                    .Include(x => x.UserClaims)
                    .Include(x => x.Scopes)
                    .Include(x => x.Secrets)
                    .ToListAsync();
                #endregion

                result.data = data;
            }

            return result;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("ApiResource/Detail")]
        public async Task<ApiResult<ApiResource>> Get(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResource>(l, BasicControllerCodes.NotFound);
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
                return new ApiResult<ApiResource>(l, BasicControllerCodes.NotFound);
            }

            return new ApiResult<ApiResource>(entity);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Create)]
        [SwaggerOperation("ApiResource/Post")]
        public async Task<ApiResult<long>> Post([FromBody]ApiResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerCodes.UnprocessableEntity,
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

        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Update)]
        [SwaggerOperation("ApiResource/Put")]
        public async Task<ApiResult<long>> Put([FromBody]ApiResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerCodes.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.Id))
            {
                return new ApiResult<long>(l, BasicControllerCodes.NotFound);
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
                        BasicControllerCodes.ExpectationFailed,
                        ex.Message);
                }
            }

            return new ApiResult<long>(value.Id);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Delete)]
        [SwaggerOperation("ApiResource/Delete")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<long>(l, BasicControllerCodes.NotFound);
            }

            var entity = await db.ApiResources.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerCodes.NotFound);
            }

            db.ApiResources.Remove(entity);

            await db.SaveChangesAsync();

            return new ApiResult<long>(id);
        }

        #region MyRegion
        /// <summary>
        /// 发布微服务到网关
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("Publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Update)]
        [SwaggerOperation("ApiResource/Publish")]
        public async Task<ApiResult<bool>> Publish([FromBody]ApiResourcePublishModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerCodes.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.id))
            {
                return new ApiResult<bool>(l, BasicControllerCodes.NotFound);
            }

            #region 如果 oauth2 或 productId为空，从租户配置读取默认配置
            if (string.IsNullOrWhiteSpace(value.authorizationServerId) &&
                    Tenant.properties.ContainsKey(AzureApiManagementConsts.AuthorizationServerId))
            {
                value.authorizationServerId = Tenant.properties[AzureApiManagementConsts.AuthorizationServerId];
            }

            if (string.IsNullOrWhiteSpace(value.productId) &&
                Tenant.properties.ContainsKey(AzureApiManagementConsts.ProductId))
            {
                value.productId = Tenant.properties[AzureApiManagementConsts.ProductId];
            }
            #endregion

            // 发布或更新微服务到网关
            var result = await AzureApim.Apis.ImportOrUpdateAsync(
                value.id.ToString(),
                value.suffix,
                value.swaggerUrl,
                value.productId,
                value.authorizationServerId,
                value.scope,
                value.openid);

            // 更新微服务策略
            if (result && !string.IsNullOrWhiteSpace(value.policy))
            {
                await AzureApim.Apis.SetPolicyAsync(value.id.ToString(), value.policy);
            }

            var publishKey = $"ApiResource:Publish:{value.id}";

            await redis.Set(publishKey, JsonConvert.SerializeObject(value), null);

            return new ApiResult<bool>(result);
        }

        /// <summary>
        /// 获取微服务的发布配置（最后一次发布）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("PublishSetting/{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("ApiResource/PublishSetting")]
        public async Task<ApiResult<ApiResourcePublishModel>> PublishSetting(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResourcePublishModel>(l, BasicControllerCodes.NotFound);
            }

            ApiResourcePublishModel result = null;

            var publishKey = $"ApiResource:Publish:{id}";

            var resultCache = await redis.Get(publishKey);

            if (!string.IsNullOrWhiteSpace(resultCache))
            {
                result = JsonConvert.DeserializeObject<ApiResourcePublishModel>(resultCache);
            }

            return new ApiResult<ApiResourcePublishModel>(result);
        }

        /// <summary>
        /// 微服务可使用的OAuthServers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AuthorizationServers")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("ApiResource/AuthorizationServers")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>> AuthorizationServers()
        {
            var result = await AzureApim.AuthorizationServers.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>(result);
        }

        /// <summary>
        /// 获取微服务产品组集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Products")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("ApiResource/Products")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementProductEntity>>> Products()
        {
            var result = await AzureApim.Products.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementProductEntity>>(result);
        }

        /// <summary>
        /// 保存微服务渠道包数据
        /// </summary>
        [HttpPost("ChannelsPackage")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Create)]
        [SwaggerOperation("ApiResource/ChannelsPackage")]
        public async Task<ApiResult<bool>> ClientPackage([FromBody]GenerateClientModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerCodes.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.id))
            {
                return new ApiResult<bool>(l, BasicControllerCodes.NotFound);
            }

            var clients = new Dictionary<string, SwaggerCodeGenResult>();

            foreach (var v in value.languages)
            {
                var genResult = await swagerCodeGen.ClientGenerate(v.Key, v.Value);

                if (!clients.ContainsKey(v.Key))
                {
                    clients.Add(v.Key, genResult);
                }

                var ClientsSDKKey = $"ApiResource:ClientsSDK:{value.id}:{v.Key}";

                var cacheResult = await redis.Set(ClientsSDKKey, JsonConvert.SerializeObject(clients), null);
            }

            return new ApiResult<bool>(true);
        }

        /// <summary>
        /// 获取微服务渠道包数据
        /// </summary>
        [HttpGet("ChannelsPackageList/{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("ApiResource/ChannelsPackageList")]
        public async Task<ApiResult<GenerateClientModel>> ChannelsPackageList(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<GenerateClientModel>(l, BasicControllerCodes.NotFound);
            }

            GenerateClientModel result = null;

            var ClientsSDKKey = $"ApiResource:ClientsSDK:{id}";

            var resultCache = await redis.Get(ClientsSDKKey);

            if (!string.IsNullOrWhiteSpace(resultCache))
            {
                result = JsonConvert.DeserializeObject<GenerateClientModel>(resultCache);
            }

            return new ApiResult<GenerateClientModel>(result);
        }

        /// <summary>
        /// 可生成的client
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        [HttpGet("ClientLanguegs")]
        [AllowAnonymous]
        [SwaggerOperation("ApiResource/ClientLanguegs")]
        public ApiResult<List<SwaggerCodeGenItem>> ClientLanguegs(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ClientItemsCache : swagerCodeGen.ClientItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }

        /// <summary>
        /// 可生成的server
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        [HttpGet("ServerLanguegs")]
        [AllowAnonymous]
        [SwaggerOperation("ApiResource/ServerLanguegs")]
        public ApiResult<List<SwaggerCodeGenItem>> ServerLanguegs(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ServerItemsCache : swagerCodeGen.ServerItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }
        #endregion

        async Task<bool> exists(long id)
        {
            var query = db.ApiResources.AsQueryable();

            var ApiResourceIds = await userDb.UserApiResources
                .Where(x => x.UserId == UserId)
                .Select(x => x.ApiResourceId).ToListAsync();

            if (ApiResourceIds.Count > 0)
            {
                query = query.Where(x => ApiResourceIds.Contains(x.Id));
            }

            return query.Any(x => x.Id == id);
        }
    }
}