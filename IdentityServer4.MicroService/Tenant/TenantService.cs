using IdentityServer4.MicroService.Mappers;
using IdentityServer4.MicroService.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantService
    {
        IMemoryCache _cache;

        public TenantService(
            IMemoryCache cache)
        {
            _cache = cache;
        }

        public Tuple<TenantPublicModel, TenantPrivateModel> GetTenant(TenantDbContext _db, string host)
        {
            #region 设置缓存Key
            // for client use
            var Unique_TenantPublic_CacheKey = $"{TenantConstant.CacheKey}:{host}:pub";

            // for server config
            var Unique_TenantPrivate_CacheKey = $"{TenantConstant.CacheKey}:{host}:pvt";
            #endregion

            #region 根据缓存Key获取数据
            var tenant_public = _cache.Get<TenantPublicModel>(Unique_TenantPublic_CacheKey);
            var tenant_private = _cache.Get<TenantPrivateModel>(Unique_TenantPrivate_CacheKey);
            #endregion

            // 详情和Issuer
            if (tenant_public == null ||
                tenant_private == null)
            {
                var _tenantId = _db.ExecuteScalarAsync(
                    "SELECT AppTenantId FROM AppTenantHosts WHERE HostName = @HostName", System.Data.CommandType.Text,
                    new SqlParameter("@HostName", host)).Result;

                if (_tenantId != null)
                {
                    var tenantId = long.Parse(_tenantId.ToString());

                    var tenant = _db.Tenants
                        .Include(x => x.Claims)
                        .Include(x => x.Hosts)
                        .Include(x => x.Properties)
                        .FirstOrDefault(x => x.Id == tenantId);

                    tenant_public = tenant.ToPublicModel();

                    tenant_private = tenant.ToPrivateModel();

                    _cache.Set(Unique_TenantPublic_CacheKey,
                        tenant_public,
                        TimeSpan.FromSeconds(tenant.CacheDuration));

                    _cache.Set(Unique_TenantPrivate_CacheKey,
                        tenant_private,
                        TimeSpan.FromSeconds(tenant.CacheDuration));
                }
            }

            return Tuple.Create(tenant_public, tenant_private);
        }
    }
}
