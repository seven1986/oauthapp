/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    /// <summary>
    /// Defines a set of options used by <see cref="QQHandler"/>.
    /// </summary>
    public class QQOptions : OAuthOptions
    {
        public QQOptions()
        {
            ClaimsIssuer = QQDefaults.Issuer;
            CallbackPath = new PathString(QQDefaults.CallbackPath);

            AuthorizationEndpoint = QQDefaults.AuthorizationEndpoint;
            TokenEndpoint = QQDefaults.TokenEndpoint;
            UserIdentificationEndpoint = QQDefaults.UserIdentificationEndpoint;
            UserInformationEndpoint = QQDefaults.UserInformationEndpoint;

            Scope.Add("get_user_info");

            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");
            ClaimActions.MapJsonKey("urn:qq:picture", "figureurl");
            ClaimActions.MapJsonKey("urn:qq:picture_medium", "figureurl_1");
            ClaimActions.MapJsonKey("urn:qq:picture_full", "figureurl_2");
            ClaimActions.MapJsonKey("urn:qq:avatar", "figureurl_qq_1");
            ClaimActions.MapJsonKey("urn:qq:avatar_full", "figureurl_qq_2");
        }

        /// <summary>
        /// Gets or sets the URL of the user identification endpoint (aka "OpenID endpoint").
        /// </summary>
        public string UserIdentificationEndpoint { get; set; }
    }
}
