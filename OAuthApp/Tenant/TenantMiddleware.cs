using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using IdentityServer4.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using AspNet.Security.OAuth.Weibo;
using AspNet.Security.OAuth.Weixin;
using AspNet.Security.OAuth.QQ;
using AspNet.Security.OAuth.GitHub;

namespace OAuthApp.Tenant
{
    public class TenantMiddleware
    {
        readonly RequestDelegate _next;
        readonly IAuthenticationSchemeProvider _oauthProvider;
        readonly IMemoryCache _memoryCache;
        readonly IdentityServerOptions _identityServerOptions;

        #region OAuthOptions
        GoogleOptions _googleOptions;
        MicrosoftAccountOptions _microsoftOptions;
        FacebookOptions _facebookOptions;
        GitHubAuthenticationOptions _githubOptions;
        QQAuthenticationOptions _qqOptions;
        WeiboAuthenticationOptions _weiboOptions;
        WeixinAuthenticationOptions _weixinOptions;
        #endregion

        public TenantMiddleware(
            RequestDelegate next,
            IAuthenticationSchemeProvider oauthProvider,
            IMemoryCache memoryCache,
            IdentityServerOptions identityServerOptions,
            //JwtBearerOptions jwtBearerOptions,
            IOptionsMonitor<MicrosoftAccountOptions> microsoftOptions,
            IOptionsMonitor<GoogleOptions> googleOptions,
            IOptionsMonitor<FacebookOptions> facebookOptions,
            IOptionsMonitor<GitHubAuthenticationOptions> githubOptions,
            IOptionsMonitor<QQAuthenticationOptions> qqOptions,
            IOptionsMonitor<WeiboAuthenticationOptions> weiboOptions,
            IOptionsMonitor<WeixinAuthenticationOptions> weixinOptions
            
            )
        {
            _next = next;
            _oauthProvider = oauthProvider;
            _memoryCache = memoryCache;
            _identityServerOptions = identityServerOptions;
            //_jwtBearerOptions = jwtBearerOptions;
            _microsoftOptions = microsoftOptions.Get(MicrosoftAccountDefaults.AuthenticationScheme);
            _googleOptions = googleOptions.Get(GoogleDefaults.AuthenticationScheme);
            _facebookOptions = facebookOptions.Get(FacebookDefaults.AuthenticationScheme);
            _githubOptions = githubOptions.Get(GitHubAuthenticationDefaults.AuthenticationScheme);
            _qqOptions = qqOptions.Get(QQAuthenticationDefaults.AuthenticationScheme);
            _weiboOptions = weiboOptions.Get(WeiboAuthenticationDefaults.AuthenticationScheme);
            _weixinOptions = weixinOptions.Get(WeixinAuthenticationDefaults.AuthenticationScheme);
        }

