// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FacebookAuthenticationOptionsExtensions2
    {
        public static AuthenticationBuilder AddFacebook2(this AuthenticationBuilder builder)
            => builder.AddFacebook2(FacebookDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddFacebook2(this AuthenticationBuilder builder, Action<FacebookOptions> configureOptions)
            => builder.AddFacebook2(FacebookDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddFacebook2(this AuthenticationBuilder builder, string authenticationScheme, Action<FacebookOptions> configureOptions)
            => builder.AddFacebook2(authenticationScheme, FacebookDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddFacebook2(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<FacebookOptions> configureOptions)
            => builder.AddOAuth<FacebookOptions, FacebookHandler2>(authenticationScheme, displayName, configureOptions);
    }
}
