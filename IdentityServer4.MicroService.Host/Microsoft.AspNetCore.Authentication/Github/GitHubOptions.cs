/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication.GitHub
{
    /// <summary>
    /// Defines a set of options used by <see cref="GitHubHandler"/>.
    /// </summary>
    public class GitHubOptions : OAuthOptions
    {
        public GitHubOptions()
        {
            ClaimsIssuer = GitHubDefaults.Issuer;

            CallbackPath = new PathString(GitHubDefaults.CallbackPath);

            AuthorizationEndpoint = GitHubDefaults.AuthorizationEndpoint;
            TokenEndpoint = GitHubDefaults.TokenEndpoint;
            UserInformationEndpoint = GitHubDefaults.UserInformationEndpoint;

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            ClaimActions.MapJsonKey("urn:github:name", "name");
            ClaimActions.MapJsonKey("urn:github:url", "url");
        }

        /// <summary>
        /// Gets or sets the address of the endpoint exposing
        /// the email addresses associated with the logged in user.
        /// </summary>
        public string UserEmailsEndpoint { get; set; } = GitHubDefaults.UserEmailsEndpoint;
    }
}
