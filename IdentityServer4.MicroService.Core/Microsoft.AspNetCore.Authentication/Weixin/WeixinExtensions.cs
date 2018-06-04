using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Weixin;

namespace Microsoft.Extensions.DependencyInjection
{
        /// <summary>
        /// Extension methods to add Weixin authentication capabilities to an HTTP application pipeline.
        /// </summary>
        public static class WeixinExtensions
        {
            public static AuthenticationBuilder AddWeixin(this AuthenticationBuilder builder)
            => builder.AddWeixin(WeixinDefaults.AuthenticationScheme, options => { });

            public static AuthenticationBuilder AddWeixin(
                this AuthenticationBuilder builder,
                Action<WeixinOptions> configuration)=>
             builder.AddWeixin(WeixinDefaults.AuthenticationScheme, configuration);
            
            public static AuthenticationBuilder AddWeixin(
                this AuthenticationBuilder builder, string scheme,
                Action<WeixinOptions> configuration)=>
                builder.AddWeixin(scheme, WeixinDefaults.DisplayName, configuration);

            public static AuthenticationBuilder AddWeixin(
                this AuthenticationBuilder builder,
                string scheme, string caption,
                Action<WeixinOptions> configuration)=>
                builder.AddOAuth<WeixinOptions, WeixinHandler>(scheme, caption, configuration);
        }
}
