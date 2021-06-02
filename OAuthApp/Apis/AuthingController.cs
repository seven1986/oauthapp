using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OAuthApp.Data;
using OAuthApp.Enums;
using OAuthApp.Models.Apis.AuthingController;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.ConsentController;
using OAuthApp.Models.Shared;
using OAuthApp.Services;
using OAuthApp.Tenant;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IEventService _events;
        private readonly IUserSession _userSession;
        private readonly IEmailSender _emailSender;
        private readonly UrlEncoder _urlEncoder;

        #region 构造函数
        public AuthingController(
            SignInManager<AppUser> SignInManager,
            UserManager<AppUser> userManager,
        IIdentityServerInteractionService interaction,
         IUserSession userSession,
            IStringLocalizer<AuthingController> localizer,
            IEventService events,
            UserDbContext _db,
            IEmailSender emailSender,
            UrlEncoder urlEncoder)
        {
            l = localizer;
            _userSession = userSession;
            _interaction = interaction;
            _SignInManager = SignInManager;
            _userManager = userManager;
            _events = events;
            db = _db;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
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

        #region 辅助方法
        private static AuthingScopeItem CreateIdentityScope(IdentityResource identity, bool check)
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
        private static AuthingScopeItem CreateApiScope(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
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
        #endregion
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

        #region 授权 - 登出
        /// <summary>
        /// 授权 - 登出
        /// </summary>
        /// <returns></returns>
        [HttpGet("SignOut")]
        [SwaggerOperation(
           OperationId = "AuthingSignOut",
           Summary = "授权 - 登出")]
        public new async Task<ApiResult<bool>> SignOut()
        {
            await _SignInManager.SignOutAsync();

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

        #region 授权 - 第三方登录服务
        /// <summary>
        /// 授权 - 第三方登录服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExternalLogins")]
        [SwaggerOperation(
            OperationId = "AuthingExternalLogins",
            Summary = "授权 - 第三方登录")]
        public async Task<ApiResult<List<string>>> ExternalLogins()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var result = (await _SignInManager.GetExternalAuthenticationSchemesAsync()).Select(x => x.Name).ToList();

            return new ApiResult<List<string>>(result);
        }
        #endregion

        #region 授权 - 使用第三方登录
        /// <summary>
        /// 授权 - 使用第三方登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExternalLogin")]
        [SwaggerOperation(
            OperationId = "AuthingExternalLogin",
            Summary = "授权 - 第三方登录")]
        public ChallengeResult ExternalLogin([FromQuery]string provider, [FromQuery]string returnUrl)
        {
            if (!returnUrl.StartsWith(AuthorizeEndpoint))
            {
                returnUrl = returnUrl.Substring(returnUrl.IndexOf(AuthorizeEndpoint));
            }

            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("ExternalLoginCallback"),
                Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
            };

            return Challenge(props, provider);
        }
        #endregion

        #region 授权 - 使用第三方登录回调
        /// <summary>
        /// 授权 - 使用第三方登录回调
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExternalLoginCallback")]
        [SwaggerOperation(
            OperationId = "ExternalLoginCallback",
            Summary = "授权 - 第三方登录")]
        [ApiExplorerSettings(IgnoreApi =true)]
        public RedirectResult ExternalLoginCallback()
        {
            // read external identity from the temporary cookie
            var result = HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).Result;

            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = FindUserFromExternalProvider(result);

            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
                user = AutoProvisionUser(provider, providerUserId, claims);
            }
            else
            {
                var picture = claims.FirstOrDefault(x => JwtClaimTypes.Picture == x.Type || ClaimTypes.Uri == x.Type);

                if (picture != null)
                {
                    user.Avatar = picture.Value;

                    var updateResult = _userManager.UpdateAsync(user).Result;
                }
            }

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            //ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            //ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), user.UserName)).Wait();

            _SignInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims.ToArray()).Wait();

            // delete temporary cookie used during external authentication
            // HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).Wait();

            // validate return URL and redirect back to authorization endpoint or a local page
            var returnUrl = result.Properties.Items["returnUrl"];

            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }
        #endregion

        #region 授权 - 已绑定的第三方登录
        /// <summary>
        /// 授权 - 已绑定的第三方登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("MyLogins")]
        [SwaggerOperation(
            OperationId = "AuthingMyLogins",
            Summary = "授权 - 已绑定的第三方登录")]
        public async Task<ApiResult<MyExternalLoginsResponse>> MyExternalLogins()
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());

            if (user == null)
            {
                return new ApiResult<MyExternalLoginsResponse>(l, BasicControllerEnums.NotFound);
            }

            var _Logins = _userManager.GetLoginsAsync(user).Result.ToList();

            var _OtherLogins = (await _SignInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => _Logins.All(ul => auth.Name != ul.LoginProvider))
                .Select(x => x.Name).ToList();

            return new ApiResult<MyExternalLoginsResponse>(new MyExternalLoginsResponse()
            {
                Logins = _Logins,
                OtherLogins = _OtherLogins
            });
        }
        #endregion

        #region 授权 - 绑定第三方登录
        /// <summary>
        /// 授权 - 绑定第三方登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("LinkExternalLogin")]
        [SwaggerOperation(
            OperationId = "AuthingLinkExternalLogin",
            Summary = "授权 - 绑定第三方登录")]
        public ActionResult LinkExternalLogin([FromQuery]string provider, [FromQuery]string redirectUrl)
        {
            var UserID = GetUserIdFromSession();

            if (string.IsNullOrWhiteSpace(UserID))
            {
                throw new Exception("no user sign in");
            }
           
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).Wait();

            var RedirectUri = Url.Action("LinkExternalLoginCallback");

            var properties = _SignInManager.ConfigureExternalAuthenticationProperties(provider, RedirectUri, UserId.ToString());

            properties.Items.Add("redirectUrl", redirectUrl);
            properties.Items.Add("userId", UserID);
            properties.Items.Add("scheme", provider);

            return Challenge(properties, provider);
        }
        #endregion

        #region 授权 - 绑定第三方登录回调
        /// <summary>
        /// 授权 - 绑定第三方登录回调
        /// </summary>
        /// <returns></returns>
        [HttpGet("LinkExternalLoginCallback")]
        [SwaggerOperation(
            OperationId = "AuthingLinkExternalLoginCallback",
            Summary = "授权 - 绑定第三方登录回调")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> LinkExternalLoginCallback()
        {
            var authResult = HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).Result;

            if (authResult?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            var (_, _provider, providerUserId, _) = FindUserFromExternalProvider(authResult);

            var userId = authResult.Properties.Items["userId"];

            AppUser user = _userManager.FindByIdAsync(userId).Result;

            var result = await _SignInManager.UserManager.AddLoginAsync(user, new UserLoginInfo(_provider, providerUserId, userId));

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var redirectUrl = authResult.Properties.Items["redirectUrl"];

            if (!result.Succeeded)
            {
                return Redirect(redirectUrl + "?error=" + JsonConvert.SerializeObject(result.Errors));
            }
            else
            {
                return Redirect(redirectUrl);
            }
        }
        #endregion

        #region 授权 - 移除指定第三方登录
        /// <summary>
        /// 授权 - 移除指定第三方登录
        /// </summary>
        /// <returns></returns>
        [HttpDelete("MyLogins")]
        [SwaggerOperation(
            OperationId = "AuthingRemoveMyLogins",
            Summary = "授权 - 移除指定第三方登录")]
        public async Task<ApiResult<bool>> RemoveMyLogins([FromQuery] string loginProvider, [FromQuery] string providerKey)
        {
            var UserId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var logins = await _userManager.GetLoginsAsync(user);
            
            if(logins.Count<2)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = "无法删除最后一个第三方登录"
                };
            }

            var result = await _SignInManager.UserManager.RemoveLoginAsync(user, loginProvider, providerKey);

            if (!result.Succeeded)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError, "The external login was not removed.");
            }

            await _SignInManager.RefreshSignInAsync(user);

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 修改密码
        /// <summary>
        /// 授权 - 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPut("ChangePassword")]
        [SwaggerOperation(
            OperationId = "AuthingChangePassword",
            Summary = "授权 - 修改密码")]
        public async Task<ApiResult<bool>> ChangePassword([FromBody] AuthingChangePasswordRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = ModelErrors()
                };
            }

            var userId = GetUserIdFromSession();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, value.OldPassword, value.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = JsonConvert.SerializeObject(changePasswordResult.Errors)
                };
            }

            await _SignInManager.RefreshSignInAsync(user);

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 修改手机号
        /// <summary>
        /// 授权 - 修改手机号
        /// </summary>
        /// <returns></returns>
        [HttpPut("ChangePhone")]
        [SwaggerOperation(
            OperationId = "AuthingChangePhone",
            Summary = "授权 - 修改手机号")]
        public async Task<ApiResult<bool>> ChangePhone([FromBody] AuthingAuthinChangePhoneRequest value)
        {
            if(!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = ModelErrors()
                };
            }

            var userId = GetUserIdFromSession();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (value.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, value.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                    {
                        message = JsonConvert.SerializeObject(setPhoneResult.Errors)
                    };
                }
            }

            await _SignInManager.RefreshSignInAsync(user);

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 修改邮箱
        /// <summary>
        /// 授权 - 修改邮箱
        /// </summary>
        /// <returns></returns>
        [HttpPut("ChangeEmail")]
        [SwaggerOperation(
            OperationId = "AuthingChangeEmail",
            Summary = "授权 - 修改邮箱")]
        public async Task<ApiResult<bool>> ChangeEmail([FromBody] AuthinChangeEmailRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = ModelErrors()
                };
            }

            var userId = GetUserIdFromSession();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            var email = await _userManager.GetEmailAsync(user);

            if (value.NewEmail != email)
            {
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, value.NewEmail);

                var callbackUrl = Url.Page(
                    "/user/confirm-email-change",
                    pageHandler: null,
                    values: new { userId = userId, email = value.NewEmail, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    value.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 修改邮箱确认
        /// <summary>
        /// 授权 - 修改邮箱确认
        /// </summary>
        /// <returns></returns>
        [HttpPut("ChangeEmailConfirm")]
        [SwaggerOperation(
            OperationId = "AuthingChangeEmailConfirm",
            Summary = "授权 - 修改邮箱确认")]
        public async Task<ApiResult<bool>> ChangeEmailConfirm([FromQuery]string userId, [FromQuery] string email, [FromQuery] string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ChangeEmailAsync(user, email, code);

            if (!result.Succeeded)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = JsonConvert.SerializeObject(result.Errors)
                };
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.HasError)
                {
                    message = JsonConvert.SerializeObject(result.Errors)
                };
            }

            await _SignInManager.RefreshSignInAsync(user);

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 授权 - 启用双认证
        /// <summary>
        /// 授权 - 启用双认证
        /// </summary>
        /// <returns></returns>
        [HttpPut("EnableAuthenticator")]
        [SwaggerOperation(
            OperationId = "AuthingEnableAuthenticator",
            Summary = "授权 - 启用双认证")]
        public async Task<ApiResult<EnableAuthenticatorResponse>> EnableAuthenticator([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResult<EnableAuthenticatorResponse>(l, BasicControllerEnums.NotFound);
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var SharedKey = FormatKey(unformattedKey);
            
            var AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);

            return new ApiResult<EnableAuthenticatorResponse>(new EnableAuthenticatorResponse()
            {
                AuthenticatorUri = AuthenticatorUri,
                SharedKey = SharedKey
            });
        }
        #endregion

        #region 授权 - 重置双认证密钥
        /// <summary>
        /// 授权 - 重置双认证密钥
        /// </summary>
        /// <returns></returns>
        [HttpPut("ResetAuthenticator")]
        [SwaggerOperation(
            OperationId = "AuthingResetAuthenticator",
            Summary = "授权 - 重置双认证密钥")]
        public async Task<ApiResult<bool>> ResetAuthenticator([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);

            await _userManager.ResetAuthenticatorKeyAsync(user);

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 辅助方法
        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("WebApplication2"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }


        private string GetUserIdFromSession()
        {
            if (User.Identity.IsAuthenticated)
            {
                return UserId.ToString();
            }

            var user = _userSession.GetUserAsync().Result;

            if (user != null)
            {
                return ((ClaimsIdentity)user.Identity).Claims.
                    FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Subject) || x.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            }

            else 
            {
                var authResult = HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).Result;

                if (authResult?.Succeeded == true)
                {
                    return authResult.Principal.Claims.FirstOrDefault(x =>
                    x.Type.Equals(JwtClaimTypes.Subject) ||
                    x.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                }
            }

            return string.Empty;
        }

        private (AppUser user, 
            string provider, 
            string providerUserId,
            List<Claim> claims) FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = _SignInManager.UserManager.FindByLoginAsync(provider, providerUserId).Result;

            return (user, provider, providerUserId, claims);
        }

        /// <summary>
        /// 自动生成用户
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        private AppUser AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            var userClaims = new List<AppUserClaim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    userClaims.Add(new AppUserClaim() { ClaimType = JwtClaimTypes.Name, ClaimValue = claim.Value });
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    userClaims.Add(new AppUserClaim() {
                        ClaimType = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], ClaimValue = claim.Value });
                }
                // copy the claim as-is
                else
                {
                    userClaims.Add(new AppUserClaim() { ClaimType = claim.Type, ClaimValue = claim.Value });
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            if (!userClaims.Any(x => x.ClaimType == JwtClaimTypes.Name))
            {
                var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    userClaims.Add(new AppUserClaim() { ClaimType = JwtClaimTypes.Name, ClaimValue = first + " " + last });
                }
                else if (first != null)
                {
                    userClaims.Add(new AppUserClaim() { ClaimType = JwtClaimTypes.Name, ClaimValue = first });
                }
                else if (last != null)
                {
                    userClaims.Add(new AppUserClaim() { ClaimType = JwtClaimTypes.Name, ClaimValue = last });
                }
            }

            var name = userClaims.FirstOrDefault(c => c.ClaimType == JwtClaimTypes.Name)?.ClaimValue;
            var openid = userClaims.FirstOrDefault(x => x.ClaimType == JwtClaimTypes.Subject)?.ClaimValue;
            var email = userClaims.FirstOrDefault(x => x.ClaimType == JwtClaimTypes.Email)?.ClaimValue;
            var user = new AppUser
            {
                Email = email,
                NickName = name
            };

            var picture = claims.FirstOrDefault(x => JwtClaimTypes.Picture == x.Type || ClaimTypes.Uri == x.Type);

            if(picture!=null)
            {
                user.Avatar = picture.Value;
            }

            user.UserName = user.UserKey.ToString("n");

            user.Claims.AddRange(userClaims);

            user.Logins.Add(new AppUserLogin()
            {
                LoginProvider = provider,
                ProviderKey = userId,
                ProviderDisplayName = provider
            });

            var roleIds = db.Roles.Where(x => x.Name.Equals(DefaultRoles.User)).Select(x => x.Id).ToList();

            var result = AppUserService.CreateUser(
                  TenantId,
                 _SignInManager.UserManager,
                  db,
                  user,
                  roleIds,
                  $"{MicroServiceName}.all",
               new List<long>() { TenantId }).Result;

            if(result.Succeeded)
            {
                return user;
            }

            throw new Exception(JsonConvert.SerializeObject(result.Errors));
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }
        #endregion
    }
}
