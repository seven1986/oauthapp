using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.ApiResourceModels
{
    /// <summary>
    /// query model
    /// </summary>
    public class ApiResourceQuery
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 微服务发布实体
    /// </summary>
    public class ApiResourcePublishModel
    {
        /// <summary>
        /// id
        /// </summary>
        [Required(ErrorMessage = "请填写ID")]
        public int id { get; set; }

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
        [Required(ErrorMessage = "请填写ProductID")]
        public string productId { get; set; }

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

    /// <summary>
    /// 渠道包的实体
    /// </summary>
    public class GenerateClientModel
    {
        /// <summary>
        /// id
        /// </summary>
        [Required(ErrorMessage = "请填写ID")]
        public int id { get; set; }

        public Dictionary<string, string> languages { get; set; }
    }
}
