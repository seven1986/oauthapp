namespace OAuthApp.ApiModels.AppRankController
{
    public class PutRequest
    { 
        public string UnionID { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// 用户来源
        /// </summary>
        public string Platform { get; set; }
    }
}
