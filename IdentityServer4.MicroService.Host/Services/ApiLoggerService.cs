using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace IdentityServer4.MicroService.Services
{
    internal class ApiLoggerModel
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Action开始执行的时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Action执行完毕的时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Request的参数
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// Request Headers
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// Request Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Request IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// Action耗时 （EndTime - StartTime）
        /// </summary>
        public double UsedSeconds { get; set; }

        /// <summary>
        /// 调用者ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 调用者Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Action报错信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// token claims
        /// </summary>
        public Dictionary<string,string> claims { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    internal class ApiLoggerOptions : Attribute
    {
        public bool EnableHeaders { get; set; }

        public bool EnableClientIp { get; set; }

        public bool EnableParams { get; set; }

        public ApiLoggerOptions() : this(true, true, true)
        {

        }

        public ApiLoggerOptions(bool EnableHeaders, bool EnableClientIp, bool EnableParams)
        {
            this.EnableHeaders = EnableHeaders;
            this.EnableClientIp = EnableClientIp;
            this.EnableParams = EnableParams;
        }
    }

    internal class ApiLoggerService : ActionFilterAttribute
    {
        private readonly string Key = MicroserviceConfig.MicroServiceName + ":ApiLogger";

        private readonly ILogger logger;

        public ApiLoggerService(ILogger<ApiLoggerService> _logger)
        {
            logger = _logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var Descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (Descriptor == null)
            {
                base.OnActionExecuting(context);

                return;
            }

            var log = new ApiLoggerModel()
            {
                StartTime = DateTime.Now,
                Action = Descriptor.ActionName,
                Controller = Descriptor.ControllerName,
                Method = Descriptor.MethodInfo.Name.ToLower(),
                Params = string.Empty,
            };

            var options = Descriptor.MethodInfo.GetCustomAttribute<ApiLoggerOptions>(true);

            if (options == null || options.EnableHeaders)
            {
                log.Headers = JsonConvert.SerializeObject(context.HttpContext.Request.Headers);
            }

            if (options == null || options.EnableClientIp)
            {
                log.ClientIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            if (options == null || options.EnableParams)
            {
                #region Request Url Params 
                if (context.ActionArguments != null &&
                    context.ActionArguments.Keys.Count > 0)
                {
                    log.Params = JsonConvert.SerializeObject(context.ActionArguments);
                }
                #endregion

                #region Request Body Params 
                if (context.HttpContext.Request.HasFormContentType)
                {
                    log.Params += "&RequestForm=" + JsonConvert.SerializeObject(context.HttpContext.Request.Form);
                }
                #endregion
            }

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = context.HttpContext.User;

                log.UserName = user.Identity.Name;

                if (user.Claims.Count() > 0)
                {
                    var subClaim = user.Claims.FirstOrDefault(x => x.Type.Equals("sub"));

                    if (subClaim != null && !string.IsNullOrWhiteSpace(subClaim.Value))
                    {
                        log.UserID = long.Parse(subClaim.Value);
                    }

                    log.claims = new Dictionary<string, string>();

                    var claimsGroup = user.Claims.GroupBy(x => x.Type).ToList();

                    foreach (var g in claimsGroup) {

                        var valueCount = g.Count();

                        if (valueCount > 1)
                        {
                            log.claims.Add(g.Key, string.Join(",", g.Select(x => x.Value).ToList()));
                        }
                        else
                        {
                            log.claims.Add(g.Key, g.FirstOrDefault().Value);
                        }
                    }
                }
            }

            context.HttpContext.Items[Key] = log;

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var logObject = context.HttpContext.Items[Key];

            if (logObject == null)
            {
                base.OnActionExecuted(context);

                return;
            }

            var log = logObject as ApiLoggerModel;

            log.EndTime = DateTime.Now;

            log.UsedSeconds = (log.EndTime - log.StartTime).TotalSeconds;

            if (context.Exception != null)
            {
                log.Error = context.Exception.Message + context.Exception.StackTrace;
            }

            logger.LogWarning(JsonConvert.SerializeObject(log));

            base.OnActionExecuted(context);
        }
    }
}