        public Task Invoke(
            HttpContext context)
        {
            var _tenantService = context.RequestServices.GetService<TenantService>();

            var tenant = _tenantService.GetTenant(context.Request.Host.Value);

            if (tenant.Item1 == null || tenant.Item2 == null)
            {
                context.Response.StatusCode = 400;

                return context.Response.WriteAsync("error page");
            }

            context.Items[TenantConstant.CacheKey] = tenant.Item1;

            context.Items[TenantConstant.HttpContextItemKey] = tenant.Item2;

            var ResetOAuthProvider_CacheKey = TenantConstant.SchemesReflush + context.Request.Host.Value;

            var ResetOAuthProvider_Flag = _memoryCache.Get<string>(ResetOAuthProvider_CacheKey);

            var TenantProperties = tenant.Item2.Properties;

            var IssuerUri = $"{context.Request.Scheme}://{tenant.Item2.IdentityServerIssuerUri}";

            #region IdentityServer4 - IssuerUri
            _identityServerOptions.IssuerUri = IssuerUri;
            #endregion

            #region ResetUserInteraction
            if (TenantProperties.ContainsKey(UserInteractionKeys.Enable) &&
                TenantProperties[UserInteractionKeys.Enable].Equals("true"))
            {
                if (TenantProperties.ContainsKey(UserInteractionKeys.LoginUrl))
                {
                    _identityServerOptions.UserInteraction.LoginUrl = TenantProperties[UserInteractionKeys.LoginUrl];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.LoginReturnUrlParameter))
                {
                    _identityServerOptions.UserInteraction.LoginReturnUrlParameter = TenantProperties[UserInteractionKeys.LoginReturnUrlParameter];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.LogoutUrl))
                {
                    _identityServerOptions.UserInteraction.LogoutUrl = TenantProperties[UserInteractionKeys.LogoutUrl];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.LogoutIdParameter))
                {
                    _identityServerOptions.UserInteraction.LogoutIdParameter = TenantProperties[UserInteractionKeys.LogoutIdParameter];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.ConsentUrl))
                {
                    _identityServerOptions.UserInteraction.ConsentUrl = TenantProperties[UserInteractionKeys.ConsentUrl];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.ConsentReturnUrlParameter))
                {
                    _identityServerOptions.UserInteraction.ConsentReturnUrlParameter = TenantProperties[UserInteractionKeys.ConsentReturnUrlParameter];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.ErrorUrl))
                {
                    _identityServerOptions.UserInteraction.ErrorUrl = TenantProperties[UserInteractionKeys.ErrorUrl];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.ErrorIdParameter))
                {
                    _identityServerOptions.UserInteraction.ErrorIdParameter = TenantProperties[UserInteractionKeys.ErrorIdParameter];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.CustomRedirectReturnUrlParameter))
                {
                    _identityServerOptions.UserInteraction.CustomRedirectReturnUrlParameter = TenantProperties[UserInteractionKeys.CustomRedirectReturnUrlParameter];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.CookieMessageThreshold))
                {
                    if (int.TryParse(TenantProperties[UserInteractionKeys.CookieMessageThreshold], out int _i))
                    {
                        _identityServerOptions.UserInteraction.CookieMessageThreshold = _i;
                    }
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.DeviceVerificationUrl))
                {
                    _identityServerOptions.UserInteraction.DeviceVerificationUrl = TenantProperties[UserInteractionKeys.DeviceVerificationUrl];
                }
                if (TenantProperties.ContainsKey(UserInteractionKeys.DeviceVerificationUserCodeParameter))
                {
                    _identityServerOptions.UserInteraction.DeviceVerificationUserCodeParameter = TenantProperties[UserInteractionKeys.DeviceVerificationUserCodeParameter];
                }
            }
            #endregion

            #region ResetOAuthProvider - PerRequest
            if (string.IsNullOrWhiteSpace(ResetOAuthProvider_Flag) && TenantProperties.Count > 0)
            {
                var AppSchemes = _oauthProvider.GetAllSchemesAsync().Result.Select(x => x.Name).ToList();

                foreach (var scheme in OAuthBuilderExtensions.Schemes)
                {
                    if (!TenantProperties.ContainsKey($"{scheme}:ClientId") ||
                        !TenantProperties.ContainsKey($"{scheme}:ClientSecret"))
                    {
                        _oauthProvider.RemoveScheme(scheme);
                        continue;
                    }

                    var ClientId_FromTenant = TenantProperties[$"{scheme}:ClientId"];
                    var ClientSecret_FromTenant = TenantProperties[$"{scheme}:ClientSecret"];

                    if (string.IsNullOrWhiteSpace(ClientId_FromTenant) ||
                        string.IsNullOrWhiteSpace(ClientSecret_FromTenant))
                    {
                        _oauthProvider.RemoveScheme(scheme);
                        continue;
                    }

                    switch (scheme)
                    {
                        case MicrosoftAccountDefaults.AuthenticationScheme:
                            _microsoftOptions.ClientId = ClientId_FromTenant;
                            _microsoftOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(MicrosoftAccountHandler));
                            break;

                        case GoogleDefaults.AuthenticationScheme:
                            _googleOptions.ClientId = ClientId_FromTenant;
                            _googleOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(GoogleHandler));
                            break;

                        case WeiboAuthenticationDefaults.AuthenticationScheme:
                            _weiboOptions.ClientId = ClientId_FromTenant;
                            _weiboOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(WeiboAuthenticationHandler));
                            break;

                        case WeixinAuthenticationDefaults.AuthenticationScheme:
                            _weixinOptions.ClientId = ClientId_FromTenant;
                            _weixinOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(WeixinAuthenticationHandler));
                            break;

                        case QQAuthenticationDefaults.AuthenticationScheme:
                            _qqOptions.ClientId = ClientId_FromTenant;
                            _qqOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(QQAuthenticationHandler));
                            break;

                        case GitHubAuthenticationDefaults.AuthenticationScheme:
                            _githubOptions.ClientId = ClientId_FromTenant;
                            _githubOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(GitHubAuthenticationHandler));
                            break;

                        case FacebookDefaults.AuthenticationScheme:
                            _facebookOptions.ClientId = ClientId_FromTenant;
                            _facebookOptions.ClientSecret = ClientSecret_FromTenant;
                            AddSchemeIfNotExists(AppSchemes, scheme, typeof(FacebookHandler));
                            break;

                        //case AmazonAuthenticationDefaults.AuthenticationScheme:
                        //    _amazonOptions.ClientId = ClientId_FromTenant;
                        //    _amazonOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case GitterAuthenticationDefaults.AuthenticationScheme:
                        //    _gitterOptions.ClientId = ClientId_FromTenant;
                        //    _gitterOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case InstagramAuthenticationDefaults.AuthenticationScheme:
                        //    _instagramOptions.ClientId = ClientId_FromTenant;
                        //    _instagramOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case LinkedInAuthenticationDefaults.AuthenticationScheme:
                        //    _linkedinOptions.ClientId = ClientId_FromTenant;
                        //    _linkedinOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case PaypalAuthenticationDefaults.AuthenticationScheme:
                        //    _paypalOptions.ClientId = ClientId_FromTenant;
                        //    _paypalOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case RedditAuthenticationDefaults.AuthenticationScheme:
                        //    _redditOptions.ClientId = ClientId_FromTenant;
                        //    _redditOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case SalesforceAuthenticationDefaults.AuthenticationScheme:
                        //    _salesforceOptions.ClientId = ClientId_FromTenant;
                        //    _salesforceOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        //case TwitterDefaults.AuthenticationScheme:
                        //    _twitterOptions.ConsumerKey = ClientId_FromTenant;
                        //    _twitterOptions.ConsumerSecret = ClientSecret_FromTenant;
                        //    break;

                        //case VisualStudioAuthenticationDefaults.AuthenticationScheme:
                        //    _visualstudioOptions.ClientId = ClientId_FromTenant;
                        //    _visualstudioOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;


                        //case WordPressAuthenticationDefaults.AuthenticationScheme:
                        //    _wordpressOptions.ClientId = ClientId_FromTenant;
                        //    _wordpressOptions.ClientSecret = ClientSecret_FromTenant;
                        //    break;

                        default: break;
                    }
                }

                var MemoryCacheOptions = new MemoryCacheEntryOptions();

                MemoryCacheOptions.SetAbsoluteExpiration(
                    TimeSpan.FromSeconds(TenantConstant.SchemesReflushDuration));

                _memoryCache.Set(ResetOAuthProvider_CacheKey,
                    "1",
                    MemoryCacheOptions);
            }
            #endregion

            return _next.Invoke(context);
        }

        void AddSchemeIfNotExists(List<string> AppSchemes, string oauthScheme, Type handlerType)
        {
            if (!AppSchemes.Contains(oauthScheme))
            {
                _oauthProvider.AddScheme(new AuthenticationScheme(oauthScheme, oauthScheme, handlerType));
            }
        }
    }
}
