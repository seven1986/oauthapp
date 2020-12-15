using OAuthApp.Models.Apis.AuthingController;
using OAuthApp.Models.Shared;
using System.Collections.Generic;

namespace OAuthApp.Models.Apis.ConsentController
{
   public class AuthingPreConsentReponse
    {
        /// <summary>
        /// 租户
        /// </summary>
        public TenantPublicModel Tenant { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 应用网址
        /// </summary>
        public string ClientUrl { get; set; }

        /// <summary>
        /// 应用Logo
        /// </summary>
        public string ClientLogoUrl { get; set; }

        /// <summary>
        /// 应用简介
        /// </summary>
        public string ClientDescription { get; set; }

        /// <summary>
        /// 标识权限
        /// </summary>
        public List<AuthingScopeItem> IdentityScopes { get; set; } = new List<AuthingScopeItem>();

        /// <summary>
        /// 服务权限
        /// </summary>
        public List<AuthingScopeItem> ApiScopes { get; set; } = new List<AuthingScopeItem>();
    }
}
