using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.EntityFramework.DbContexts;
using static IdentityServer4.MicroService.MicroserviceConfig;
using static IdentityServer4.MicroService.AppDefaultData;
using IdentityServer4.MicroService.Attributes;
using System.Text.RegularExpressions;
using IdentityServer4.MicroService.Host.Models.Views.Account;
using identity = Microsoft.AspNetCore.Identity;

namespace IdentityServer4.MicroService.Host.Controllers
{
    [Authorize]
    public class AccountController : BasicController
    {
        #region services
        private AzureApiManagementServices _azureApim;
        public AzureApiManagementServices AzureApim
        {
            get
            {
                if (_azureApim == null)
                {

                    if (pvtTenant.Properties.ContainsKey(AzureApiManagementKeys.Host) &&
                    pvtTenant.Properties.ContainsKey(AzureApiManagementKeys.ApiId) &&
                    pvtTenant.Properties.ContainsKey(AzureApiManagementKeys.ApiKey))
                    {
                        _azureApim = new AzureApiManagementServices(
                            pvtTenant.Properties[AzureApiManagementKeys.Host],
                            pvtTenant.Properties[AzureApiManagementKeys.ApiId],
                            pvtTenant.Properties[AzureApiManagementKeys.ApiKey]);
                    }
                }

                return _azureApim;
            }
        }
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailService _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly AccountService _account;
        private readonly IdentityDbContext _userContext;
        private readonly ConfigurationDbContext _configDbContext;
        #endregion

        #region 构造函数
        public AccountController(
           IIdentityServerInteractionService interaction,
           IHttpContextAccessor httpContextAccessor,
           UserManager<AppUser> userManager,
           SignInManager<AppUser> signInManager,
           EmailService emailSender,
           ISmsSender smsSender,
           IdentityDbContext userContext,
           ConfigurationDbContext configDbContext,
           TenantService _tenantService,
           TenantDbContext _tenantDb,
           ILogger<AccountController> _logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;

            _account = new AccountService(interaction, httpContextAccessor);
            _userContext = userContext;
            _configDbContext = configDbContext;

            tenantService = _tenantService;
            tenantDb = _tenantDb;
            logger = _logger;
        }
        #endregion

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        bool IsEmail(string value)
        {
            return Regex.IsMatch(value, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }
        bool IsMobile(string value)
        {
            return Regex.IsMatch(value, @"^[1]+[1,9]+\d{9}");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                identity.SignInResult signinResult = null;

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                // 邮箱登录
                if (IsEmail(model.Username))
                {
                    signinResult = await _signInManager.PasswordSignInAsync(model.Username,
                        model.Password, model.RememberMe, 
                        lockoutOnFailure: false);
                }

                // 手机号登录
                else if (IsMobile(model.Username))
                {
                    
                    var user = _userContext.Users.Where(x => x.PhoneNumberConfirmed == true && 
                    x.PhoneNumber.Equals(model.Username)).FirstOrDefault();

                    if (user != null)
                    {
                        signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, 
                            model.RememberMe, lockoutOnFailure: false);
                    }
                }

                // 昵称登录
                else
                {
                    var user = await _userManager.FindByNameAsync(model.Username);

                    if (user != null)
                    {
                        signinResult = await _signInManager.PasswordSignInAsync(user, model.Password,
                            model.RememberMe, lockoutOnFailure: false);
                    }
                }

                if (signinResult.Succeeded)
                {
                    logger.LogInformation(1, "User logged in.");

                    return RedirectToLocal(returnUrl, model.Username, model.Password);
                }

                if (signinResult.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, model.RememberMe });
                }

                if (signinResult.IsLockedOut)
                {
                    logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null, int Referee = 1)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Referee"] = 1;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                /*以存在的邮箱账号，但没有验证通过*/
                var existedUser = await _userManager.FindByEmailAsync(model.Email);

