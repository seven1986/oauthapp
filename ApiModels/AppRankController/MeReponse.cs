using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.ApiModels.AppRankController
{
    public class MeReponse
    {
        /// <summary>
        /// OpenID
        /// </summary>
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
        /// 用户自定义数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 用户本局得分
        /// </summary>
        public long Score { get; set; }

        /// <summary>
        /// 用户最高分
        /// </summary>
        public long MaximumScore { get; set; }

        /// <summary>
        /// 用户最佳排名
        /// </summary>
        public long BetterRankIndex { get; set; }

        /// <summary>
        /// 用户当前游戏排名
        /// </summary>
        public int RankIndex { get; set; }

        /// <summary>
        /// 排行榜总数
        /// </summary>
        public long RankTotalCount { get; set; }

        /// <summary>
        /// 用户游戏得分超越玩家百分比
        /// </summary>
        public decimal BeyoundPercent { get; set; }
    }
}
