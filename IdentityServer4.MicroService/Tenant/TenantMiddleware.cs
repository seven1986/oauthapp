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
                    ResetOAuthOptions(WeixinDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(WeiboDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(GitHubDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(QQDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(FacebookDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(MicrosoftAccountDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(GoogleDefaults.AuthenticationScheme, 
                        pvtModel.Properties);

                    ResetOAuthOptions(TwitterDefaults.AuthenticationScheme, 
                        pvtModel.Properties);
                }
                #endregion
            }

            return _next(context);
        }

        void ResetOAuthOptions(
            string scheme,
            Dictionary<string, string> claims)
        {
            var ClientIdKey = $"{scheme}:ClientId";
            var ClientSecretKey = $"{scheme}:ClientSecret";

            if (claims.ContainsKey(ClientIdKey) &&
                claims.ContainsKey(ClientSecretKey))
            {
                AppDefaultData.Tenant.TenantProperties[ClientIdKey] = claims[ClientIdKey];
                AppDefaultData.Tenant.TenantProperties[ClientSecretKey] = claims[ClientSecretKey];
            }
        }
    }
}