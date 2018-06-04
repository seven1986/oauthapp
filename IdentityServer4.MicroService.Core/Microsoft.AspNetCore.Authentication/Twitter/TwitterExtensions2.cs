// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwitterExtensions2
    {
        public static AuthenticationBuilder AddTwitter2(this AuthenticationBuilder builder)
            => builder.AddTwitter2(TwitterDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddTwitter2(this AuthenticationBuilder builder, Action<TwitterOptions> configureOptions)
            => builder.AddTwitter2(TwitterDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddTwitter2(this AuthenticationBuilder builder, string authenticationScheme, Action<TwitterOptions> configureOptions)
            => builder.AddTwitter2(authenticationScheme, TwitterDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddTwitter2(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TwitterOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TwitterOptions>, TwitterPostConfigureOptions>());
            return builder.AddRemoteScheme<TwitterOptions, TwitterHandler2>(authenticationScheme, displayName, configureOptions);
        }
    }
}
