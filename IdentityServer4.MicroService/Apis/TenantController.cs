using System;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Models.Shared;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.TenantController;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    // Tenant 根据 OwnerUserId 来获取列表、或详情、增删改

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
        public async Task<ApiResult<long>> Post([FromBody]AppTenant value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            value.OwnerUserId = UserId;

            db.Add(value);

            await db.SaveChangesAsync();

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
        public async Task<ApiResult<long>> Put([FromBody]AppTenant value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            using (var tran = tenantDb.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    value.OwnerUserId = UserId;
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    tenantDb.Update(value);
                    tenantDb.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await tenantDb.Tenants.Where(x => x.Id == value.Id)
                                     .Include(x => x.Hosts)
                                     .Include(x => x.Claims)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
                    #endregion

                    #region Update Entity.Claims
                    if (value.Claims != null && value.Claims.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Claims.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Claims.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE AppTenantClaims WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                tenantDb.Database.ExecuteSqlRaw($"DELETE AppTenantClaims WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                tenantDb.Database.ExecuteSqlRaw($"UPDATE AppTenantClaims SET [ClaimType]={x.ClaimType},[ClaimValue]={x.ClaimValue} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                tenantDb.Database.ExecuteSqlRaw($"INSERT INTO AppTenantClaims VALUES ({x.ClaimType},{x.ClaimValue},{source.Id})");
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Properties
                    if (value.Properties != null && value.Properties.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Properties.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Properties.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE AppTenantProperties WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                tenantDb.Database.ExecuteSqlRaw($"DELETE AppTenantProperties WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Properties.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                tenantDb.Database.ExecuteSqlRaw($"UPDATE AppTenantProperties SET [Key]={x.Key},[Value]={x.Value} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Properties.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                tenantDb.Database.ExecuteSqlRaw($"INSERT INTO AppTenantProperties VALUES ({x.Key},{x.Value},{source.Id})");
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Hosts
                    if (value.Hosts != null && value.Hosts.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Hosts.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Hosts.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE AppTenantHosts WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                tenantDb.Database.ExecuteSqlRaw($"DELETE AppTenantHosts WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Hosts.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                tenantDb.Database.ExecuteSqlRaw($"UPDATE AppTenantHosts SET [HostName]= {x.HostName} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Hosts.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                tenantDb.Database.ExecuteSqlRaw($"INSERT INTO AppTenantHosts VALUES ({source.Id},{x.HostName})");
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
        public async Task<ApiResult<long>> Delete(int id)
        {
            var entity = await tenantDb.Tenants.FirstOrDefaultAsync(x => x.OwnerUserId == UserId && x.Id == id);

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            tenantDb.Tenants.Remove(entity);

            await tenantDb.SaveChangesAsync();

            tenantDb.Database.ExecuteSqlRaw($"DELETE AspNetUserTenants WHERE AppTenantId = {id}");

            return new ApiResult<long>(id);
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
