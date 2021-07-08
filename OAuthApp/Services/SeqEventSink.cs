using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OAuthApp.Tenant;
using Serilog;
using Serilog.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthApp.Services
{
    public class AuthingEventSink : IEventSink
    {
        private readonly ILogger<AuthingEventSink> _log;

        public long ClientId { get; set; }
        public long UserId { get; set; }
        public long TenantId { get; set; }

        public AuthingEventSink(ILogger<AuthingEventSink> log,
            HttpContextAccessor accessor
            )
        {
            _log = log;
            var claims = ((ClaimsIdentity)accessor.HttpContext.User.Identity).Claims.ToList();
            ClientId = long.Parse(claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.ClientId)).Value);
            UserId = long.Parse(claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Subject)).Value);
            TenantId = long.Parse(claims.FirstOrDefault(x => x.Type.Equals(TenantConstant.TokenKey)).Value);
        }

        public Task PersistAsync(Event evt)
        {
            using (LogContext.PushProperty("ClientId", ClientId))
            using (LogContext.PushProperty("UserId", UserId))
            using (LogContext.PushProperty("TenantId", TenantId))
            {
                if (evt.EventType == EventTypes.Success ||
                evt.EventType == EventTypes.Information)
                {
                    _log.LogInformation("{Name} ({Id}), Details: {@details}",
                        evt.Name,
                        evt.Id,
                        evt);
                }
                else
                {
                    _log.LogError("{Name} ({Id}), Details: {@details}",
                        evt.Name,
                        evt.Id,
                        evt);
                }

                return Task.CompletedTask;
            }
        }
    }

}
