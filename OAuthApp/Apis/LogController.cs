//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using OAuthApp.Data;
//using OAuthApp.Enums;
//using OAuthApp.Models.Apis.Common;
//using OAuthApp.Models.Apis.LogController;
//using Serilog.Context;
//using Swashbuckle.AspNetCore.Annotations;
//using System.Linq;
//using System.Threading.Tasks;
//using static OAuthApp.AppConstant;

//namespace OAuthApp.Apis
//{
//    /// <summary>
//    /// 日志
//    /// </summary>
//    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
//    [ApiExplorerSettingsDynamic("Log")]
//    [SwaggerTag("#### 日志")]
//    [Produces("application/json")]
//    [Consumes("application/json")]
//    public class LogController : ApiControllerBase
//    {
//        #region Services
//        readonly ILogger<LogController> log;
//        readonly LogDbContext logDbContext;
//        #endregion

//        #region 构造函数
//        public LogController(
//            ILogger<LogController> _log,
//            LogDbContext _logDbContext)
//        {
//            log = _log;
//            logDbContext = _logDbContext;
//        }
//        #endregion

//        #region 日志 - 列表
//        /// <summary>
//        /// 日志 - 列表
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet]
//        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:log.get")]
//        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:log.get")]
//        [SwaggerOperation(OperationId = "LogGet",
//            Summary = "日志 - 列表",
//            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.log.get | oauthapp.log.get |")]
//        public async Task<PagingResult<LogEvents>> Get([FromQuery] PagingRequest<LogGetRequest> value)
//        {
//            if (!ModelState.IsValid)
//            {
//                return new PagingResult<LogEvents>()
//                {
//                    code = (int)BasicControllerEnums.UnprocessableEntity,
//                    message = ModelErrors()
//                };
//            }

//            var query = logDbContext.logs.AsQueryable();

//            #region filter
//            query = query.Where(x => x.TenantId == TenantId);

//            if (!string.IsNullOrWhiteSpace(value.q.Message))
//            {
//                query = query.Where(x => x.Message.Contains(value.q.Message));
//            }

//            if (!string.IsNullOrWhiteSpace(value.q.MessageTemplate))
//            {
//                query = query.Where(x => x.MessageTemplate.Equals(value.q.MessageTemplate));
//            }

//            if (!string.IsNullOrWhiteSpace(value.q.Level))
//            {
//                query = query.Where(x => x.Level.Equals(value.q.Level));
//            }

//            if (!string.IsNullOrWhiteSpace(value.q.LogEvent))
//            {
//                query = query.Where(x => x.LogEvent.Contains(value.q.LogEvent));
//            }

//            if (value.q.TimeStamp.HasValue)
//            {
//                query = query.Where(x => x.TimeStamp <= value.q.TimeStamp.Value);
//            }
//            #endregion

//            #region total
//            var result = new PagingResult<LogEvents>()
//            {
//                skip = value.skip.Value,
//                data = query.Skip(value.skip.Value).Take(value.take.Value).ToList(),
//                take = value.take.Value,
//                total = await query.CountAsync()
//            };
//            #endregion

//            return result;
//        }
//        #endregion

//        #region 日志 - 添加
//        /// <summary>
//        /// 日志 - 添加
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:log.post")]
//        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:log.post")]
//        [SwaggerOperation(OperationId = "LogPost",
//            Summary = "日志 - 添加",
//            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.log.post | oauthapp.log.post |")]
//        public ApiResult<bool> Post([FromBody]LogPostRequest value)
//        {
//            using (LogContext.PushProperty("UserID", UserId))
//            using (LogContext.PushProperty("TenantID", TenantId))
//            using (LogContext.PushProperty("ClientID", ClientId))
//            {
//                switch (value.level)
//                {
//                    case LogLevel.Trace: log.LogTrace(value.message); break;
//                    case LogLevel.Debug: log.LogDebug(value.message); break;
//                    case LogLevel.Information: log.LogInformation(value.message); break;
//                    case LogLevel.Warning: log.LogWarning(value.message); break;
//                    case LogLevel.Error: log.LogError(value.message); break;
//                    default: break;
//                }
                
//            }
//            return new ApiResult<bool>(true);
//        }
//        #endregion

//        #region 日志 - 删除
//        /// <summary>
//        /// 日志 - 删除
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        [HttpDelete("{id}")]
//        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:log.delete")]
//        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:log.delete")]
//        [SwaggerOperation(OperationId = "LogDelete",
//            Summary = "日志 - 删除",
//            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.log.delete | oauthapp.log.delete |")]
//        public async Task<ApiResult<long>> Delete(int id)
//        {
//            var entity = await logDbContext.logs.FirstOrDefaultAsync(x => x.Id == id);

//            if (entity == null)
//            {
//                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
//            }

//            logDbContext.logs.Remove(entity);

//            await db.SaveChangesAsync();

//            return new ApiResult<long>(id);
//        }
//        #endregion
//    }
//}
