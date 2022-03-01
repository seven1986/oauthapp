using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OAuthApp.Tenant
{
    public class TenantProvider
    {
        readonly IMemoryCache _cache;

        readonly TenantDbContext _db;

        readonly TenantDbCreator _dbCreator;

        public TenantProvider(TenantDbContext db,IMemoryCache cache, TenantDbCreator dbCreator)
        {
            _db = db;
            _cache = cache;
            _dbCreator = dbCreator;
        }

        public MemoryCacheEntryOptions CacheEntryOptions(double duration)
        {
            var MemoryCacheOptions = new MemoryCacheEntryOptions();

            MemoryCacheOptions.SetAbsoluteExpiration(
                TimeSpan.FromSeconds(duration));

            return MemoryCacheOptions;
        }

        public TenantContext GetTenantContext(string host)
        {
            var _tenantContext = _cache.Get<TenantContext>(TenantDefaults.CacheKey);

            if (_tenantContext == null)
            {
                _tenantContext = CreateTenantIfNotExists(host);

                var cacheOptions = CacheEntryOptions(TenantDefaults.ReflushDuration);

                _cache.Set(TenantDefaults.CacheKey, _tenantContext, cacheOptions);
            }

            return _tenantContext;
        }

        TenantContext CreateTenantIfNotExists(string host)
        {
            TenantContext result = null;

            var tenantId = _db.TenantHosts.Where(x => x.HostName.Equals(host))
                    .Select(x => x.TenantID).FirstOrDefault();

            if (tenantId > 0)
            {
                result = _db.Tenants.Where(x => x.ID == tenantId)
                    .Select(x => new TenantContext
                    {
                        ClaimsIssuer = x.ClaimsIssuer,
                        Description = x.Description,
                        LogoUri = x.LogoUri,
                        Name = x.Name,
                        OwnerId = x.OwnerId,
                        Id = x.ID
                    }).FirstOrDefault();

                if(result!=null)
                {
                    result.Claims = _db.TenantClaims.Where(q => q.TenantID == tenantId)
                        .ToDictionary(k => k.ClaimType, v => v.ClaimValue);

                    result.Properties = _db.TenantProperties.Where(q => q.TenantID == tenantId)
                        .ToDictionary(k => k.Name, v => v.Value);
                }
            }

            if (result==null)
            {
                var tenant = new Tenant()
                {
                    Name = host,
                    ClaimsIssuer = host,
                    CacheDuration = TenantDefaults.ReflushDuration,
                    OwnerHost = host,
                    OwnerId = 1,
                    DatabaseMaxSize = TenantDefaults.DatabaseMaxSize,
                    AppServerMaxSize = TenantDefaults.DatabaseMaxSize,
                    BlobServerMaxSize = TenantDefaults.DatabaseMaxSize,
                    ReleaseServerMaxSize = TenantDefaults.DatabaseMaxSize
                };

                _db.Tenants.Add(tenant);
                _db.SaveChanges();

                _db.TenantHosts.Add(new TenantHost() { TenantID = tenant.ID, HostName = host });
                _db.SaveChanges();

                //_db.TenantProperties.Add(new TenantProperty() 
                //{
                //    Name = ServerConst.BlobServer, 
                //    TenantID = tenant.ID,
                //    Value = ServerConst.BlobServerID
                //});
                //_db.TenantProperties.Add(new TenantProperty()
                //{
                //    Name = ServerConst.ReleaseServer,
                //    TenantID = tenant.ID,
                //    Value = ServerConst.ReleaseServerID
                //});
                //_db.SaveChanges();

                result = new TenantContext()
                {
                    Name = host,
                    ClaimsIssuer = host,
                    Claims = new Dictionary<string, string>(),
                    Properties = new Dictionary<string, string>()
                };

                _dbCreator.EnsureCreated();
            }
            return result;
        }
    }
}