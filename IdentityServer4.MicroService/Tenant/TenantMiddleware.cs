using System.Threading.Tasks;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System;
using Microsoft.Extensions.Caching.Memory;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Options;

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
            TenantService tenantService,
            IAuthenticationSchemeProvider oauthProvider,
            IMemoryCache memoryCache,
            IdentityServerOptions identityServerOptions)
        {
            _next = next;
            _tenantService = tenantService;
            _oauthProvider = oauthProvider;
            _memoryCache = memoryCache;
            _identityServerOptions = identityServerOptions;
        }

        public Task Invoke(
            HttpContext context,
            TenantDbContext _db,
            IOptionsMonitor<IdentityServerAuthenticationOptions> identityServerAuthenticationOptions)
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

                #region AuthorityUri
                identityServerAuthenticationOptions.CurrentValue.Authority = _identityServerOptions.IssuerUri; 
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
