namespace IdentityServer4.MicroService.Models.Apis.CodeGenController
{
  public class CodeGenCommonOptionsModel
    {
        /// <summary>
        /// 网关swagger地址
        /// </summary>
        public string gatewaySwaggerUrl { get; set; }

        /// <summary>
        /// 直连swagger地址
        /// </summary>
        public string apiResourceSwaggerUrl { get; set; }
    }
}
