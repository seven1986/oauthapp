using OAuthApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace OAuthApp.Tenant
{
    public class TenantDbCreator
    {
        public string _DBConnection;

        public string DBConnection 
        { 
            get 
            {
                return "Data Source=" + _DBConnection;
            }
        }

        public long DBSize 
        { 
            get
            { 
                if(File.Exists(_DBConnection))
                {
                    return new FileInfo(_DBConnection).Length;
                }

                return 0;
            } 
        }

        readonly IServiceScopeFactory _servFac;
        readonly ILogger<TenantDbCreator> _log;
        public TenantDbCreator(
            IHttpContextAccessor accessor,
            IServiceScopeFactory servFac,
            ILogger<TenantDbCreator> log)
        {
            _log = log;

            _servFac = servFac;

            if (accessor.HttpContext != null)
            {
                _DBConnection = AppConst.TenantDBPath + "\\" + accessor.HttpContext.Request.Host.Host + ".db";
            }

            _log.LogWarning("【TenantDbCreator】:" + DBConnection);
        }

        public void EnsureCreated()
        {
            using var serviceScope = _servFac.CreateScope();

            var apiDbcontext = serviceScope.ServiceProvider.GetRequiredService<ApiDbContext>();

            //var apiDbIsCtreated = apiDbcontext.Database.EnsureCreated();

            apiDbcontext.Database.Migrate();

            // _log.LogWarning("【TenantDbCreator】:" + DBConnection + ",【apiDbcontext】：" + apiDbIsCtreated);

            var appDbcontext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

            appDbcontext.Database.Migrate();

            InSertSeedData(appDbcontext);
        }

        private static void InSertSeedData(AppDbContext appDb)
        {
            if (!appDb.Apps.Any(x => x.ID > 0))
            {
                appDb.Apps.Add(new App()
                {
                    Name = "client",
                    Logo = "./assets/app_default.png",
                    ProjectID = 0,
                    UserID = 0,
                    AppKey = DataSeed.AppKey
                });

                appDb.SaveChanges();
            }
        }
    }
}
