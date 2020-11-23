namespace OAuthApp.Models.Apis.ClientController
{
    public class ClientIssueTokenRequest
    {
        /// <summary>
        /// 有效时间，单位时秒，默认微3600
        /// </summary>
        public int lifetime { get; set; }
    }
}
