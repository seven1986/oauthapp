using AspNet.Security.OAuth.Amazon;
using AspNet.Security.OAuth.Gitter;
using AspNet.Security.OAuth.Instagram;
using AspNet.Security.OAuth.LinkedIn;
using AspNet.Security.OAuth.Paypal;
using AspNet.Security.OAuth.Reddit;
using AspNet.Security.OAuth.Salesforce;
using AspNet.Security.OAuth.VisualStudio;
using AspNet.Security.OAuth.WordPress;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.Weixin;
using System.Collections.Generic;
using System;

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

        const string unconfig = "unconfig";

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="authBuilder">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static void AddIdentityServer4MicroServiceOAuths(this AuthenticationBuilder authBuilder, IConfiguration configuration)
        {
            #region Amazon (/signin-amazon)
            var amazon_options = configuration.GetSection("OAuth:Amazon").Get<AmazonAuthenticationOptions>();
            if (amazon_options != null)
            {
                Handlers.Add(AmazonAuthenticationDefaults.AuthenticationScheme, typeof(AmazonAuthenticationHandler));

                authBuilder.AddAmazon(x =>
                {
                    x.ClientId = amazon_options.ClientId ?? unconfig;
                    x.ClientSecret = amazon_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Facebook (/signin-facebook)
            var facebook_options = configuration.GetSection("OAuth:Facebook").Get<FacebookOptions>();
            if (facebook_options != null)
            {
                Handlers.Add(FacebookDefaults.AuthenticationScheme, typeof(FacebookHandler2));

                authBuilder.AddFacebook2(x =>
                {
                    x.AppId = facebook_options.ClientId ?? unconfig;
                    x.AppSecret = facebook_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region GitHub (/signin-github)
            var github_options = configuration.GetSection("OAuth:GitHub").Get<GitHubOptions>();
            if (github_options != null)
            {
                Handlers.Add(GitHubDefaults.AuthenticationScheme, typeof(GitHubHandler));

                authBuilder.AddGitHub(x =>
                {
                    x.ClientId = github_options.ClientId ?? unconfig;
                    x.ClientSecret = github_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Gitter (/signin-gitter)
            var gitter_options = configuration.GetSection("OAuth:Gitter").Get<GitterAuthenticationOptions>();
            if (gitter_options != null)
            {
                Handlers.Add(GitterAuthenticationDefaults.AuthenticationScheme, typeof(GitterAuthenticationHandler));

                authBuilder.AddGitter(x =>
                {
                    x.ClientId = gitter_options.ClientId ?? unconfig;
                    x.ClientSecret = gitter_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Google (/signin-google)
            var google_options = configuration.GetSection("OAuth:Google").Get<GoogleOptions>();
            if (google_options != null)
            {
                Handlers.Add(GoogleDefaults.AuthenticationScheme, typeof(GoogleHandler2));

                authBuilder.AddGoogle2(x =>
                {
                    x.ClientId = google_options.ClientId ?? unconfig;
                    x.ClientSecret = google_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Instagram (/signin-instagram)
            var instagram_options = configuration.GetSection("OAuth:Instagram").Get<InstagramAuthenticationOptions>();
            if (instagram_options != null)
            {
                Handlers.Add(InstagramAuthenticationDefaults.AuthenticationScheme, typeof(InstagramAuthenticationHandler));

                authBuilder.AddInstagram(x =>
                {
                    x.ClientId = instagram_options.ClientId ?? unconfig;
                    x.ClientSecret = instagram_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region LinkedIn (/signin-linkedin)
            var linkedin_options = configuration.GetSection("OAuth:LinkedIn").Get<LinkedInAuthenticationOptions>();
            if (linkedin_options != null)
            {
                Handlers.Add(LinkedInAuthenticationDefaults.AuthenticationScheme, typeof(LinkedInAuthenticationHandler));

                authBuilder.AddLinkedIn(x =>
                {
                    x.ClientId = linkedin_options.ClientId ?? unconfig;
                    x.ClientSecret = linkedin_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region MicrosoftAccount (/signin-microsoft)
            var microsoft_options = configuration.GetSection("OAuth:MicrosoftAccount").Get<MicrosoftAccountOptions>();
            if (microsoft_options != null)
            {
                Handlers.Add(MicrosoftAccountDefaults.AuthenticationScheme, typeof(MicrosoftAccountHandler2));

                authBuilder.AddMicrosoftAccount2(x =>
                {
                    x.ClientId = microsoft_options.ClientId ?? unconfig;
                    x.ClientSecret = microsoft_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Paypal (/signin-paypal)
            var paypal_options = configuration.GetSection("OAuth:Paypal").Get<PaypalAuthenticationOptions>();
            if (paypal_options != null)
            {
                Handlers.Add(PaypalAuthenticationDefaults.AuthenticationScheme, typeof(PaypalAuthenticationHandler));

                authBuilder.AddPaypal(x =>
                {
                    x.ClientId = paypal_options.ClientId ?? unconfig;
                    x.ClientSecret = paypal_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region QQ (/signin-qq)
            var qq_options = configuration.GetSection("OAuth:QQ").Get<QQOptions>();
            if (qq_options != null)
            {
                Handlers.Add(QQDefaults.AuthenticationScheme, typeof(QQHandler));

                authBuilder.AddQQ(x =>
                {
                    x.ClientId = qq_options.ClientId ?? unconfig;
                    x.ClientSecret = qq_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Reddit (/signin-reddit)
            var reddit_options = configuration.GetSection("OAuth:Reddit").Get<RedditAuthenticationOptions>();
            if (reddit_options != null)
            {
                Handlers.Add(RedditAuthenticationDefaults.AuthenticationScheme, typeof(RedditAuthenticationHandler));

                authBuilder.AddReddit(x =>
                {
                    x.ClientId = reddit_options.ClientId ?? unconfig;
                    x.ClientSecret = reddit_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Salesforce (/signin-salesforce)
            var salesforce_options = configuration.GetSection("OAuth:Salesforce").Get<SalesforceAuthenticationOptions>();
            if (salesforce_options != null)
            {
                Handlers.Add(SalesforceAuthenticationDefaults.AuthenticationScheme, typeof(SalesforceAuthenticationHandler));

                authBuilder.AddSalesforce(x =>
                {
                    x.ClientId = salesforce_options.ClientId ?? unconfig;
                    x.ClientSecret = salesforce_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Twitter (/signin-twitter)
            var twitter_options = configuration.GetSection("OAuth:Twitter").Get<TwitterOptions>();
            if (twitter_options != null)
            {
                Handlers.Add(TwitterDefaults.AuthenticationScheme, typeof(TwitterHandler2));

                authBuilder.AddTwitter2(x =>
                {
                    x.ConsumerKey = twitter_options.ConsumerKey ?? unconfig;
                    x.ConsumerSecret = twitter_options.ConsumerSecret ?? unconfig;
                });
            }
            #endregion

            #region VisualStudio (/signin-visualstudio)
            var visualstudio_options = configuration.GetSection("OAuth:VisualStudio").Get<VisualStudioAuthenticationOptions>();
            if (visualstudio_options != null)
            {
                Handlers.Add(VisualStudioAuthenticationDefaults.AuthenticationScheme, typeof(VisualStudioAuthenticationHandler));

                authBuilder.AddVisualStudio(x =>
                {
                    x.ClientId = visualstudio_options.ClientId ?? unconfig;
                    x.ClientSecret = visualstudio_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Weibo (/signin-weibo)
            var weibo_options = configuration.GetSection("OAuth:Weibo").Get<WeiboOptions>();
            if (weibo_options != null)
            {
                Handlers.Add(WeiboDefaults.AuthenticationScheme, typeof(WeiboHandler));

                authBuilder.AddWeibo(x =>
                {
                    x.ClientId = weibo_options.ClientId ?? unconfig;
                    x.ClientSecret = weibo_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region Weixin (/signin-weixin)
            var weixin_options = configuration.GetSection("OAuth:Weixin").Get<WeixinOptions>();
            if (weixin_options != null)
            {
                Handlers.Add(WeixinDefaults.AuthenticationScheme, typeof(WeixinHandler));

                authBuilder.AddWeixin(x =>
                {
                    x.ClientId = weixin_options.ClientId ?? unconfig;
                    x.ClientSecret = weixin_options.ClientSecret ?? unconfig;
                });
            }
            #endregion

            #region WordPress (/signin-wordpress)
            var wordpress_options = configuration.GetSection("OAuth:WordPress").Get<WordPressAuthenticationOptions>();
            if (wordpress_options != null)
            {
                Handlers.Add(WordPressAuthenticationDefaults.AuthenticationScheme, typeof(WordPressAuthenticationHandler));

                authBuilder.AddWordPress(x =>
                {
                    x.ClientId = wordpress_options.ClientId ?? unconfig;
                    x.ClientSecret = wordpress_options.ClientSecret ?? unconfig;
                });
            }
            #endregion
        }

        public static readonly Dictionary<string, Type> Handlers = new Dictionary<string, Type>();
    }
}