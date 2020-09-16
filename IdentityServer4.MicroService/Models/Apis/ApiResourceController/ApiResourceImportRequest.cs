using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourceImportRequest
    {
        /// <summary>
        /// API名称
        /// </summary>
        [Required]
        public string MicroServiceName { get; set; }

        /// <summary>
        /// API显示名称
        /// </summary>
        public string MicroServiceDisplayName { get; set; }

        /// <summary>
        /// API简介
        /// </summary>
        public string MicroServiceDescription { get; set; }

        /// <summary>
        /// API策略集
        /// </summary>
        public List<PolicyConfig> MicroServicePolicies { get; set; }

        /// <summary>
        /// 要关联RedirectUrls的ClientID
        /// 默认位swagger
        /// </summary>
        public List<string> MicroServiceClientIDs { get; set; } = new List<string>();

        /// <summary>
        /// 合法回调地址集合
        /// </summary>
        public List<string> MicroServiceRedirectUrls { get; set; } = new List<string>();
    }

    public class PolicyConfig
    {
        public string ControllerName { get; set; }

        public List<string> Scopes { get; set; } = new List<string>();

        public List<string> Permissions { get; set; } = new List<string>();
    }
}
