using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Mappers;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantService
    {
        RedisService _redis;

        public TenantService(
            RedisService redis)
        {
            _redis = redis;
        }

        public Tuple<string, string> GetTenant(TenantDbContext _db,string host)
        {
            #region 设置缓存Key
            // for client use
            var Unique_TenantPublic_CacheKey = $"{TenantConstant.CacheKey}:{host}:pub";
            
            // for server config
            var Unique_TenantPrivate_CacheKey = $"{TenantConstant.CacheKey}:{host}:pvt";
            #endregion

            #region 根据缓存Key获取数据
            var tenant_public = _redis.Get(Unique_TenantPublic_CacheKey).Result;
            var tenant_private = _redis.Get(Unique_TenantPrivate_CacheKey).Result;
            #endregion

            // 详情和Issuer
            if (string.IsNullOrWhiteSpace(tenant_public) ||
                string.IsNullOrWhiteSpace(tenant_private))
            {
                var tenantId = _db.TenantHosts.Where(x => x.HostName.Equals(host))
                    .Select(x => x.AppTenantId)
                    .FirstOrDefault();

                if (tenantId > 0)
                {
                    var tenant = _db.Tenants
                        .Include(x => x.Claims)
                        .Include(x => x.Hosts)
                        .Include(x => x.Properties)
                        .FirstOrDefault(x => x.Id == tenantId);

                    tenant_public = JsonConvert.SerializeObject(tenant.ToPublicModel());

                    tenant_private = JsonConvert.SerializeObject(tenant.ToPrivateModel());

                    var cacheResult = _redis.Set(Unique_TenantPublic_CacheKey,
                        tenant_public,
                        TimeSpan.FromSeconds(tenant.CacheDuration)).Result;

                    cacheResult = _redis.Set(Unique_TenantPrivate_CacheKey,
                        tenant_private,
                        TimeSpan.FromSeconds(tenant.CacheDuration)).Result;
                }
            }

            return Tuple.Create(tenant_public, tenant_private);
        }
    }
}
