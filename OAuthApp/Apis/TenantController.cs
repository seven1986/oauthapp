using OAuthApp.Enums;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.TenantController;
using OAuthApp.Models.Shared;
using OAuthApp.Services;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// 租户
    /// </summary>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Tenant")]
    [SwaggerTag("#### 租户管理")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TenantController : ApiControllerBase
    {
        #region 构造函数
        public TenantController(
            TenantDbContext _db,
            RedisService _redis,
            IStringLocalizer<TenantController> _localizer,
            TenantService _tenantService
            )
        {
            // 多语言
            l = _localizer;
            redis = _redis;
            tenantDb = _db;
            tenantService = _tenantService;
        }
        #endregion

        #region 租户 - 列表
        /// <summary>
        /// 租户 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:tenant.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:tenant.get")]
        [SwaggerOperation(OperationId = "TenantGet",
            Summary = "租户 - 列表",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.tenant.get | oauthapp.tenant.get |")]
        public async Task<PagingResult<AppTenant>> Get([FromQuery]PagingRequest<TenantGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<AppTenant>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = tenantDb.Tenants.AsQueryable();

            query = query.Where(x => x.OwnerUserId == UserId);

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.Host))
            {
                query = query.Where(x => x.Hosts.Any(h => h.HostName.Equals(value.q.Host)));
            }
            #endregion

            #region total
            var result = new PagingResult<AppTenant>()
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
                            .Include(x => x.Claims)
                            .Include(x => x.Hosts)
                            .Include(x => x.Properties)
                            .ToListAsync();
                #endregion

                if (data.Count > 0)
                {
                    result.data = data;
                }
            }

            return result;
        }
        #endregion

        #region 租户 - 详情
        /// <summary>
        /// 租户 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:tenant.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:tenant.detail")]
        [SwaggerOperation(OperationId = "TenantDetail",
            Summary = "租户 - 详情",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.tenant.detail | oauthapp.tenant.detail |")]
        public async Task<ApiResult<AppTenant>> Get(int id)
        {
            var query = tenantDb.Tenants.AsQueryable();

            query = query.Where(x => x.OwnerUserId == UserId);

            var entity = await query
                .Include(x => x.Hosts)
                .Include(x => x.Claims)
                .Include(x => x.Properties)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<AppTenant>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<AppTenant>(entity);
        }
        #endregion

        #region 租户 - 创建
        /// <summary>
        /// 租户 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:tenant.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:tenant.post")]
        [SwaggerOperation(OperationId = "TenantPost",
            Summary = "租户 - 创建",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.tenant.post | oauthapp.tenant.post |")]
        public ApiResult<long> Post([FromBody]AppTenant value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            value.OwnerUserId = UserId;

            tenantDb.Add(value);

            try
            {
                tenantDb.SaveChanges();
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

        #region 租户 - 更新
        /// <summary>
        /// 租户 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:tenant.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:tenant.put")]
        [SwaggerOperation(OperationId = "TenantPut",
            Summary = "租户 - 更新",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.tenant.put | oauthapp.tenant.put |")]
        public ApiResult<bool> Put([FromBody]AppTenant value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (value.OwnerUserId != UserId)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            var Entity = tenantDb.Tenants.Find(value.Id);

            if (Entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            Entity.CacheDuration = value.CacheDuration;
            Entity.CreateDate = value.CreateDate;
            Entity.LastUpdateTime = value.LastUpdateTime;
            Entity.Status = value.Status;

            if (!string.IsNullOrWhiteSpace(value.IdentityServerIssuerUri) &&
                !value.IdentityServerIssuerUri.Equals(Entity.IdentityServerIssuerUri))
            {
                Entity.IdentityServerIssuerUri = value.IdentityServerIssuerUri;
            }

            if (!string.IsNullOrWhiteSpace(value.Name) &&
                !value.Name.Equals(Entity.Name))
            {
                Entity.Name = value.Name;
            }

            if (!string.IsNullOrWhiteSpace(value.Theme) &&
                !value.Theme.Equals(Entity.Theme))
            {
                Entity.Theme = value.Theme;
            }

            if (!string.IsNullOrWhiteSpace(value.LogoUri) &&
              !value.LogoUri.Equals(Entity.LogoUri))
            {
                Entity.LogoUri = value.LogoUri;
            }

            if (!string.IsNullOrWhiteSpace(value.Description) &&
              !value.Description.Equals(Entity.Description))
            {
                Entity.Description = value.Description;
            }

            #region Claims
            var Claims = tenantDb.TenantClaims.Where(x => x.AppTenantId == value.Id).ToList();

            if (Claims != null && Claims.Count > 0)
            {
                tenantDb.TenantClaims.RemoveRange(Claims);
            }

            if (value.Claims != null && value.Claims.Count > 0)
            {
                value.Claims.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.ClaimType))
                    {
                        tenantDb.TenantClaims.Add(new AppTenantClaim()
                        {
                            ClaimType = x.ClaimType,
                            ClaimValue = x.ClaimValue,
                            AppTenantId = value.Id
                        });
                    }
                });
            }
            #endregion

            #region Properties
            var Properties = tenantDb.TenantProperties.Where(x => x.AppTenantId == value.Id).ToList();

            if (Properties != null && Properties.Count > 0)
            {
                tenantDb.TenantProperties.RemoveRange(Properties);
            }
            if (value.Properties != null && value.Properties.Count > 0)
            {
                value.Properties.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Key))
                    {
                        tenantDb.TenantProperties.Add(new AppTenantProperty()
                        {
                            Key = x.Key,
                            Value = x.Value,
                            AppTenantId = value.Id
                        });
                    }
                });
            }
            #endregion

            #region Hosts
            var Hosts = tenantDb.TenantHosts.Where(x => x.AppTenantId == value.Id).ToList();

            if (Hosts != null && Hosts.Count > 0)
            {
                tenantDb.TenantHosts.RemoveRange(Hosts);
            }
            if (value.Hosts != null && value.Hosts.Count > 0)
            {
                value.Hosts.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.HostName))
                    {
                        tenantDb.TenantHosts.Add(new AppTenantHost()
                        {
                            HostName = x.HostName,
                            AppTenantId = value.Id
                        });
                    }
                });
            }
            #endregion

            try
            {
                tenantDb.SaveChanges();
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

        #region 租户 - 删除
        /// <summary>
        /// 租户 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:tenant.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:tenant.delete")]
        [SwaggerOperation(OperationId = "TenantDelete",
            Summary = "租户 - 删除",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.tenant.delete | oauthapp.tenant.delete |")]
        public ApiResult<bool> Delete(int id)
        {
            var entity = tenantDb.Tenants.Where(x => x.OwnerUserId == UserId && x.Id == id)
                .Include(x => x.Claims)
                .Include(x => x.Properties)
                .Include(x => x.Hosts)
                .FirstOrDefault();

            if (entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            tenantDb.Tenants.Remove(entity);

            try
            {
                tenantDb.SaveChanges();
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

        #region 租户 - 详情（公共）
        /// <summary>
        /// 租户 - 详情（公共）
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        [HttpGet("Info")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "TenantInfo",
            Summary = "租户 - 详情（公共）",
            Description = "")]
        public ApiResult<TenantPublicModel> Info(string host)
        {
            var entity = tenantService.GetTenant(host);

            if (entity == null)
            {
                return new ApiResult<TenantPublicModel>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<TenantPublicModel>(entity.Item1);
        }
        #endregion

        #region 租户 - 错误码表
        /// <summary>
        /// 租户 - 错误码表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "TenantCodes",
            Summary = "租户 - 错误码表",
            Description = "租户 - 错误码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<TenantControllerEnums>();

            return result;
        }
        #endregion
    }
}
