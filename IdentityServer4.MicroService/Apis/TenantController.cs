using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.TenantController;
using IdentityServer4.MicroService.Models.Shared;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
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
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// 租户
    /// </summary>
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Tenant")]
    [SwaggerTag("租户")]
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
            Description = "scope&permission：isms.tenant.get")]
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
            Description = "scope&permission：isms.tenant.detail")]
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
            Description = "scope&permission：isms.tenant.post")]
        public ApiResult<long> Post([FromBody]AppTenant value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            value.OwnerUserId = UserId;

            db.Add(value);

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
            Description = "scope&permission：isms.tenant.put")]
        public ApiResult<bool> Put([FromBody]AppTenant value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var Entity = tenantDb.Tenants.Where(x => x.Id == value.Id && x.OwnerUserId == UserId)
                .Include(x => x.Hosts)
                .Include(x => x.Claims)
                .Include(x => x.Properties).FirstOrDefault();

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

            #region Theme
            if (!string.IsNullOrWhiteSpace(value.Theme) && !value.Theme.Equals(Entity.Theme))
            {
                Entity.Theme = value.Theme;
            }
            #endregion

            #region IdentityServerIssuerUri
            if (!string.IsNullOrWhiteSpace(value.IdentityServerIssuerUri) && !value.IdentityServerIssuerUri.Equals(Entity.IdentityServerIssuerUri))
            {
                Entity.IdentityServerIssuerUri = value.IdentityServerIssuerUri;
            }
            #endregion

            #region Status
            if (value.Status != Entity.Status)
            {
                Entity.Status = value.Status;
            }
            #endregion

            #region CacheDuration
            if (value.CacheDuration != Entity.CacheDuration)
            {
                Entity.CacheDuration = value.CacheDuration;
            } 
            #endregion

            #region Hosts
            if (Entity.Hosts.Count > 0)
            {
                tenantDb.TenantHosts.RemoveRange(Entity.Hosts);
            }

            if (value.Hosts != null && value.Hosts.Count > 0)
            {
                Entity.Hosts.Clear();

                value.Hosts.ForEach(x =>
                {
                    Entity.Hosts.Add(new AppTenantHost()
                    {
                        HostName = x.HostName,
                        AppTenantId = value.Id
                    });
                });
            }
            #endregion

            #region Claims
            if (Entity.Claims.Count > 0)
            {
                tenantDb.TenantClaims.RemoveRange(Entity.Claims);
            }

            if (value.Claims != null && value.Claims.Count > 0)
            {
                Entity.Claims.Clear();

                value.Claims.ForEach(x =>
                {
                    Entity.Claims.Add(new AppTenantClaim()
                    {
                        ClaimType = x.ClaimType,
                        AppTenantId = value.Id,
                        ClaimValue = x.ClaimValue,
                    });
                });
            }
            #endregion

            #region Properties
            if (Entity.Properties.Count > 0)
            {
                tenantDb.TenantProperties.RemoveRange(Entity.Properties);
            }

            if (value.Properties != null && value.Properties.Count > 0)
            {
                Entity.Properties.Clear();

                value.Properties.ForEach(x =>
                {
                    Entity.Properties.Add(new AppTenantProperty()
                    {
                        Key = x.Key,
                        Value = x.Value,
                        AppTenantId = value.Id
                    });
                });
            }
            #endregion

            Entity.LastUpdateTime = DateTime.UtcNow.AddHours(8);

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
            Description = "scope&permission：isms.tenant.delete")]
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
            var entity = tenantService.GetTenant(tenantDb, host);

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
