using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourcePublishRequest
    {
        /// <summary>
        /// API名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// API功能简介
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// api的id
        /// </summary>
        [Required(ErrorMessage = "请填写ID")]
        public string apiId { get; set; }

        /// <summary>
        /// 二级路径
        /// </summary>
        [RegularExpression("[a-zA-Z0-9]{1,32}", ErrorMessage = "路径格式错误")]
        public string suffix { get; set; }

        /// <summary>
        /// swagger文档Uri
        /// </summary>
        [Url(ErrorMessage = "文档路径格式错误")]
        public string swaggerUrl { get; set; }

        /// <summary>
        /// 服务所属产品组
        /// </summary>
        public string[] productIds { get; set; }

        /// <summary>
        /// 使用的Oauth服务ID，从网关创建得来，可空
        /// </summary>
        public string authorizationServerId { get; set; }

        /// <summary>
        /// 调用Api时需要授权的的scope，可空
        /// </summary>
        public string scope { get; set; }

        /// <summary>
        /// 使用的OpenId服务ID，从网关创建得来，可空
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 服务的策略配置
        /// https://docs.microsoft.com/zh-cn/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-api-entity#SetAPIPolicy
        /// </summary>
        public string policy { get; set; }
    }
}
