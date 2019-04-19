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

        public MemoryCacheEntryOptions CacheEntryOptions(double duration)
        {
            var options = new MemoryCacheEntryOptions();

            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(duration));

            return options;
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
                var _tenantId = _db.TenantHosts.Where(x => x.HostName.Equals(host))
                    .Select(x => x.AppTenantId).FirstOrDefault();

                if (_tenantId > 0)
                {
                    var tenant = _db.Tenants
                        .Include(x => x.Claims).AsNoTracking()
                        .Include(x => x.Hosts).AsNoTracking()
                        .Include(x => x.Properties).AsNoTracking()
                        .Where(x => x.Id == _tenantId).AsNoTracking()
                        .FirstOrDefault();

                    tenant_public = tenant.ToPublicModel();

                    tenant_private = tenant.ToPrivateModel();

                    var cacheOptions = CacheEntryOptions(tenant.CacheDuration);

                    _cache.Set(Unique_TenantPublic_CacheKey, tenant_public, cacheOptions);

                    _cache.Set(Unique_TenantPrivate_CacheKey, tenant_private, cacheOptions);
                }
            }

            return Tuple.Create(tenant_public, tenant_private);
        }
    }
}