                if (existedUser != null)
                {
                    var IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(existedUser);

                    if (!IsEmailConfirmed)
                    {
                        await SendActiveEmail(existedUser);
                    }

                    ModelState.AddModelError(string.Empty, "已存在的账号");

                    return View(model);
                }


                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PasswordHash = model.Password,
                    ParentUserID = model.ParentUserID,
                    Permission = "-10,1,2,3,4,5,27,-11,6,8,9,11"
                };

                var result = await CreateUser(user);

                if (result)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    logger.LogInformation(3, "User created a new account with password.");

                    return RedirectToLocal(returnUrl, model.Email, model.Password);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout2()
        {
            await _signInManager.SignOutAsync();
            logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);

                return RedirectToLocal(returnUrl, user.Email, null);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new AppUser { UserName = model.Email, Email = model.Email };

                /*以存在的邮箱账号，但没有验证通过*/
                var existedUser = await _userManager.FindByEmailAsync(model.Email);

                if (existedUser != null)
                {
                    var IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(existedUser);

                    if (!IsEmailConfirmed)
                    {
                        await SendActiveEmail(existedUser);
                    }

                    ModelState.AddModelError(string.Empty, "已存在的账号");

                    return View(model);
                }

                var result = await CreateUser(user);

                if (result)
                {
                    var AddLoginResult = await _userManager.AddLoginAsync(user, info);

                    if (AddLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);

                        return RedirectToLocal(returnUrl, null, null);
                    }

                    AddErrors(AddLoginResult);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action(nameof(ResetPassword),
                    "Account",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);

                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                //var xsmtpapi = JsonConvert.SerializeObject(new
                //{
                //    to = new string[] { model.Email },
                //    sub = new Dictionary<string, string[]>()
                //    {
                //        { "%callbackUrl%", new string[] { callbackUrl } }
                //    }
                //});

                var result = await _emailSender.SendEmailAsync(
                    SendCloudMailTemplates.reset_password,
                    new string[] { model.Email },
                     new Dictionary<string, string[]>()
                     {
                         { "%callbackUrl%",new string[] { callbackUrl }}
                     });

                //await _emailSender.SendEmailAsync("reset password", "reset_password", xsmtpapi);

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;

            if (model.SelectedProvider == "Email")
            {
                //await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);

                var result = await _emailSender.SendEmailAsync(
                    SendCloudMailTemplates.security_code,
                    new string[] { await _userManager.GetEmailAsync(user) },
                    new Dictionary<string, string[]>()
                    {
                        { "%code%",new string[] { code }}
                    });
            }

            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(JsonConvert.SerializeObject(new { code }),
                    await _userManager.GetPhoneNumberAsync(user),
                    "9812");
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl, null, null);
            }
            if (result.IsLockedOut)
            {
                logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        //
        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await _account.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            await _signInManager.SignOutAsync();

            var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);

            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.SignOutAsync(vm.ExternalAuthenticationScheme,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException) // this is for the external providers that don't have signout
                {

                }
                catch (InvalidOperationException) // this is for Windows/Negotiate
                {

                }
            }

            // delete local authentication cookie
            await HttpContext.SignOutAsync();

            return View("LoggedOut", vm);
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #region 注册用户默认权限
        static string _defaultUserPermissions;
        public static string DefaultUserPermissions
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultUserPermissions))
                {
                    var permissions = typeof(UserPermissions).GetFields()
              .Where(x => x.GetCustomAttribute<PolicyClaimValuesAttribute>().IsDefault)
              .Select(x => x.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues[0]).ToList();

                    _defaultUserPermissions = string.Join(",", permissions);
                }

                return _defaultUserPermissions;
            }
        }
        #endregion

        #region 注册用户默认角色
        static List<long> DefaultUserRoleIds = new List<long>(); 
        #endregion

        private async Task<bool> CreateUser(AppUser user)
        {
            if (DefaultUserRoleIds.Count < 1)
            {
                DefaultUserRoleIds = _userContext.Roles.Where(x => x.Name.Equals(Roles.Users) || x.Name.Equals(Roles.Developer))
                      .Select(x => x.Id).ToList();
            }

            var tenantIds = tenantDb.Tenants.Select(x => x.Id).ToList();

            var result = await AppUserService.CreateUser(
                pvtTenant.Id,
                _userManager,
                _userContext,
                user,
                DefaultUserRoleIds,
                DefaultUserPermissions,
                tenantIds);

            if (result.Succeeded)
            {
                var sendResult = await SendActiveEmail(user);

                return true;
            }

            AddErrors(result);

            return false;
        }

        #region 发送激活邮件
        async Task<bool> SendActiveEmail(AppUser user)
        {
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
                new { userId = user.Id, code },
                protocol: HttpContext.Request.Scheme);

            var sendEmailResult = await _emailSender.SendEmailAsync(
                SendCloudMailTemplates.test_template_active,
                new string[] { user.Email },
                new Dictionary<string, string[]>()
                {
                        { "%name%", new string[] { user.Email } },
                        { "%url%", new string[] { callbackUrl } },
                });

            return sendEmailResult;
        }
        #endregion

        private IActionResult RedirectToLocal(string returnUrl, string email, string password = "666666")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                try
                {
                    if (pvtTenant.Properties.ContainsKey(AzureApiManagementKeys.PortalUris))
                    {
                        var portalUris = pvtTenant.Properties[AzureApiManagementKeys.PortalUris]
                             .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        if (portalUris.Length > 0)
                        {
                            var url = new Uri(returnUrl);

                            if (portalUris.Contains(url.Host))
                            {
                                var userId = 0L;

                                if (IsEmail(email))
                                {
                                    userId = _userContext.Users
                                        .Where(x => x.Email.Equals(email))
                                        .Select(x => x.Id).FirstOrDefault();
                                }
                                else
                                {
                                    userId = _userContext.Users
                                       .Where(x => x.UserName.Equals(email))
                                       .Select(x => x.Id).FirstOrDefault();

                                    email += "@xxx.com";
                                }

                                var addResult = AzureApim.Users.AddAsync(userId.ToString(), email, password).Result;

                                if (addResult)
                                {
                                    var result = AzureApim.Users.GenerateSsoUrlAsync(userId.ToString()).Result;

                                    return Redirect(result);
                                }
                            }
                        }
                    }
                }
                catch
                {

                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }
}
