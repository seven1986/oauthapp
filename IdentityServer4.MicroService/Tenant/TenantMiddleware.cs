using System;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Amazon;
using AspNet.Security.OAuth.Gitter;
using AspNet.Security.OAuth.Salesforce;
using AspNet.Security.OAuth.Reddit;
using AspNet.Security.OAuth.Paypal;
using AspNet.Security.OAuth.LinkedIn;
using AspNet.Security.OAuth.Instagram;
using AspNet.Security.OAuth.WordPress;
using AspNet.Security.OAuth.VisualStudio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using IdentityServer4.Configuration;
using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.DependencyInjection;

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
        readonly AmazonAuthenticationOptions _amazonOptions;
        readonly FacebookOptions _facebookOptions;
        readonly GitHubOptions _githubOptions;
        readonly GitterAuthenticationOptions _gitterOptions;
        readonly GoogleOptions _googleOptions;
        readonly InstagramAuthenticationOptions _instagramOptions;
        readonly LinkedInAuthenticationOptions _linkedinOptions;
        readonly MicrosoftAccountOptions _microsoftOptions;
        readonly PaypalAuthenticationOptions _paypalOptions;
        readonly QQOptions _qqOptions;
        readonly RedditAuthenticationOptions _redditOptions;
        readonly SalesforceAuthenticationOptions _salesforceOptions;
        readonly TwitterOptions _twitterOptions;
        readonly VisualStudioAuthenticationOptions _visualstudioOptions;
        readonly WeiboOptions _weiboOptions;
        readonly WeixinOptions _weixinOptions;
        readonly WordPressAuthenticationOptions _wordpressOptions;
        #endregion

        public TenantMiddleware(
            RequestDelegate next,
            TenantService tenantService,
            IAuthenticationSchemeProvider oauthProvider,
            IMemoryCache memoryCache,
            IdentityServerOptions identityServerOptions,

            IOptionsMonitor<AmazonAuthenticationOptions> amazonOptions,
            IOptionsMonitor<FacebookOptions> facebookOptions,
            IOptionsMonitor<GitHubOptions> githubOptions,
            IOptionsMonitor<GitterAuthenticationOptions> gitterOptions,
            IOptionsMonitor<GoogleOptions> googleOptions,
            IOptionsMonitor<InstagramAuthenticationOptions> instagramOptions,
            IOptionsMonitor<LinkedInAuthenticationOptions> linkedinOptions,
            IOptionsMonitor<MicrosoftAccountOptions> microsoftOptions,
            IOptionsMonitor<PaypalAuthenticationOptions> paypalOptions,
            IOptionsMonitor<QQOptions> qqOptions,
            IOptionsMonitor<RedditAuthenticationOptions> redditOptions,
            IOptionsMonitor<SalesforceAuthenticationOptions> salesforceOptions,
            IOptionsMonitor<TwitterOptions> twitterOptions,
            IOptionsMonitor<VisualStudioAuthenticationOptions> visualstudioOptions,
            IOptionsMonitor<WeiboOptions> weiboOptions,
            IOptionsMonitor<WeixinOptions> weixinOptions,
            IOptionsMonitor<WordPressAuthenticationOptions> wordpressOptions
            )
        {
            _next = next;
            _tenantService = tenantService;
            _oauthProvider = oauthProvider;
            _memoryCache = memoryCache;
            _identityServerOptions = identityServerOptions;

            _amazonOptions = amazonOptions.CurrentValue;
            _facebookOptions = facebookOptions.CurrentValue;
            _githubOptions = githubOptions.CurrentValue;
            _gitterOptions = gitterOptions.CurrentValue;
            _googleOptions = googleOptions.CurrentValue;
            _instagramOptions = instagramOptions.CurrentValue;
            _linkedinOptions = linkedinOptions.CurrentValue;
            _microsoftOptions = microsoftOptions.CurrentValue;
            _paypalOptions = paypalOptions.CurrentValue;
            _qqOptions = qqOptions.CurrentValue;
            _redditOptions = redditOptions.CurrentValue;
            _salesforceOptions = salesforceOptions.CurrentValue;
            _twitterOptions = twitterOptions.CurrentValue;
            _visualstudioOptions = visualstudioOptions.CurrentValue;
            _weiboOptions = weiboOptions.CurrentValue;
            _weixinOptions = weixinOptions.CurrentValue;
            _wordpressOptions = wordpressOptions.CurrentValue;
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
                    // All Schemes
                    var ApplicationSchemes = _oauthProvider.GetAllSchemesAsync().Result.Select(x => x.Name).ToList();

                    // All Scheme Providers
                    var SchemeProviders = OAuthBuilderExtensions.Handlers.Select(x => x.Key).ToList();

                    foreach (var oauthScheme in SchemeProviders)
                    {
                        var ClientId_FromTenant = pvtModel.Properties[$"{oauthScheme}:ClientId"];

                        var ClientSecret_FromTenant = pvtModel.Properties[$"{oauthScheme}:ClientSecret"];

                        if (string.IsNullOrWhiteSpace(ClientId_FromTenant) ||
                            string.IsNullOrWhiteSpace(ClientSecret_FromTenant))
                        {
                            _oauthProvider.RemoveScheme(oauthScheme);

                            continue;
                        }

                        switch (oauthScheme)
                        {
                            case AmazonAuthenticationDefaults.AuthenticationScheme:
                                _amazonOptions.ClientId = ClientId_FromTenant;
                                _amazonOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case FacebookDefaults.AuthenticationScheme:
                                _facebookOptions.ClientId = ClientId_FromTenant;
                                _facebookOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case GitHubDefaults.AuthenticationScheme:
                                _amazonOptions.ClientId = ClientId_FromTenant;
                                _amazonOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case GitterAuthenticationDefaults.AuthenticationScheme:
                                _gitterOptions.ClientId = ClientId_FromTenant;
                                _gitterOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case GoogleDefaults.AuthenticationScheme:
                                _googleOptions.ClientId = ClientId_FromTenant;
                                _googleOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case InstagramAuthenticationDefaults.AuthenticationScheme:
                                _instagramOptions.ClientId = ClientId_FromTenant;
                                _instagramOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case LinkedInAuthenticationDefaults.AuthenticationScheme:
                                _linkedinOptions.ClientId = ClientId_FromTenant;
                                _linkedinOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case MicrosoftAccountDefaults.AuthenticationScheme:
                                _microsoftOptions.ClientId = ClientId_FromTenant;
                                _microsoftOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case PaypalAuthenticationDefaults.AuthenticationScheme:
                                _paypalOptions.ClientId = ClientId_FromTenant;
                                _paypalOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case QQDefaults.AuthenticationScheme:
                                _amazonOptions.ClientId = ClientId_FromTenant;
                                _amazonOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case RedditAuthenticationDefaults.AuthenticationScheme:
                                _redditOptions.ClientId = ClientId_FromTenant;
                                _redditOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case SalesforceAuthenticationDefaults.AuthenticationScheme:
                                _salesforceOptions.ClientId = ClientId_FromTenant;
                                _salesforceOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case TwitterDefaults.AuthenticationScheme:
                                _twitterOptions.ConsumerKey = ClientId_FromTenant;
                                _twitterOptions.ConsumerSecret = ClientSecret_FromTenant;
                                break;

                            case VisualStudioAuthenticationDefaults.AuthenticationScheme:
                                _visualstudioOptions.ClientId = ClientId_FromTenant;
                                _visualstudioOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case WeiboDefaults.AuthenticationScheme:
                                _weiboOptions.ClientId = ClientId_FromTenant;
                                _weiboOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case WeixinDefaults.AuthenticationScheme:
                                _weixinOptions.ClientId = ClientId_FromTenant;
                                _weixinOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            case WordPressAuthenticationDefaults.AuthenticationScheme:
                                _wordpressOptions.ClientId = ClientId_FromTenant;
                                _wordpressOptions.ClientSecret = ClientSecret_FromTenant;
                                break;

                            default: break;
                        }

                        if (!ApplicationSchemes.Contains(oauthScheme))
                        {
                            _oauthProvider.AddScheme(new AuthenticationScheme(oauthScheme,
                                oauthScheme, OAuthBuilderExtensions.Handlers[oauthScheme]));
                        }
                    }

                    _memoryCache.Set(ResetOAuthProvider_CacheKey,
                        "1",
                        TimeSpan.FromSeconds(TenantConstant.SchemesReflushDuration));
                }
                #endregion
            }

            return _next(context);
        }
    }
}
