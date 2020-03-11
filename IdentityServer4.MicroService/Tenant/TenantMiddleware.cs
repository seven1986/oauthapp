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
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using AspNet.Security.OAuth.Weibo;
using AspNet.Security.OAuth.Weixin;
using AspNet.Security.OAuth.QQ;
using AspNet.Security.OAuth.GitHub;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantMiddleware
    {
        readonly RequestDelegate _next;
        readonly TenantService _tenantService;
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
            TenantService tenantService,
            IAuthenticationSchemeProvider oauthProvider,
            IMemoryCache memoryCache,
            IdentityServerOptions identityServerOptions,
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
            _tenantService = tenantService;
            _oauthProvider = oauthProvider;
            _memoryCache = memoryCache;
            _identityServerOptions = identityServerOptions;
            _microsoftOptions = microsoftOptions.Get(MicrosoftAccountDefaults.AuthenticationScheme);
            _googleOptions = googleOptions.Get(GoogleDefaults.AuthenticationScheme);
            _facebookOptions = facebookOptions.Get(FacebookDefaults.AuthenticationScheme);
            _githubOptions = githubOptions.Get(GitHubAuthenticationDefaults.AuthenticationScheme);
            _qqOptions = qqOptions.Get(QQAuthenticationDefaults.AuthenticationScheme);
            _weiboOptions = weiboOptions.Get(WeiboAuthenticationDefaults.AuthenticationScheme);
            _weixinOptions = weixinOptions.Get(WeixinAuthenticationDefaults.AuthenticationScheme);
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

            var ResetOAuthProvider_CacheKey = TenantConstant.SchemesReflush + context.Request.Host.Value;

            var ResetOAuthProvider_Flag = _memoryCache.Get<string>(ResetOAuthProvider_CacheKey);

            if (tenant.Item2 != null)
            {
                var pvtModel = tenant.Item2;

                #region IdentityServer4 - IssuerUri
                _identityServerOptions.IssuerUri = context.Request.Scheme + "://" + tenant.Item2.IdentityServerIssuerUri;
                #endregion

                #region IdentityServer4 - AuthorityUri
                identityServerAuthenticationOptions.CurrentValue.Authority = _identityServerOptions.IssuerUri;
                #endregion

                #region ResetOAuthProvider - PerRequest
                if (string.IsNullOrWhiteSpace(ResetOAuthProvider_Flag) && pvtModel.Properties.Count > 0)
                {
                    var AppSchemes = _oauthProvider.GetAllSchemesAsync().Result.Select(x => x.Name).ToList();

                    foreach (var scheme in OAuthBuilderExtensions.Schemes)
                    {
                        if (!pvtModel.Properties.ContainsKey($"{scheme}:ClientId") ||
                            !pvtModel.Properties.ContainsKey($"{scheme}:ClientSecret"))
                        {
                            _oauthProvider.RemoveScheme(scheme);
                            continue;
                        }

                        var ClientId_FromTenant = pvtModel.Properties[$"{scheme}:ClientId"];
                        var ClientSecret_FromTenant = pvtModel.Properties[$"{scheme}:ClientSecret"];

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
            }

            return _next(context);
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
