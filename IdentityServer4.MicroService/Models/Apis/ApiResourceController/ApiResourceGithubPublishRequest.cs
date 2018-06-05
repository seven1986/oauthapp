namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceGithubPublishRequest
    {
        /// <summary>
     /// userAgent
     /// </summary>
        public string userAgent { get; set; } = "Awesome-Game5.0-App";

        /// <summary>
        /// owner
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// repo
        /// </summary>
        public string repo { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// swaggerUrl
        /// </summary>
        public string swaggerUrl { get; set; }

        /// <summary>
        /// 同步标签
        /// </summary>
        public bool syncLabels { get; set; } = true;

        /// <summary>
        /// 同步文档
        /// </summary>
        public bool syncDocs { get; set; } = true;
    }
}
