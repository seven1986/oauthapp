using System;

namespace OAuthApp.ApiModels.AppRankController
{
    public class GetResponse
    {
        /// <summary>
        /// 排行榜ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 总人数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 总分数
        /// </summary>
        public long TotalScore { get; set; }

        /// <summary>
        /// 最高得分
        /// </summary>
        public long TopScore { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
