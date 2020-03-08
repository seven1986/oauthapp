//using AspNet.Security.OAuth.Amazon;
//using AspNet.Security.OAuth.Gitter;
//using AspNet.Security.OAuth.Instagram;
//using AspNet.Security.OAuth.LinkedIn;
//using AspNet.Security.OAuth.Paypal;
//using AspNet.Security.OAuth.Reddit;
//using AspNet.Security.OAuth.Salesforce;
//using AspNet.Security.OAuth.VisualStudio;
//using AspNet.Security.OAuth.WordPress;
//using Microsoft.AspNetCore.Authentication.Facebook;
//using Microsoft.AspNetCore.Authentication.GitHub;
//using Microsoft.AspNetCore.Authentication.QQ;
//using Microsoft.AspNetCore.Authentication.Twitter;
//using Microsoft.AspNetCore.Authentication.Weibo;
//using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Facebook;
using AspNet.Security.OAuth.GitHub;
using AspNet.Security.OAuth.QQ;
using AspNet.Security.OAuth.Weibo;
using AspNet.Security.OAuth.Weixin;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// DI extension methods for adding IdentityServer
    /// </summary>
    public static class OAuthBuilderExtensions
    {
        // https://github.com/aspnet/Security/issues/1576
        // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/microsoft-logins
        // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/facebook-logins
        // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/google-logins
        // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/twitter-logins

        public static List<string> Schemes = new List<string>();

        static T OAuthSettings<T>(this IConfiguration configuration, string AuthenticationName) where T : OAuthOptions
        {
            var result = configuration.GetSection("IdentityServer:Authentication:" + AuthenticationName).Get<T>();

            return result;
        }

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="authBuilder">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static void AddIdentityServer4MicroServiceOAuths(this AuthenticationBuilder authBuilder, IConfiguration configuration)
        {
            // 如果有配置Auth节点，就添加对应的Auth服务。
            // 如果Auth节点数据没设置，就添加默认的ClientId、ClientSecret

            #region MicrosoftAccount (/signin-microsoft)
            var microsoft_options = configuration.OAuthSettings<MicrosoftAccountOptions>("MicrosoftAccount");
            authBuilder.Services.AddSingleton(microsoft_options);
            if (microsoft_options != null&&
                !string.IsNullOrWhiteSpace(microsoft_options.ClientId)&&
                !string.IsNullOrWhiteSpace(microsoft_options.ClientSecret))
            {
                Schemes.Add(MicrosoftAccountDefaults.AuthenticationScheme);
                authBuilder.AddMicrosoftAccount(x =>
                {
                    x.ClientId = microsoft_options.ClientId;
                    x.ClientSecret = microsoft_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(MicrosoftAccountOptions), microsoft_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            #region Google (/signin-google)
            var google_options = configuration.OAuthSettings<GoogleOptions>(GoogleDefaults.AuthenticationScheme);
            authBuilder.Services.AddSingleton(google_options);
            if (google_options != null &&
                !string.IsNullOrWhiteSpace(google_options.ClientId) &&
                !string.IsNullOrWhiteSpace(google_options.ClientSecret))
            {
                Schemes.Add(GoogleDefaults.AuthenticationScheme);
                authBuilder.AddGoogle(x =>
                {
                    x.ClientId = google_options.ClientId;
                    x.ClientSecret = google_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(GoogleOptions), google_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            #region Facebook (/signin-facebook)
            var facebook_options = configuration.OAuthSettings<FacebookOptions>(FacebookDefaults.AuthenticationScheme);
            authBuilder.Services.AddSingleton(facebook_options);
            if (facebook_options != null &&
                !string.IsNullOrWhiteSpace(facebook_options.ClientId) &&
                !string.IsNullOrWhiteSpace(facebook_options.ClientSecret))
            {
                Schemes.Add(FacebookDefaults.AuthenticationScheme);

                authBuilder.AddFacebook(x =>
                {
                    x.ClientId = facebook_options.ClientId;
                    x.ClientSecret = facebook_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(FacebookOptions), facebook_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            #region GitHub (/signin-github)
            var github_options = configuration.OAuthSettings<GitHubAuthenticationOptions>(GitHubAuthenticationDefaults.AuthenticationScheme);
            authBuilder.Services.AddSingleton(github_options);
            if (github_options != null &&
                !string.IsNullOrWhiteSpace(github_options.ClientId) &&
                !string.IsNullOrWhiteSpace(github_options.ClientSecret))
            {
                Schemes.Add(GitHubAuthenticationDefaults.AuthenticationScheme);

                authBuilder.AddGitHub(x =>
                {
                    x.ClientId = github_options.ClientId;
                    x.ClientSecret = github_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(GitHubAuthenticationOptions), github_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            #region QQ (/signin-qq)
            var qq_options = configuration.OAuthSettings<QQAuthenticationOptions>(QQAuthenticationDefaults.AuthenticationScheme);
            authBuilder.Services.AddSingleton(qq_options);
            if (qq_options != null &&
                !string.IsNullOrWhiteSpace(qq_options.ClientId) &&
                !string.IsNullOrWhiteSpace(qq_options.ClientSecret))
            {
                Schemes.Add(QQAuthenticationDefaults.AuthenticationScheme);

                authBuilder.AddQQ(x =>
                {
                    x.ClientId = qq_options.ClientId;
                    x.ClientSecret = qq_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(QQAuthenticationOptions), qq_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            #region Weibo (/signin-weibo)
            var weibo_options = configuration.OAuthSettings<WeiboAuthenticationOptions>(WeiboAuthenticationDefaults.AuthenticationScheme);
            authBuilder.Services.AddSingleton(weibo_options);
            if (weibo_options != null &&
                !string.IsNullOrWhiteSpace(weibo_options.ClientId) &&
                !string.IsNullOrWhiteSpace(weibo_options.ClientSecret))
            {
                Schemes.Add(WeiboAuthenticationDefaults.AuthenticationScheme);

                authBuilder.AddWeibo(x =>
                {
                    x.ClientId = weibo_options.ClientId;
                    x.ClientSecret = weibo_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(WeiboAuthenticationOptions), weibo_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            #region Weixin (/signin-weixin)
            var weixin_options = configuration.OAuthSettings<WeixinAuthenticationOptions>(WeixinAuthenticationDefaults.AuthenticationScheme);
            authBuilder.Services.AddSingleton(weixin_options);
            if (weixin_options != null &&
                !string.IsNullOrWhiteSpace(weixin_options.ClientId) &&
                !string.IsNullOrWhiteSpace(weixin_options.ClientSecret))
            {
                Schemes.Add(WeixinAuthenticationDefaults.AuthenticationScheme);

                authBuilder.AddWeixin(x =>
                {
                    x.ClientId = weixin_options.ClientId;
                    x.ClientSecret = weixin_options.ClientSecret;
                    authBuilder.Services.Remove(new ServiceDescriptor(typeof(WeixinAuthenticationOptions), weixin_options));
                    authBuilder.Services.AddSingleton(x);
                });
            }
            #endregion

            //#region Twitter (/signin-twitter)
            //var twitter_options = configuration.GetSection("IdentityServer:Authentication:Twitter").Get<TwitterOptions>();
            //if (twitter_options != null)
            //{
            //    Handlers.Add(TwitterDefaults.AuthenticationScheme, typeof(TwitterHandler2));

            //    authBuilder.AddTwitter2(x =>
            //    {
            //        x.ConsumerKey = twitter_options.ConsumerKey ?? unconfig;
            //        x.ConsumerSecret = twitter_options.ConsumerSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region Amazon (/signin-amazon)
            //var amazon_options = configuration.Options<AmazonAuthenticationOptions>("Amazon");
            //if (amazon_options != null)
            //{
            //    Handlers.Add(AmazonAuthenticationDefaults.AuthenticationScheme, typeof(AmazonAuthenticationHandler));

            //    authBuilder.AddAmazon(x =>
            //    {
            //        x.ClientId = amazon_options.ClientId ?? unconfig;
            //        x.ClientSecret = amazon_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion


            //#region Gitter (/signin-gitter)
            //var gitter_options = configuration.Options<GitterAuthenticationOptions>("Gitter");
            //if (gitter_options != null)
            //{
            //    Handlers.Add(GitterAuthenticationDefaults.AuthenticationScheme, typeof(GitterAuthenticationHandler));

            //    authBuilder.AddGitter(x =>
            //    {
            //        x.ClientId = gitter_options.ClientId ?? unconfig;
            //        x.ClientSecret = gitter_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region Instagram (/signin-instagram)
            //var instagram_options = configuration.Options<InstagramAuthenticationOptions>("Instagram");
            //if (instagram_options != null)
            //{
            //    Handlers.Add(InstagramAuthenticationDefaults.AuthenticationScheme, typeof(InstagramAuthenticationHandler));

            //    authBuilder.AddInstagram(x =>
            //    {
            //        x.ClientId = instagram_options.ClientId ?? unconfig;
            //        x.ClientSecret = instagram_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region LinkedIn (/signin-linkedin)
            //var linkedin_options = configuration.Options<LinkedInAuthenticationOptions>("LinkedIn");
            //if (linkedin_options != null)
            //{
            //    Handlers.Add(LinkedInAuthenticationDefaults.AuthenticationScheme, typeof(LinkedInAuthenticationHandler));

            //    authBuilder.AddLinkedIn(x =>
            //    {
            //        x.ClientId = linkedin_options.ClientId ?? unconfig;
            //        x.ClientSecret = linkedin_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region Paypal (/signin-paypal)
            //var paypal_options = configuration.Options<PaypalAuthenticationOptions>("Paypal");
            //if (paypal_options != null)
            //{
            //    Handlers.Add(PaypalAuthenticationDefaults.AuthenticationScheme, typeof(PaypalAuthenticationHandler));

            //    authBuilder.AddPaypal(x =>
            //    {
            //        x.ClientId = paypal_options.ClientId ?? unconfig;
            //        x.ClientSecret = paypal_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region Reddit (/signin-reddit)
            //var reddit_options = configuration.Options<RedditAuthenticationOptions>("Reddit");
            //if (reddit_options != null)
            //{
            //    Handlers.Add(RedditAuthenticationDefaults.AuthenticationScheme, typeof(RedditAuthenticationHandler));

            //    authBuilder.AddReddit(x =>
            //    {
            //        x.ClientId = reddit_options.ClientId ?? unconfig;
            //        x.ClientSecret = reddit_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region Salesforce (/signin-salesforce)
            //var salesforce_options = configuration.Options<SalesforceAuthenticationOptions>("Salesforce");
            //if (salesforce_options != null)
            //{
            //    Handlers.Add(SalesforceAuthenticationDefaults.AuthenticationScheme, typeof(SalesforceAuthenticationHandler));

            //    authBuilder.AddSalesforce(x =>
            //    {
            //        x.ClientId = salesforce_options.ClientId ?? unconfig;
            //        x.ClientSecret = salesforce_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region VisualStudio (/signin-visualstudio)
            //var visualstudio_options = configuration.Options<VisualStudioAuthenticationOptions>("VisualStudio");
            //if (visualstudio_options != null)
            //{
            //    Handlers.Add(VisualStudioAuthenticationDefaults.AuthenticationScheme, typeof(VisualStudioAuthenticationHandler));

            //    authBuilder.AddVisualStudio(x =>
            //    {
            //        x.ClientId = visualstudio_options.ClientId ?? unconfig;
            //        x.ClientSecret = visualstudio_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion

            //#region WordPress (/signin-wordpress)
            //var wordpress_options = configuration.Options<WordPressAuthenticationOptions>("WordPress");
            //if (wordpress_options != null)
            //{
            //    Handlers.Add(WordPressAuthenticationDefaults.AuthenticationScheme, typeof(WordPressAuthenticationHandler));

            //    authBuilder.AddWordPress(x =>
            //    {
            //        x.ClientId = wordpress_options.ClientId ?? unconfig;
            //        x.ClientSecret = wordpress_options.ClientSecret ?? unconfig;
            //    });
            //}
            //#endregion
        }
    }
}