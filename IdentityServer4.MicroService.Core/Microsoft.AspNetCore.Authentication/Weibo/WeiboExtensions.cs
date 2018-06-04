/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Weibo;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to add Weibo authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class WeiboAuthenticationExtensions
    {
        public static AuthenticationBuilder AddWeibo(this AuthenticationBuilder builder) =>
             builder.AddWeibo(WeiboDefaults.AuthenticationScheme, options => { });

        public static AuthenticationBuilder AddWeibo(
            this AuthenticationBuilder builder,
            Action<WeiboOptions> configuration) =>
             builder.AddWeibo(WeiboDefaults.AuthenticationScheme, configuration);

        public static AuthenticationBuilder AddWeibo(
            this AuthenticationBuilder builder, string scheme,
            Action<WeiboOptions> configuration) =>
             builder.AddWeibo(scheme, WeiboDefaults.DisplayName, configuration);

        public static AuthenticationBuilder AddWeibo(
            this AuthenticationBuilder builder,
            string scheme, string caption,
            Action<WeiboOptions> configuration) =>
             builder.AddOAuth<WeiboOptions, WeiboHandler>(scheme, caption, configuration);
    }
}
