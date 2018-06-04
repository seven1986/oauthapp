// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GoogleExtensions2
    {
        public static AuthenticationBuilder AddGoogle2(this AuthenticationBuilder builder)
            => builder.AddGoogle2(GoogleDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddGoogle2(this AuthenticationBuilder builder, Action<GoogleOptions> configureOptions)
            => builder.AddGoogle2(GoogleDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddGoogle2(this AuthenticationBuilder builder, string authenticationScheme, Action<GoogleOptions> configureOptions)
            => builder.AddGoogle2(authenticationScheme, GoogleDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddGoogle2(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GoogleOptions> configureOptions)
            => builder.AddOAuth<GoogleOptions, GoogleHandler2>(authenticationScheme, displayName, configureOptions);
    }
}
