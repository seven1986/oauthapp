using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using System.Collections.Generic;
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

        public static List<string> Schemes = new List<string>() {
            MicrosoftAccountDefaults.AuthenticationScheme,
            GoogleDefaults.AuthenticationScheme,
            FacebookDefaults.AuthenticationScheme,
            GitHubAuthenticationDefaults.AuthenticationScheme,
            QQAuthenticationDefaults.AuthenticationScheme,
            WeiboAuthenticationDefaults.AuthenticationScheme,
            WeixinAuthenticationDefaults.AuthenticationScheme
        };

        const string unknow = "unknow";

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="authBuilder">The services.</param>
        /// <returns></returns>
        public static void AddIdentityServer4MicroServiceOAuths(this AuthenticationBuilder authBuilder)
        {
            #region MicrosoftAccount (/signin-microsoft)
            var microsoft_options = new MicrosoftAccountOptions();
            authBuilder.Services.AddSingleton(microsoft_options);
            authBuilder.AddMicrosoftAccount(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(MicrosoftAccountOptions), microsoft_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion

            #region Google (/signin-google)
            var google_options = new GoogleOptions();
            authBuilder.Services.AddSingleton(google_options);
            authBuilder.AddGoogle(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(GoogleOptions), google_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion

            #region Facebook (/signin-facebook)
            var facebook_options = new FacebookOptions();
            authBuilder.Services.AddSingleton(facebook_options);
            authBuilder.AddFacebook(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(FacebookOptions), facebook_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion

            #region GitHub (/signin-github)
            var github_options = new GitHubAuthenticationOptions();
            authBuilder.Services.AddSingleton(github_options);
            authBuilder.AddGitHub(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(GitHubAuthenticationOptions), github_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion

            #region QQ (/signin-qq)
            var qq_options = new QQAuthenticationOptions();
            authBuilder.Services.AddSingleton(qq_options);
            authBuilder.AddQQ(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(QQAuthenticationOptions), qq_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion

            #region Weibo (/signin-weibo)
            var weibo_options = new WeiboAuthenticationOptions();
            authBuilder.Services.AddSingleton(weibo_options);
            authBuilder.AddWeibo(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(WeiboAuthenticationOptions), weibo_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion

            #region Weixin (/signin-weixin)
            var weixin_options = new WeixinAuthenticationOptions();
            authBuilder.Services.AddSingleton(weixin_options);
            authBuilder.AddWeixin(x =>
            {
                x.ClientId = unknow;
                x.ClientSecret = unknow;
                authBuilder.Services.Remove(new ServiceDescriptor(typeof(WeixinAuthenticationOptions), weixin_options));
                authBuilder.Services.AddSingleton(x);
            });
            #endregion
        }
    }
}