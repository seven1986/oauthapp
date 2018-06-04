using AspNet.Security.OAuth.Amazon;
using AspNet.Security.OAuth.Gitter;
using AspNet.Security.OAuth.Instagram;
using AspNet.Security.OAuth.LinkedIn;
using AspNet.Security.OAuth.Paypal;
using AspNet.Security.OAuth.Reddit;
using AspNet.Security.OAuth.Salesforce;
using AspNet.Security.OAuth.VisualStudio;
using AspNet.Security.OAuth.WordPress;
using IdentityServer4.MicroService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.Weixin;

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

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="authBuilder">The services.</param>
        /// <returns></returns>
        public static void AddIdentityServer4MicroServiceOAuths(this AuthenticationBuilder authBuilder)
        {
            #region Amazon (/signin-amazon)
            authBuilder.AddAmazon(x =>
            {
                var ClientId = $"{AmazonAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{AmazonAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Facebook (/signin-facebook)
            authBuilder.AddFacebook2(x =>
            {
                var ClientId = $"{FacebookDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{FacebookDefaults.AuthenticationScheme}:ClientSecret";
                x.AppId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.AppSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region GitHub (/signin-github)
            authBuilder.AddGitHub(x =>
            {
                var ClientId = $"{GitHubDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{GitHubDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Gitter (/signin-gitter)
            authBuilder.AddGitter(x =>
            {
                var ClientId = $"{GitterAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{GitterAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Google (/signin-google)
            authBuilder.AddGoogle2(x =>
            {
                var ClientId = $"{GoogleDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{GoogleDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Instagram (/signin-instagram)
            authBuilder.AddInstagram(x =>
            {
                var ClientId = $"{InstagramAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{InstagramAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region LinkedIn (/signin-linkedin)
            authBuilder.AddLinkedIn(x =>
            {
                var ClientId = $"{LinkedInAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{LinkedInAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region MicrosoftAccount (/signin-microsoft)
            authBuilder.AddMicrosoftAccount2(x =>
            {
                var ClientId = $"{MicrosoftAccountDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{MicrosoftAccountDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Paypal (/signin-paypal)
            authBuilder.AddPaypal(x =>
            {
                var ClientId = $"{PaypalAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{PaypalAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region QQ (/signin-qq)
            authBuilder.AddQQ(x =>
            {
                var ClientId = $"{QQDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{QQDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Reddit (/signin-reddit)
            authBuilder.AddReddit(x =>
            {
                var ClientId = $"{RedditAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{RedditAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Salesforce (/signin-salesforce)
            authBuilder.AddSalesforce(x =>
            {
                var ClientId = $"{SalesforceAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{SalesforceAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Twitter (/signin-twitter)
            authBuilder.AddTwitter2(x =>
            {
                var ClientId = $"{TwitterDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{TwitterDefaults.AuthenticationScheme}:ClientSecret";
                x.ConsumerKey = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ConsumerSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region VisualStudio (/signin-visualstudio)
            authBuilder.AddVisualStudio(x =>
            {
                var ClientId = $"{VisualStudioAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{VisualStudioAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Weibo (/signin-weibo)
            authBuilder.AddWeibo(x =>
            {
                var ClientId = $"{WeiboDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{WeiboDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Weixin (/signin-weixin)
            authBuilder.AddWeixin(x =>
            {
                var ClientId = $"{WeixinDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{WeixinDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region WordPress (/signin-visualstudio)
            authBuilder.AddWordPress(x =>
            {
                var ClientId = $"{WordPressAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{WordPressAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion
        }
    }
}