using System;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.MicroService.Controllers
{
    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> l;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(
            IStringLocalizer<HomeController> localizer,
            SignInManager<AppUser> signInManager
            )
        {
            l = localizer;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var cul = System.Globalization.CultureInfo.CurrentCulture.Name;

            var ll = l["无效的登录尝试"];

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult apimdelegation(string operation,string returnUrl,string salt,string sig,string userId,string productId,string subscriptionId)
        {
            #region SignOut
            if (operation.Equals("SignOut"))
            {
                _signInManager.SignOutAsync().Wait();

                HttpContext.SignOutAsync().Wait();

                return Redirect("https://portal.ixingban.com");
            }
            #endregion

            #region SignIn
            string key = "829PBWllXiTWGCExixRQKsSaTaimeY0ebpOl1r0Odqb6nquN7BR8phbC47nng8go2Ugr1i9Peh5/Am2V4TnctA==";

            string signature;

            using (var encoder = new HMACSHA512(Convert.FromBase64String(key)))
            {
                switch (operation)
                {
                    case "SignIn":
                        signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + returnUrl)));
                        break;

                    case "ChangePassword": 
                    case "ChangeProfile":
                        signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + userId)));
                        break;

                    case "Subscribe":
                        signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + productId + "\n" + userId)));
                        break;
                    case "Unsubscribe":
                        signature = Convert.ToBase64String(encoder.ComputeHash(Encoding.UTF8.GetBytes(salt + "\n" + subscriptionId)));
                        break;

                    //case "SignOut":
                    //    break;
                    //case "CloseAccount":
                    //    break;
                    //case "Renew":
                    //    break;

                    default: signature = "";break;
                }
            }

            if (!signature.Equals(sig))
            {
                return Redirect("/home/error");
            }

            if (operation.Equals("SignIn"))
            {
                returnUrl = HttpUtility.UrlEncode("https://portal.ixingban.com") + returnUrl;

                return Redirect("/Account/Login?returnurl=" + returnUrl);
            } 
            #endregion

            return Redirect("/home/error");
        }
    }
}
