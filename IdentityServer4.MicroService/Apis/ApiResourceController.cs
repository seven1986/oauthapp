using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// API
    /// </summary>
    /// <remarks>为API提供版本管理、网关集成都功能。</remarks>
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("ApiResource")]
    [SwaggerTag("资源")]
    public class ApiResourceController : ApiControllerBase
    {
        //sql cache options
        readonly DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpiration = DateTimeOffset.MaxValue
        };

        #region Services
        //Database
        readonly ConfigurationDbContext configDb;
        readonly SwaggerCodeGenService swagerCodeGen;
        readonly AzureStorageService storageService;
        readonly EmailService email;
        readonly IDistributedCache cache;
        readonly IHttpContextAccessor accessor;
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
        /// <param name="_swagerCodeGen"></param>
        /// <param name="_storageService"></param>
        /// <param name="_email"></param>
        /// <param name="_provider"></param>
        /// <param name="_cache"></param>
        /// <param name="_accessor"></param>
        public ApiResourceController(
            ConfigurationDbContext _configDb,
            UserDbContext _userDb,
            IStringLocalizer<ApiResourceController> localizer,
            TenantService _tenantService,
            TenantDbContext _tenantDb,
            //RedisService _redis,
            SwaggerCodeGenService _swagerCodeGen,
            AzureStorageService _storageService,
            EmailService _email,
            IDataProtectionProvider _provider,
            IDistributedCache _cache,
            IHttpContextAccessor _accessor)
        {
            configDb = _configDb;
            db = _userDb;
            l = localizer;
            tenantDb = _tenantDb;
            tenantService = _tenantService;
            //redis = _redis;
            swagerCodeGen = _swagerCodeGen;
            storageService = _storageService;
            email = _email;
            protector = _provider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();

            cache = _cache;

            accessor = _accessor;
        }
        #endregion

        #region API
        #region API - 列表
        /// <summary>
        /// API - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.get")]
        [SwaggerOperation(
            OperationId = "ApiResourceGet", 
            Summary = "API - 列表",
            Description = "scope&permission：isms.apiresource.get")]
        public async Task<PagingResult<ApiResource>> Get([FromQuery]PagingRequest<ApiResourceGetRequest> value)
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

        #region API - 详情
        /// <summary>
        /// API - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.detail")]
        [SwaggerOperation(OperationId = "ApiResourceDetail",
            Summary = "API - 详情",
            Description = "scope&permission：isms.apiresource.detail")]
        public async Task<ApiResult<ApiResource>> Get(long id)
        {
            if (!await exists(id))
            {
                return new ApiResult<ApiResource>(l, BasicControllerEnums.NotFound);
            }

            var query = configDb.ApiResources.AsQueryable();

            var entity = await query
                .Where(x => x.Id == id)
                .Include(x => x.Scopes).ThenInclude(x => x.Scope)
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

        #region API - 创建
        /// <summary>
        /// API - 创建
        /// </summary>
        /// <param name="value">ID</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.post")]
        [SwaggerOperation(
            OperationId = "ApiResourcePost", 
            Summary = "API - 创建",
            Description = "scope&permission：isms.apiresource.post")]
        public ApiResult<long> Post([FromBody]ApiResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            configDb.Add(value);

            try
            {
                configDb.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<long>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = 0
                };
            }

            db.UserApiResources.Add(new AspNetUserApiResource()
            {
                ApiResourceId = value.Id,
                UserId = UserId
            });

            try
            {
                db.SaveChanges();
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

        #region API - 更新
        /// <summary>
        /// API - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.put")]
        [SwaggerOperation(
            OperationId = "ApiResourcePut",
            Summary = "API - 更新",
            Description = "scope&permission：isms.apiresource.put")]
        public async Task<ApiResult<bool>> Put([FromBody]ApiResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.Id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var Entity = configDb.ApiResources.Where(x => x.Id == value.Id)
                .Include(x => x.Scopes).ThenInclude(x => x.Scope)
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .FirstOrDefault();

            if (Entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            #region Name
            if (!string.IsNullOrWhiteSpace(value.Name) && !value.Name.Equals(Entity.Name))
            {
                Entity.Name = value.Name;
            }
            #endregion

            #region DisplayName
            if (!string.IsNullOrWhiteSpace(value.DisplayName) && !value.DisplayName.Equals(Entity.DisplayName))
            {
                Entity.DisplayName = value.DisplayName;
            }
            #endregion

            #region Description
            if (!string.IsNullOrWhiteSpace(value.Description) && !value.Description.Equals(Entity.Description))
            {
                Entity.Description = value.Description;
            }
            #endregion

            #region Enabled
            if (value.Enabled != Entity.Enabled)
            {
                Entity.Enabled = value.Enabled;
            }
            #endregion

            #region NonEditable
            if (value.NonEditable != Entity.NonEditable)
            {
                Entity.NonEditable = value.NonEditable;
            }
            #endregion

            #region LastAccessed
            if (value.LastAccessed != Entity.LastAccessed)
            {
                Entity.LastAccessed = value.LastAccessed;
            }
            #endregion

            #region Scopes
            if (value.Scopes != null && value.Scopes.Count > 0)
            {
                Entity.Scopes.Clear();

                value.Scopes.ForEach(x =>
                {
                    Entity.Scopes.Add(new ApiResourceScope()
                    {
                        ApiResource = value,
                        ApiResourceId = x.ApiResourceId,
                        Scope = x.Scope
                    });
                });
            }
            #endregion

            #region Secrets
            if (value.Secrets != null && value.Secrets.Count > 0)
            {
                Entity.Secrets.Clear();

                value.Secrets.ForEach(x =>
                {
                    Entity.Secrets.Add(new ApiResourceSecret()
                    {
                        ApiResource = value,
                        ApiResourceId = x.ApiResourceId,
                        Created = x.Created,
                        Description = x.Description,
                        Expiration = x.Expiration,
                        Type = x.Type,
                        Value = x.Value
                    });
                });
            }
            #endregion

            #region UserClaims
            if (value.UserClaims != null && value.UserClaims.Count > 0)
            {
                Entity.UserClaims.Clear();

                value.UserClaims.ForEach(x =>
                {
                    Entity.UserClaims.Add(new ApiResourceClaim()
                    {
                        ApiResource = value,
                        ApiResourceId = x.ApiResourceId,
                        Type = x.Type
                    });
                });
            }
            #endregion

            Entity.Updated = DateTime.UtcNow.AddHours(8);

            try
            {
                configDb.SaveChanges();
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

        #region API - 删除
        /// <summary>
        /// API - 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.delete")]
        [SwaggerOperation(
            OperationId = "ApiResourceDelete", 
            Summary = "API - 删除",
            Description = "scope&permission：isms.apiresource.delete")]
        public async Task<ApiResult<bool>> Delete(long id)
        {
            if (!await exists(id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var entity = configDb.ApiResources.Where(x => x.Id == id)
                .Include(x => x.Scopes).ThenInclude(x => x.Scope)
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .FirstOrDefault(); ;

            if (entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            configDb.ApiResources.Remove(entity);

            try
            {
                configDb.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = false
                };
            }

            #region relation
            var relation = db.UserApiResources.Where(x => x.UserId == UserId && x.ApiResourceId == id).FirstOrDefault();

            if (relation != null)
            {
                db.Remove(relation);

                try
                {
                    db.SaveChanges();
                }

                catch (Exception ex)
                {
                    return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                    {
                        data = false
                    };
                }
            } 
            #endregion

            return new ApiResult<bool>(true);
        }
        #endregion

        #region API - 导入
        /// <summary>
        /// API - 导入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Import")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "ApiResourceImport", 
            Summary = "API - 导入",
            Description ="")]
        public ApiResult<bool> Import([FromBody]ApiResourceImportRequest value)
        {
            var data = configDb.ApiResources.Where(x => x.Name.Equals(value.MicroServiceName))
                .Include(x => x.Scopes).FirstOrDefault();

            if (data == null)
            {
                var entity = new ApiResource()
                {
                    Name = value.MicroServiceName,
                    DisplayName = value.MicroServiceDisplayName,
                    Description = value.MicroServiceDescription,
                    Created = DateTime.UtcNow,
                    LastAccessed = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Enabled = true,
                    Scopes = new List<ApiResourceScope>(),
                };

                #region role、permission
                entity.UserClaims = new List<ApiResourceClaim>()
                {
                    new ApiResourceClaim()
                    {
                        ApiResource = entity,
                        Type = "role"
                    },
                    new ApiResourceClaim()
                    {
                        ApiResource = entity,
                        Type = "permission"
                    }
                };
                #endregion

                #region scopes
                if (value.MicroServicePolicies.Count > 0)
                {
                    value.MicroServicePolicies.ForEach(policy =>
                    {
                        policy.Scopes.ForEach(scope =>
                        {
                            var scopeName = $"{value.MicroServiceName}.{scope}";

                            entity.Scopes.Add(new ApiResourceScope()
                            {
                                ApiResource = entity,
                                Scope = scopeName
                            });
                        });

                        var scopeControllName = $"{value.MicroServiceName}.{policy.ControllerName}.all";

                        entity.Scopes.Add(new ApiResourceScope()
                        {
                            ApiResource = entity,
                            Scope = scopeControllName
                        });
                    });
                }

                var scopeApiResourceName = $"{value.MicroServiceName}.all";

                entity.Scopes.Add(new ApiResourceScope()
                {
                    ApiResource = entity,
                    Scope = scopeApiResourceName
                });
                #endregion

                configDb.Add(entity);

                configDb.SaveChanges();

                #region update user permission for new ApiResource
                var userClaims = db.UserClaims.Where(x => x.ClaimType.Equals("permission") &&
                        !x.ClaimValue.Contains(scopeApiResourceName)).ToList();

                if (userClaims.Count > 0)
                {
                    userClaims.ForEach(x =>
                    {
                        var permissions = x.ClaimValue.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (!permissions.Contains(scopeApiResourceName))
                        {
                            permissions.Add(scopeApiResourceName);

                            x.ClaimValue = string.Join(",", permissions);
                        }
                    });

                    db.SaveChanges();
                }
                #endregion
            }

            else
            {
                data.DisplayName = value.MicroServiceDisplayName;

                data.Description = value.MicroServiceDescription;

                data.Updated = DateTime.UtcNow;

                data.Scopes.Clear();

                value.MicroServicePolicies.ForEach(policy =>
                {
                    policy.Scopes.ForEach(scope =>
                    {
                        var scopeName = $"{value.MicroServiceName}.{scope}";

                        data.Scopes.Add(new ApiResourceScope()
                        {
                            ApiResource = data,
                            Scope = scopeName
                        });
                    });

                    var scopeControllName = $"{value.MicroServiceName}.{policy.ControllerName}.all";

                    data.Scopes.Add(new ApiResourceScope()
                    {
                        ApiResource = data,
                        Scope = scopeControllName
                    });
                });

                var scopeApiResourceName = $"{value.MicroServiceName}.all";

                data.Scopes.Add(new ApiResourceScope()
                {
                    ApiResource = data,
                    Scope = scopeApiResourceName
                });

                configDb.SaveChanges();
            }

            #region redirectUrl&scope for client-swagger
            value.MicroServiceClientIDs.ForEach(clientId =>
            {
                var client = configDb.Clients
                .Where(x => x.ClientName.Equals(clientId))
                .Include(x => x.RedirectUris)
                .Include(x => x.AllowedScopes).FirstOrDefault();

                #region client redirectUrls
                value.MicroServiceRedirectUrls.ForEach(redirectUrl =>
                {
                    var redirectUrlItem = client.RedirectUris
                    .Where(x => x.RedirectUri.Equals(redirectUrl)).FirstOrDefault();

                    if (redirectUrlItem == null)
                    {
                        client.RedirectUris.Add(new ClientRedirectUri()
                        {
                            RedirectUri = redirectUrl,
                            Client = client
                        });
                    }
                });
                #endregion

                #region client scope
                var scope = $"{value.MicroServiceName}.all";
                var scopeItem = client.AllowedScopes.FirstOrDefault(x => x.Scope.Equals(scope));
                if (scopeItem == null)
                {
                    client.AllowedScopes.Add(new ClientScope()
                    {
                        Client = client,
                        Scope = scope
                    });
                }
                #endregion

                configDb.SaveChanges();
            });
            #endregion

            return new ApiResult<bool>(true);
        }
        #endregion

        #region API - 权限代码
        /// <summary>
        /// API - 权限代码
        /// </summary>
        /// <returns></returns>
        [HttpGet("Scopes")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.scopes")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.scopes")]
        [SwaggerOperation(
            OperationId = "ApiResourceScopes", 
            Summary = "API - 权限代码",
            Description = "scope&permission：isms.apiresource.scopes")]
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

        #region API - 错误码表
        /// <summary>
        /// API - 错误码表
        /// </summary>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "ApiResourceCodes", 
            Summary = "API - 错误码表",
            Description = "API代码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<ApiResourceControllerEnums>();

            return result;
        }
        #endregion
        #endregion

        #region API - 网关
        #region API - 网关 - 发布或更新版本
        /// <summary>
        /// API - 网关 - 发布或更新版本
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}/Publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.publish")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.publish")]
        [SwaggerOperation(
            OperationId = "ApiResourcePublish",
            Summary = "API - 网关 - 发布或更新版本",
            Description = "scope&permission：isms.apiresource.publish")]
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

            // 更新API策略
            if (result.IsSuccessStatusCode)
            {
                #region CacheConfigurations
                try
                {
                    var publishKey = $"ApiResource:Publish:{id}";

                    await cache.SetStringAsync(publishKey, JsonConvert.SerializeObject(value), cacheOptions);

                    //redis.SetAsync(publishKey, JsonConvert.SerializeObject(value), null);
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

        #region API - 网关 - 创建修订版
        /// <summary>
        /// API - 网关 - 创建修订版
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/PublishRevision")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.publishrevision")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.publishrevision")]
        [SwaggerOperation(
            OperationId = "ApiResourcePublishRevision", 
            Summary = "API - 网关 - 创建修订版",
            Description = "scope&permission：isms.apiresource.publishrevision")]
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

        #region API - 网关 - 创建新版本
        /// <summary>
        /// API - 网关 - 创建新版本
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/PublishVersion")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.publishversion")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.publishversion")]
        [SwaggerOperation(
            OperationId = "ApiResourcePublishVersion", 
            Summary = "API - 网关 - 创建新版本",
            Description = "scope&permission：isms.apiresource.publishversion")]
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

        #region API - 网关 - 上次发布配置
        /// <summary>
        /// API - 网关 - 上次发布配置
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/PublishConfiguration")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.publishconfiguration")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.publishconfiguration")]
        [SwaggerOperation(
            OperationId = "ApiResourcePublishConfiguration", 
            Summary = "API - 网关 - 上次发布配置",
            Description = "scope&permission：isms.apiresource.publishconfiguration")]
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

            var resultCache = await cache.GetStringAsync(publishKey); //redis.GetAsync(publishKey);

            if (!string.IsNullOrWhiteSpace(resultCache))
            {
                result = JsonConvert.DeserializeObject<ApiResourcePublishRequest>(resultCache);
            }

            return result;
        }
        #endregion

        #region API - 网关 - 版本列表
        /// <summary>
        /// API - 网关 - 版本列表
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/Versions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.versions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.versions")]
        [SwaggerOperation(
            OperationId = "ApiResourceVersions", 
            Summary = "API - 网关 - 版本列表",
            Description = "scope&permission：isms.apiresource.versions")]
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

        #region API - 网关 - 上线指定版本
        /// <summary>
        /// API - 网关 - 上线指定版本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="revisionId"></param>
        /// <returns></returns>
        [HttpPost("{id}/Versions/{revisionId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.setonlineversion")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.setonlineversion")]
        [SwaggerOperation(
            OperationId = "ApiResourceSetOnlineVersion",
            Summary = "API - 网关 - 上线指定版本",
            Description = "scope&permission：isms.apiresource.setonlineversion")]
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

        #region API - 网关 - OAuthServers
        /// <summary>
        /// API - 网关 - OAuthServers
        /// </summary>
        /// <returns></returns>
        [HttpGet("AuthServers")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.authservers")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.authservers")]
        [SwaggerOperation(
            OperationId = "ApiResourceAuthServers", 
            Summary = "API - 网关 - OAuthServers",
            Description = "scope&permission：isms.apiresource.authservers")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>> AuthServers()
        {
            var result = await AzureApim.AuthorizationServers.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementAuthorizationServerEntity>>(result);
        }
        #endregion

        #region API - 网关 - 产品包列表
        /// <summary>
        /// API - 网关 - 产品包列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Products")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.products")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.products")]
        [SwaggerOperation(
            OperationId = "ApiResourceProducts", 
            Summary = "API - 网关 - 产品包列表",
            Description = "scope&permission：isms.apiresource.products")]
        public async Task<ApiResult<AzureApiManagementEntities<AzureApiManagementProductEntity>>> Products()
        {
            var result = await AzureApim.Products.GetAsync();

            return new ApiResult<AzureApiManagementEntities<AzureApiManagementProductEntity>>(result);
        }
        #endregion
        #endregion

        #region API - 修订内容
        #region API - 修订内容 - 列表
        /// <summary>
        /// API - 修订内容 - 列表
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="apiId">Api的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/Releases")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.releases")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.releases")]
        [SwaggerOperation(
            OperationId = "ApiResourceReleases", 
            Summary = "API - 修订内容 - 列表",
            Description = "scope&permission：isms.apiresource.releases")]
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

        #region API - 修订内容 - 发布
        /// <summary>
        /// API - 修订内容 - 发布
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/Releases")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.postrelease")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.postrelease")]
        [SwaggerOperation(
            OperationId = "ApiResourcePostRelease", 
            Summary = "API - 修订内容 - 发布",
            Description = "scope&permission：isms.apiresource.postrelease")]
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

        #region API - 修订内容 - 更新
        /// <summary>
        /// API - 修订内容 - 更新
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="releaseId">修订内容的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}/Releases/{releaseId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.putrelease")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.putrelease")]
        [SwaggerOperation(
            OperationId = "ApiResourcePutRelease", 
            Summary = " API - 修订内容 - 更新",
            Description = "scope&permission：isms.apiresource.putrelease")]
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

        #region API - 修订内容 - 删除
        /// <summary>
        /// API - 修订内容 - 删除
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="releaseId">修订内容的ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/Releases/{releaseId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.deleterelease")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.deleterelease")]
        [SwaggerOperation(
            OperationId = "ApiResourceDeleteRelease", 
            Summary = "API - 修订内容 - 删除",
            Description = "scope&permission：isms.apiresource.deleterelease")]
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

        #region API - 订阅者
        #region API - 订阅者 - 列表
        /// <summary>
        /// API - 订阅者 - 列表
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/Subscriptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.subscriptions")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.subscriptions")]
        [SwaggerOperation(
            OperationId = "ApiResourceSubscriptions", 
            Summary = "API - 订阅者 - 列表",
            Description = "scope&permission：isms.apiresource.subscriptions")]
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

        #region API - 订阅者 - 添加
        /// <summary>
        /// API - 订阅者 - 添加
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="code">邮箱校验加密字符串</param>
        /// <returns></returns>
        [HttpGet("{id}/AddSubscription")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "ApiResourceAddSubscription", 
            Summary = "API - 订阅者 - 添加",
            Description ="")]
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

        #region API - 订阅者 - 取消
        /// <summary>
        /// API - 订阅者 - 取消
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="code">邮箱校验加密字符串</param>
        /// <returns></returns>
        [HttpGet("{id}/DelSubscription")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "ApiResourceDelSubscription", Summary = "API - 订阅者 - 取消",Description ="")]
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

        #region API - 订阅者 - 验证邮箱
        /// <summary>
        /// API - 订阅者 - 验证邮箱
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/Subscriptions/VerifyEmail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.verifyemail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.verifyemail")]
        [SwaggerOperation(
            OperationId = "ApiResourceVerifyEmail", 
            Summary = "API - 订阅者 - 验证邮箱",
            Description = "scope&permission：isms.apiresource.verifyemail")]
        public async Task<ApiResult<bool>> VerifyEmail(long id, [FromBody]ApiResourceSubscriptionsVerifyEmailRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region API是否存在
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

                var result = await email.SendEmailAsync("verify_apiresource_subscription", "验证邮箱",
                   //SendCloudMailTemplates.verify_apiresource_subscription,
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

        #region API - 包市场
        #region API - 包市场 - 列表
        /// <summary>
        /// API - 包市场 - 列表
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <returns></returns>
        [HttpGet("{id}/Packages")]
        [SwaggerOperation(
            OperationId = "ApiResourcePackages", 
            Summary = "API - 包市场 - 列表",
            Description = "scope&permission：isms.apiresource.packages")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.packages")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.packages")]
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

        #region API - 包市场 - 添加
        /// <summary>
        /// API - 包市场 - 添加
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/Packages")]
        [SwaggerOperation(
            OperationId = "ApiResourcePostPackage", 
            Summary = "API - 包市场 - 添加",
            Description = "scope&permission：isms.apiresource.postpackages")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.postpackages")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.postpackages")]
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

        #region API - 包市场 - 删除
        /// <summary>
        /// API - 包市场 - 删除
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="packageId">包的ID</param>
        /// <returns></returns>
        [HttpDelete("{id}/Packages/{packageId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.deletepackage")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.deletepackage")]
        [SwaggerOperation(
            OperationId = "ApiResourceDeletePackage", 
            Summary = "API - 包市场 - 删除",
            Description = "scope&permission：isms.apiresource.deletepackage")]
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

        #region API - 包市场 - 更新
        /// <summary>
        /// API - 包市场 - 更新
        /// </summary>
        /// <param name="id">API的ID</param>
        /// <param name="packageId">包的ID</param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut("{id}/Packages/{packageId}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiresource.putpackage")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiresource.putpackage")]
        [SwaggerOperation(
            OperationId = "ApiResourcePutPackage", 
            Summary = "API - 包市场 - 更新",
            Description = "scope&permission：isms.apiresource.deletepackage")]
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