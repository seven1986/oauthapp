using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OAuthApp.Controllers;
using OAuthApp.Data;
using OAuthApp.Enums;
using OAuthApp.Models.Apis.AuthingController;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.ConsentController;
using OAuthApp.Tenant;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// Authing
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [ApiController]
    //[Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Authing")]
    [SwaggerTag("#### 授权")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [AllowAnonymous]
    public class AuthingController : ApiControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<AppUser> _SignInManager;

        #region 构造函数
        public AuthingController(
            SignInManager<AppUser> SignInManager,
            IIdentityServerInteractionService interaction,
            IStringLocalizer<ConsentController> localizer)
        {
            l = localizer;
            _interaction = interaction;
            _SignInManager = SignInManager;
        }
        #endregion

        #region 授权 - 登陆
        /// <summary>
        /// 授权 - 登陆
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("SignIn")]
        [SwaggerOperation(
            OperationId = "AuthingSignIn",
            Summary = "授权 - 登陆")]
        public async Task<ApiResult<AuthingSignInReponse>> SignIn([FromBody] AuthingSignInRequest value)
        {
            if (!value.ReturnUrl.StartsWith(AuthorizeEndpoint))
            {
                value.ReturnUrl = value.ReturnUrl.Substring(value.ReturnUrl.IndexOf(AuthorizeEndpoint));
            }

            var request = await _interaction.GetAuthorizationContextAsync(value.ReturnUrl);

            if (request == null)
            {
                return new ApiResult<AuthingSignInReponse>(l, BasicControllerEnums.HasError);
            }

            var result = await _SignInManager.PasswordSignInAsync(value.UserName, value.Password, true, false);

            return new ApiResult<AuthingSignInReponse>(new AuthingSignInReponse()
            {
                IsLockedOut = result.IsLockedOut,
                IsNotAllowed = result.IsNotAllowed,
                RequiresTwoFactor = result.RequiresTwoFactor,
                Succeeded = result.Succeeded,
                Message = result.ToString()
            });
        }
        #endregion

        #region 授权 - 预授权应用
        /// <summary>
        /// 授权 - 预授权应用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("PreConsent")]
        [SwaggerOperation(
            OperationId = "AuthingPreConsent",
            Summary = "授权 - 预授权应用")]
        public async Task<ApiResult<AuthingPreConsentReponse>> PreConsent([FromBody]AuthingPreConsentRequest value)
        {
            if (!value.ReturnUrl.StartsWith(AuthorizeEndpoint))
            {
                value.ReturnUrl = value.ReturnUrl.Substring(value.ReturnUrl.IndexOf(AuthorizeEndpoint));
            }

            var request = await _interaction.GetAuthorizationContextAsync(value.ReturnUrl);

            if (request == null)
            {
                return new ApiResult<AuthingPreConsentReponse>(l, BasicControllerEnums.HasError);
            }

            var tenant = HttpContext.GetTenantWithClaims();

            var result = new AuthingPreConsentReponse
            {
                Tenant = tenant,
                ClientName = request.Client.ClientName ?? request.Client.ClientId,
                ClientUrl = request.Client.ClientUri,
                ClientLogoUrl = request.Client.LogoUri,
                ClientDescription = request.Client.Description
            };

            result.IdentityScopes = request.ValidatedResources.Resources.IdentityResources
                .Select(x => CreateIdentityScope(x, true)).ToList();

            var apiScopes = new List<AuthingScopeItem>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateApiScope(parsedScope, apiScope, true);
                    apiScopes.Add(scopeVm);
                }
            }
            if (request.Client.AllowOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(CreateOfflineAccessScope(true));
            }
            result.ApiScopes = apiScopes;

            return new ApiResult<AuthingPreConsentReponse>(result);
        }
        #endregion

        #region 授权 - 授权应用
        /// <summary>
        /// 授权 - 授权应用
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("Consent")]
        [SwaggerOperation(
            OperationId = "AuthingConsent",
            Summary = "授权 - 授权应用")]
        public async Task<ApiResult<bool>> Consent([FromBody]AuthingConsentRequest value)
        {
            if (!value.ReturnUrl.StartsWith(AuthorizeEndpoint))
            {
                value.ReturnUrl = value.ReturnUrl.Substring(value.ReturnUrl.IndexOf(AuthorizeEndpoint));
            }

            var request = await _interaction.GetAuthorizationContextAsync(value.ReturnUrl);

            if (request == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError);
            }

            var grantedConsent = new ConsentResponse
            {
                RememberConsent = value.Remember,
                Description = value.Description,
                ScopesValuesConsented = value.Scopes
            };

            await _interaction.GrantConsentAsync(request, grantedConsent);

            if (grantedConsent.Error != null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError, grantedConsent.ErrorDescription);
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 已授权应用
        /// <summary>
        /// 授权 - 已授权应用
        /// </summary>
        /// <returns></returns>
        [HttpGet("Grants")]
        [SwaggerOperation(
            OperationId = "AuthingScopes",
            Summary = "授权 - 已授权应用")]
        public async Task<List<Grant>> Grants()
        {
            var result = await _interaction.GetAllUserGrantsAsync();

            return new List<Grant>(result);
        }
        #endregion

        #region 授权 - 撤销应用权限
        /// <summary>
        /// 授权 - 撤销应用权限
        /// </summary>
        /// <param name="clientId">应用ID</param>
        /// <returns></returns>
        [HttpDelete("Revoke")]
        [SwaggerOperation(
           OperationId = "AuthingRevoke",
           Summary = "授权 - 撤销应用权限")]
        public async Task<ApiResult<bool>> Revoke([FromQuery] string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 错误报告
        /// <summary>
        /// 授权 - 错误报告
        /// </summary>
        /// <param name="ErrorId"></param>
        /// <returns></returns>
        [HttpGet("ErrorReport")]
        [SwaggerOperation(
            OperationId = "AuthingErrorReport",
            Summary = "授权 - 错误报告")]
        public async Task<ApiResult<ErrorMessage>> ErrorReport([FromQuery]string ErrorId)
        {
            var result = await _interaction.GetErrorContextAsync(ErrorId);

            return new ApiResult<ErrorMessage>(result);
        }
        #endregion

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
        private AuthingScopeItem CreateApiScope(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
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
                Value = IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptions.OfflineAccessDisplayName,
                Description = ConsentOptions.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }
    }
}
