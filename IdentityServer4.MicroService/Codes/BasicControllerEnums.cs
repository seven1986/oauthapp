using System.ComponentModel;

namespace IdentityServer4.MicroService.Codes
{
    /// <summary>
    /// 公用接口业务代码常量表
    /// </summary>
    public enum BasicControllerEnums
    {
        /// <summary>
        /// 正确
        /// </summary>
        [Description("ok")]
        Status200OK = 200,

        /// <summary>
        /// 请求实体错误
        /// </summary>
        [Description("请求实体错误")]
        UnprocessableEntity = 422,

        /// <summary>
        /// 服务器内部错误
        /// </summary>
        [Description("服务器内部错误")]
        ExpectationFailed = 417,

        [Description("未找到内容")]
        NotFound = 404,
    }
}
