using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityServer4.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using IdentityServer4.MicroService.Models.AppTenantModels;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantMiddleware
    {
        readonly RequestDelegate _next;
        readonly TenantService _tenantService;

        public TenantMiddleware(
            RequestDelegate next,
            TenantService tenantService
            )
        {
            _next = next;
            _tenantService = tenantService;
        }

        public Task Invoke(
            HttpContext context, 
            TenantDbContext _db
            // IOptionsMonitor<WeixinOptions> _WeixinOptions,
            // IOptionsMonitor<WeiboOptions> _WeiboOptions,
            // IOptionsMonitor<GitHubOptions> _GitHubOptions,
            // IOptionsMonitor<QQOptions> _QQOptions,
            // IOptionsMonitor<FacebookOptions> _FacebookOptions,
            // IOptionsMonitor<MicrosoftAccountOptions> _MicrosoftAccountOptions,
            // IOptionsMonitor<GoogleOptions> _GoogleOptions,
            // IOptionsMonitor<TwitterOptions> _TwitterOptions
            )
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
                            .DeserializeObject<AppTenantPrivateModel>(tenant.Item2);

                #region IssuerUri
                var IdServerOptions = context.RequestServices.GetRequiredService<IdentityServerOptions>();
                IdServerOptions.IssuerUri = context.Request.Scheme + "://" + pvtModel.IdentityServerIssuerUri;
                #endregion

                #region ConfigAuthentication
                //if (pvtModel.properties.Count > 0)
                //{
                //    ConfigAuthentication(_WeixinOptions.CurrentValue,
                //            pvtModel.properties, WeixinDefaults.AuthenticationScheme);
                //
                //    ConfigAuthentication(_WeiboOptions.CurrentValue,
                //        pvtModel.properties, WeiboDefaults.AuthenticationScheme);
                //
                //    ConfigAuthentication(_GitHubOptions.CurrentValue,
                //        pvtModel.properties, GitHubDefaults.AuthenticationScheme);
                //
                //    ConfigAuthentication(_QQOptions.CurrentValue,
                //        pvtModel.properties, QQDefaults.AuthenticationScheme);
                //
                //    ConfigAuthentication(_FacebookOptions.CurrentValue,
                //        pvtModel.properties, FacebookDefaults.AuthenticationScheme);
                //
                //    ConfigAuthentication(_MicrosoftAccountOptions.CurrentValue,
                //        pvtModel.properties, MicrosoftAccountDefaults.AuthenticationScheme);
                //
                //    ConfigAuthentication(_GoogleOptions.CurrentValue,
                //        pvtModel.properties, GoogleDefaults.AuthenticationScheme);
                //
                //    ConfigTwitterAuthentication(_TwitterOptions.CurrentValue,
                //        pvtModel.properties);
                //}
                #endregion
            }

            return _next(context);
        }

        void ConfigAuthentication<T>(
            T options, 
            Dictionary<string, string> claims,
            string key) where T : OAuthOptions
        {
            var clientIdKey = $"{key}:ClientId";
            var ClientSecretKey = $"{key}:ClientSecret";

            if (claims.ContainsKey(clientIdKey) &&
                claims.ContainsKey(ClientSecretKey))
            {
               options.ClientId = claims[clientIdKey];
               options.ClientSecret = claims[ClientSecretKey];
            }
        }

        void ConfigTwitterAuthentication(TwitterOptions options,
            Dictionary<string, string> claims)
        {
            var clientIdKey = "Twitter:ClientId";
            var ClientSecretKey = "Twitter:ClientSecret";

            if (claims.ContainsKey(clientIdKey) &&
                claims.ContainsKey(ClientSecretKey))
            {
                options.ConsumerKey = claims[clientIdKey];
                options.ConsumerSecret = claims[ClientSecretKey];
            }
        }
    }
}