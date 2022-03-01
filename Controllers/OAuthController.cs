using OAuthApp.Data;
using OAuthApp.Models;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace OAuthApp.Controllers
{
    public class OAuthController : Controller
    {
        private readonly ILogger<OAuthController> _logger;
        private readonly IMemoryCache _cache;
        private readonly TokenProvider _tokenProvider;
        private readonly ITenantUser _tenantUser;
        private TenantContext _tenant;
        private readonly AppDbContext _context;

        public OAuthController(ILogger<OAuthController> logger,
            IMemoryCache cache,
            TokenProvider tokenProvider,
            IHttpContextAccessor contextAccessor,
            ITenantUser tenantUser,
            AppDbContext context)
        {
            _logger = logger;
            _cache = cache;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
            _tokenProvider = tokenProvider;
            _tenantUser = tenantUser;
            _context = context;
        }

        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult UserInfo()
        {
            return Json(_tokenProvider.User);
        }

        [Authorize]
        public IActionResult Consent([FromQuery][Required]string code)
        {
            var authCode = _cache.Get<AuthorizationCode>(code);

            if (authCode == null)
            {
                return Json(new { code = 500, err = "授权码过期" });
            }

            var clientItem = _context.Apps.FirstOrDefault(x => x.AppKey.Equals(authCode.client_id));

            if (clientItem == null)
            {
                return Json(new { code = 500, err = "应用" + authCode.client_id + "不存在" });
            }

            ViewBag.client = clientItem;

            return View(authCode);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ConsentPost(string code)
        {
            var authCode = _cache.Get<AuthorizationCode>(code);

            var url = WebUtility.UrlDecode(authCode.redirect_uri);

            _cache.Remove(code);

            return new RedirectResult(url);
        }

        [HttpGet]
        public IActionResult SignIn([FromQuery] OAuthSignInModel value)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var clientItem = _context.Apps.FirstOrDefault(x => x.AppKey.Equals(value.client_id));

                if (clientItem == null)
                {
                    return new NotFoundObjectResult("应用" + value.client_id + "不存在");
                }

                ViewBag.client = clientItem;

                return View(value);
            }

            if (string.IsNullOrWhiteSpace(value.code))
            {
                value.code = MakeAuthCode(value.client_id,
                    value.scope, value.grantType, value.redirect_uri,
                    _tokenProvider.User, BuildRequestUri(value));
            }

            return Redirect("/OAuth/Consent?code=" + value.code);
        }

        [HttpPost]
        public IActionResult SignInPost(OAuthSignInPostModel value)
        {
            TenantUserModel user;
            try
            {
                user = _tenantUser.FindUser(value.userName, value.Pwd);
            }
            catch (ArgumentException ex)
            {
                return new OkObjectResult(ex.Message);
            }

            var ticket = _tenant.CreateTicket(user, value.client_id);

            if (!string.IsNullOrWhiteSpace(value.redirect_uri))
            {
                ticket.Properties.RedirectUri = value.redirect_uri;
            }

            HttpContext.SignInAsync(ticket.Principal, ticket.Properties).Wait();

            var code = MakeAuthCode(value.client_id, value.scope, value.grantType, value.redirect_uri,
                user, BuildRequestUri(value));

            return Redirect("/OAuth/SignIn?Code=" + code);
        }

        private string BuildRequestUri(OAuthSignInModel q)
        {
            return $"/OAuth/SignIn?scheme={q.scheme}&client_id={q.client_id}&redirect_uri={q.redirect_uri}&scope={q.scope}&nonce={q.nonce}&state={q.state}";
        }

        [HttpPost]
        public IActionResult SignOut(string code)
        {
            HttpContext.SignOutAsync().Wait();

            var authCode = _cache.Get<AuthorizationCode>(code);

            _cache.Remove(code);

            return Redirect(authCode.request_uri);
        }

        string MakeAuthCode(string client_id,
            string scope,
            string grantType,
            string redirect_uri,
            TenantUserModel user,
            string request_uri)
        {
            var authCode = new AuthorizationCode()
            {
                tenantId = _tenant.Id,
                tenantName = _tenant.Name,
                client_id = client_id,
                scope = scope,
                response_type = grantType,
                userId = user.ID,
                userName = user.Name,
                userRole = user.Role,
                request_uri = request_uri,
                userAvatar = user.Avatar
            };

            var claims = _tenant.CreateClaims(user, client_id);

            var token = _tokenProvider.CreateToken(_claims =>
            {
                _claims.AddRange(claims);
            });

            authCode.redirect_uri = redirect_uri +
                $"?access_token={token.access_token}&expires_in={token.expires_in}&token_type=${token.token_type}";

            var code = Guid.NewGuid().ToString("n");

            _cache.Set(code, authCode);

            return code;
        }
    }
}
