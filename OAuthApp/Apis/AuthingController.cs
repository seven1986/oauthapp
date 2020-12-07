using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OAuthApp.Controllers;
using OAuthApp.Models.Apis.AuthingController;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.ConsentController;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// Authing
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Authing")]
    [SwaggerTag("#### 授权服务")]
    public class AuthingController: ApiControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;

        #region 构造函数
        public AuthingController(
            IIdentityServerInteractionService interaction,
            IStringLocalizer<ConsentController> localizer)
        {
            _interaction = interaction;
            l = localizer;
        }
        #endregion

        [HttpPost("Consent")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:authing.consent")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:authing.consent")]
        [SwaggerOperation(
           OperationId = "AuthingConsent",
           Summary = "授权服务 - 授权",
           Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.authing.consent | authing.authing.consent |")]
        public ApiResult<bool> Consent([FromBody]AuthingConsentRequest value)
        {
            var request = _interaction.GetAuthorizationContextAsync(value.ReturnUrl).Result;

            if(request==null)
            {
                return new ApiResult<bool>(false);
            }

            var grantedConsent = new ConsentResponse()
            {
                ScopesValuesConsented = value.ScopesConsented,
                Description = value.Description,
                RememberConsent = value.RememberConsent
            };

            _interaction.GrantConsentAsync(request, grantedConsent).Wait();

            return new ApiResult<bool>(true);
        }

        [HttpPost("Scopes")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:authing.scopes")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:authing.scopes")]
        [SwaggerOperation(
            OperationId = "AuthingScopes",
            Summary = "授权服务 - 权限列表",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.authing.scopes | authing.authing.scopes |")]
        public ApiResult<AuthingScopesReponse> Scopes([FromBody] AuthingConsentRequest value)
        {
            var request = _interaction.GetAuthorizationContextAsync(value.ReturnUrl).Result;

            var result = new AuthingScopesReponse
            {
                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
            };

            result.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateIdentityScope(x, value.ScopesConsented.Contains(x.Name) || value == null)).ToArray();

            var apiScopes = new List<AuthingScopeItem>();

            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);

                if (apiScope != null)
                {
                    var scopeVm = CreateApiScope(parsedScope, apiScope, value.ScopesConsented.Contains(parsedScope.RawValue) || value == null);

                    apiScopes.Add(scopeVm);
                }
            }
            if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(CreateOfflineAccessScope(value.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || value == null));
            }
            result.ApiScopes = apiScopes;

            return new ApiResult<AuthingScopesReponse>(result);
        }

        private AuthingScopeItem CreateIdentityScope(IdentityResource identity, bool check)
        {
            return new AuthingScopeItem
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }
        public AuthingScopeItem CreateApiScope(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;

            if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new AuthingScopeItem
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }
        private AuthingScopeItem CreateOfflineAccessScope(bool check)
        {
            return new AuthingScopeItem
            {
                Value = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
