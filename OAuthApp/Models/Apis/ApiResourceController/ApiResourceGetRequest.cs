namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourceGetRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否展开所有Scope（默认为false）
        /// </summary>
        public bool expandScopes { get; set; } = false;

        /// <summary>
        /// 是否展开所有Claims（默认为false）
        /// </summary>
        public bool expandClaims { get; set; } = false;
    }
}
