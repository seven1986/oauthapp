/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.GitHub;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to add GitHub authentication capabilities to an HTTP application pipeline.
    /// </summary>
    public static class GitHubAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGitHub(this AuthenticationBuilder builder) =>
             builder.AddGitHub(GitHubDefaults.AuthenticationScheme, options => { });

        public static AuthenticationBuilder AddGitHub(
            this AuthenticationBuilder builder,
            Action<GitHubOptions> configuration) =>
             builder.AddGitHub(GitHubDefaults.AuthenticationScheme, configuration);

        public static AuthenticationBuilder AddGitHub(
            this AuthenticationBuilder builder, string scheme,
            Action<GitHubOptions> configuration) =>
             builder.AddGitHub(scheme, GitHubDefaults.DisplayName, configuration);

        public static AuthenticationBuilder AddGitHub(
            this AuthenticationBuilder builder,
            string scheme, string caption,
            Action<GitHubOptions> configuration) =>
             builder.AddOAuth<GitHubOptions, GitHubHandler>(scheme, caption, configuration);
    }
}
