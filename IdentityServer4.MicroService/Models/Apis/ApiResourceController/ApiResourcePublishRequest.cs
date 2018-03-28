using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourcePublishRequest
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

        /// <summary>
        /// 创建为修订版本
        /// </summary>
        //public PublishMode publishMode { get; set; } = PublishMode.Release;

        /// <summary>
        /// 发布修订版时，必填
        /// </summary>
        //public string releaseNote { get; set; }
    }

    //public enum PublishMode
    //{
    //    /// <summary>
    //    /// 首次发布或覆盖当前版本的发布
    //    /// </summary>
    //    [EnumMember(Value = "Release")]
    //    Release = 0,

    //    /// <summary>
    //    /// 发布修订版
    //    /// </summary>
    //    [EnumMember(Value = "ReVersion")]
    //    ReVersion = 1,

    //    /// <summary>
    //    /// 发布新版本
    //    /// </summary>
    //    [EnumMember(Value = "NewVersion")]
    //    NewVersion = 2,
    //}
}
