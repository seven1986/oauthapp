namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceCreateVersionRequest
    {
        /// <summary>
        /// api 修订版的版本号
        /// </summary>
        public string revisionId { get; set; }

        /// <summary>
        /// 版本号（v2，v3）
        /// </summary>
        public string apiVersionName { get; set; }
    }
}
