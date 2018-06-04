using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Microsoft.AspNetCore.Authentication.Weixin
{
    /// <summary>
    /// Defines a set of options used by <see cref="WeixinHandler"/>.
    /// </summary>
    public class WeixinOptions : OAuthOptions
    {
        public WeixinOptions()
        {
            ClaimsIssuer = WeixinDefaults.Issuer;
            CallbackPath = new PathString(WeixinDefaults.CallbackPath);

            AuthorizationEndpoint = WeixinDefaults.AuthorizationEndpoint;
            TokenEndpoint = WeixinDefaults.TokenEndpoint;
            UserInformationEndpoint = WeixinDefaults.UserInformationEndpoint;

            Scope.Add("snsapi_login");
            Scope.Add("snsapi_userinfo");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "unionid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey(ClaimTypes.Gender, "sex");
            ClaimActions.MapJsonKey(ClaimTypes.Country, "country");
            ClaimActions.MapJsonKey("urn:weixin:openid", "openid");
            ClaimActions.MapJsonKey("urn:weixin:province", "province");
            ClaimActions.MapJsonKey("urn:weixin:city", "city");
            ClaimActions.MapJsonKey("urn:weixin:headimgurl", "headimgurl");

            // issue for InvalidCastException: Cannot cast Newtonsoft.Json.Linq.JArray to Newtonsoft.Json.Linq.JToken.
            // ClaimActions.MapJsonKey("urn:weixin:privilege", "privilege");
        }
    }
}
