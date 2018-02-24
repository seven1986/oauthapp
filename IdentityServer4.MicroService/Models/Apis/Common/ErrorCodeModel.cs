namespace IdentityServer4.MicroService.Models.Apis.Common
{
    public class ErrorCodeModel
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public long Code { get; set; }

        /// <summary>
        /// 错误名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string Description { get; set; }
    }
}
