using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityServer4.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using IdentityServer4.MicroService.Models.Shared;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantMiddleware
    {
        readonly RequestDelegate _next;
        readonly TenantService _tenantService;
        readonly IAuthenticationSchemeProvider _oauthProvider;
        public TenantMiddleware(
            RequestDelegate next,
            TenantService tenantService,
            IAuthenticationSchemeProvider oauthProvider
            )
        {
            _next = next;
            _tenantService = tenantService;
            _oauthProvider = oauthProvider;
        }

        public Task Invoke(
            HttpContext context, 
            TenantDbContext _db)
        {
            var tenant = _tenantService.GetTenant(_db,
                context.Request.Host.Value);

            if (!string.IsNullOrWhiteSpace(tenant.Item1))
            {
                context.Items[TenantConstant.CacheKey] = tenant.Item1;
            }

            if (!string.IsNullOrWhiteSpace(tenant.Item2))
            {
                var pvtModel = JsonConvert
                            .DeserializeObject<TenantPrivateModel>(tenant.Item2);

                #region IssuerUri
                var IdServerOptions = context.RequestServices.GetRequiredService<IdentityServerOptions>();
                IdServerOptions.IssuerUri = context.Request.Scheme + "://" + pvtModel.IdentityServerIssuerUri;
                #endregion

                #region ResetOAuthOptions
                if (pvtModel.Properties.Count > 0)
                {
                    // 获取当前所有OAuth Scheme
                    var AllSchemes = _oauthProvider.GetAllSchemesAsync().Result.Select(x => x.Name).ToList();

                    foreach (var v in AppDefaultData.Tenant.OAuthHandlers)
                    {
                        var ClientIdKey = $"{v.Key}:ClientId";
                        var ClientIdValue = pvtModel.Properties[ClientIdKey];

                        var ClientSecretKey = $"{v.Key}:ClientSecret";
                        var ClientSecretValue = pvtModel.Properties[ClientSecretKey];

                        if (string.IsNullOrWhiteSpace(ClientIdValue) ||
                            string.IsNullOrWhiteSpace(ClientSecretValue))
                        {
                            _oauthProvider.RemoveScheme(v.Key);
                            continue;
                        }

                        AppDefaultData.Tenant.TenantProperties[ClientIdKey] = ClientIdValue;
                        AppDefaultData.Tenant.TenantProperties[ClientSecretKey] = ClientSecretValue;

                        if (!AllSchemes.Contains(v.Key))
                        {
                            var authScheme = new AuthenticationScheme(v.Key,
                               v.Key, AppDefaultData.Tenant.OAuthHandlers[v.Key]);

                            _oauthProvider.AddScheme(authScheme);
                        }
                    }
                }
                #endregion
            }

            return _next(context);
        }
    }
}