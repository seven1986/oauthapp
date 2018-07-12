using System.Threading.Tasks;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantMiddleware
    {
        readonly RequestDelegate _next;
        readonly TenantService _tenantService;
        readonly IAuthenticationSchemeProvider _oauthProvider;
        readonly IMemoryCache _memoryCache;
        readonly IdentityServerOptions _identityServerOptions;

        public TenantMiddleware(
            RequestDelegate next,
            Lazy<TenantService> tenantService,
            Lazy<IAuthenticationSchemeProvider> oauthProvider,
            Lazy<IMemoryCache> memoryCache,
            Lazy<IdentityServerOptions> identityServerOptions
            )
        {
            _next = next;
            _tenantService = tenantService.Value;
            _oauthProvider = oauthProvider.Value;
            _memoryCache = memoryCache.Value;
            _identityServerOptions = identityServerOptions.Value;
        }

        public Task Invoke(
            HttpContext context,
            TenantDbContext _db)
        {
            var tenant = _tenantService.GetTenant(_db,
                context.Request.Host.Value);

            if (tenant.Item1 != null)
            {
                context.Items[TenantConstant.CacheKey] = tenant.Item1;
            }

            var reflushFlagCacheKey = TenantConstant.SchemesReflush + context.Request.Host.Value;

            var reflushFlag = _memoryCache.Get<string>(reflushFlagCacheKey);

            if (tenant.Item2 != null)
            {
                var pvtModel = tenant.Item2;

                #region IssuerUri
                _identityServerOptions.IssuerUri = context.Request.Scheme + "://" + tenant.Item2.IdentityServerIssuerUri;
                #endregion

                #region ResetOAuthOptions
                if (string.IsNullOrWhiteSpace(reflushFlag) && pvtModel.Properties.Count > 0)
                {
                    // 获取当前所有OAuth Scheme
                    var AllSchemes = _oauthProvider.GetAllSchemesAsync().Result.Select(x => x.Name).ToList();

                    var TenantSchemes = AppDefaultData.Tenant.OAuthHandlers.Select(x => x.Key).ToList();

                    foreach (var scheme in TenantSchemes)
                    {
                        var ClientIdKey = $"{scheme}:ClientId";
                        var ClientIdValue = pvtModel.Properties[ClientIdKey];

                        var ClientSecretKey = $"{scheme}:ClientSecret";
                        var ClientSecretValue = pvtModel.Properties[ClientSecretKey];

                        if (string.IsNullOrWhiteSpace(ClientIdValue) ||
                            string.IsNullOrWhiteSpace(ClientSecretValue))
                        {
                            _oauthProvider.RemoveScheme(scheme);
                            continue;
                        }

                        AppDefaultData.Tenant.TenantProperties[ClientIdKey] = ClientIdValue;
                        AppDefaultData.Tenant.TenantProperties[ClientSecretKey] = ClientSecretValue;

                        if (!AllSchemes.Contains(scheme))
                        {
                            var authScheme = new AuthenticationScheme(scheme,
                               scheme, AppDefaultData.Tenant.OAuthHandlers[scheme]);

                            _oauthProvider.AddScheme(authScheme);
                        }
                    }


                    _memoryCache.Set(reflushFlagCacheKey,
                        "1",
                        TimeSpan.FromSeconds(TenantConstant.SchemesReflushDuration));
                }
                #endregion
            }

            return _next(context);
        }
    }
}
