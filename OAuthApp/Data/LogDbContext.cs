using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Tenant;
using Serilog.Core;
using Serilog.Events;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;

namespace OAuthApp.Data
{
   public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
         : base(options)
        {
        }

        public DbSet<LogEvents> logs { get; set; }
    }

    [Table("LogEvents")]
    public class LogEvents
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string LogEvent { get; set; }

        public long TenantId { get; set; } = 1;

        public long? UserId { get; set; }

        public long? ClientId { get; set; }
    }

    //public class OAuthAppEnricher : ILogEventEnricher
    //{
    //    readonly HttpContextAccessor _contextAccessor;

    //    public OAuthAppEnricher() { }

    //    public OAuthAppEnricher(HttpContextAccessor contextAccessor)
    //    {
    //        _contextAccessor = contextAccessor;
    //    }

    //    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    //    {
    //        var claims = ((ClaimsIdentity)_contextAccessor.HttpContext.User.Identity).Claims;
    //        var UserID = claims.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier);
    //        var ClientId = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.ClientId);
    //        var TenantID = claims.FirstOrDefault(x => x.Type == TenantConstant.TokenKey);

    //        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
    //                "UserID", UserID.Value));
    //        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
    //                "ClientId", ClientId.Value));
    //        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
    //                "TenantID", TenantID.Value));
    //    }
    //}
}
