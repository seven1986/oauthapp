using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OAuthApp.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.ApiModels.AppRankController;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用排行榜")]
    public class AppRankController : BaseController
    {
        #region services
        private readonly AppDbContext _context;
        #endregion

        #region construct
        public AppRankController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        string RankKey
        {
            get
            {
                return Request.RouteValues["rankKey"].ToString();
            }
        }

        #region 统计
        const string list_CMD = @"SELECT RankKey as ID ,COUNT(1) AS Count, SUM(SCORE) as TotalScore,MAX(MaximumScore) AS TopScore,MIN(CreateDate) as CreateDate FROM AppRanks WHERE AppID = {0} GROUP BY RankKey";

        /// <summary>
        /// 统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("{appId}/Report")]
        [SwaggerOperation(OperationId = "AppRankReport")]
        public IActionResult Report()
        {
            var sql = string.Format(list_CMD, AppID);

            var result = _context.Query<GetResponse>(sql);

            return OK(result);
        }
        #endregion

        #region 列表
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pltform">用户来源</param>
        /// <param name="take">条数</param>
        /// /// <returns></returns>
        [HttpGet("{appId}/{rankKey}")]
        [SwaggerOperation(OperationId = "AppRankList")]
        public IActionResult List([FromQuery] string pltform, [FromQuery] int take = 30)
        {
            var q = _context.AppRanks.AsQueryable();

            q = q.Where(x => x.AppID == AppID && x.RankKey == RankKey);

            if (!string.IsNullOrWhiteSpace(pltform))
            {
                q = q.Where(x => x.Platform == pltform);
            }

            var result = q.OrderByDescending(x => x.MaximumScore)
                .Take(take).ToList();

            return OK(result);
        }
        #endregion

        #region 提交或更新分数
        /// <summary>
        /// 提交或更新分数
        /// </summary>
        /// <param name="value">用户数据</param>
        /// <returns></returns>
        [HttpPut("{appId}/{rankKey}")]
        [SwaggerOperation(OperationId = "AppRankPut")]
        public IActionResult Put([Required][FromBody]PutRequest value)
        {
            var entity = _context.AppRanks.FirstOrDefault(x =>
                    x.AppID == AppID &&
                    x.RankKey == RankKey &&
                    x.Platform == value.Platform &&
                    x.UnionID == value.UnionID);

            if (entity != null)
            {
                if (!string.IsNullOrWhiteSpace(value.Avatar))
                {
                    entity.Avatar = value.Avatar;
                }

                if (!string.IsNullOrWhiteSpace(value.NickName))
                {
                    entity.NickName = value.NickName;
                }

                if (!string.IsNullOrWhiteSpace(value.Data))
                {
                    entity.Data = value.Data;
                }

                if (entity.MaximumScore < value.Score)
                {
                    entity.MaximumScore = value.Score;
                }

                entity.Score = value.Score;

                entity.LastUpdate = DateTime.UtcNow;
            }

            else
            {
                entity = new AppRank()
                {
                    Avatar = value.Avatar,
                    AppID = AppID,
                    Data = value.Data,
                    UserID = UserID,
                    NickName = value.NickName,
                    UnionID = value.UnionID,
                    Platform = value.Platform,
                    RankKey = RankKey,
                    Score = value.Score,
                    MaximumScore = value.Score
                };

                _context.AppRanks.Add(entity);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return OK(true);
        }
        #endregion

        #region 我的排名
        const string MyRank_CMD_1 = "SELECT * FROM " +
            "(SELECT ROW_NUMBER() OVER(ORDER BY Score DESC) AS RankIndex," +
                "Avatar,UnionID,NickName,Score,Data,[Platform],MaximumScore " +
                "FROM AppRanks WHERE AppID = @AppID AND RankKey=@RankKey) A" +
            "  WHERE A.[Platform]=@Platform AND A.UnionID=@UnionID";

        const string MyRank_CMD_2 = "SELECT A.RankIndex FROM " +
           " (SELECT ROW_NUMBER() OVER(ORDER BY MaximumScore DESC) AS RankIndex, " +
           " UnionID,[Platform] " +
           " FROM AppRanks WHERE AppID = @AppID AND RankKey=@RankKey) A " +
           " WHERE A.[Platform]=@Platform AND A.UnionID=@UnionID LIMIT 1";

        const string MyRank_CMD_3 = "SELECT COUNT(1) FROM AppRanks WHERE AppID = @AppID AND RankKey=@RankKey";

        /// <summary>
        /// 我的排名
        /// </summary>
        /// <param name="platform">用户来源</param>
        /// <param name="unionID">UnionID</param>
        /// /// <returns></returns>
        [HttpGet("{appId}/{rankKey}/Me")]
        [SwaggerOperation(OperationId = "AppRankMe")]
        public IActionResult Me([Required][FromQuery] string platform,
            [Required][FromQuery] string unionID)
        {
            var result = _context.QueryFirstOrDefault<MeReponse>(MyRank_CMD_1,
                new
                {
                    AppID,
                    RankKey,
                    Platform = platform,
                    UnionID = unionID
                });

            if(result==null)
            {
                return Error("数据不存在");
            }

            var _BetterRankIndex = _context.QueryFirstOrDefault<string>(MyRank_CMD_2,
                new
                {
                    AppID,
                    RankKey,
                    Platform = platform,
                    UnionID = unionID
                });

            if (!string.IsNullOrWhiteSpace(_BetterRankIndex))
            {
                result.BetterRankIndex = long.Parse(_BetterRankIndex);
            }

            var _RankTotalCount = _context.QueryFirstOrDefault<string>(MyRank_CMD_3,
                new
                {
                    AppID,
                    RankKey
                });

            if (!string.IsNullOrWhiteSpace(_RankTotalCount))
            {
                result.RankTotalCount = long.Parse(_RankTotalCount);
            }

            if (result.Score == 0)
            {
                result.BeyoundPercent = 0;
            }

            else if (result.RankIndex == 1)
            {
                result.BeyoundPercent = 100;
            }

            else
            {
                result.BeyoundPercent = (decimal)Math.Round(100.00 - result.RankIndex / (double)result.RankTotalCount * 100.00, 2);
            }

            return OK(result);
        }
        #endregion
    }
}
