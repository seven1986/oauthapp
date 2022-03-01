using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Tenant;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using OAuthApp.Data;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("租户")]
    public class TenantsController : BaseController
    {
        private readonly TenantDbContext _context;
        private readonly AppDbContext _appContext;
        private readonly ApiDbContext _apiContext;
        
        public TenantsController(TenantDbContext context, 
            AppDbContext appContext,
            ApiDbContext apiContext)
        {
            _context = context;
            _appContext = appContext;
            _apiContext = apiContext;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "Tenants")]
        [EncryptResultFilter]
        public IActionResult List()
        {
            var result = _context.Tenants
               .Where(x => !x.IsDelete && x.OwnerId == UserID && x.OwnerHost == Request.Host.Host)
               .OrderByDescending(x => x.ID).ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "Tenant")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var tenant = _context.Tenants
                .Where(x => x.ID == id && x.OwnerId == UserID)
                .FirstOrDefault();

            if (tenant == null)
            {
                return NotFound();
            }

            var path = AppConst.TenantDBPath + "\\" + tenant.OwnerHost + ".db";

            try
            {
                using (var dbFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    tenant.DatabaseSize = dbFile.Length;
                    tenant.LastUpdate = DateTime.Now;
                    _context.SaveChanges();
                }
            }
            catch { }

            var appSize = _context.TenantOrders
                .Where(x => x.TenantID == id && x.ChannelCode == ChannelCodes.App).Sum(x => x.Amount);

            var appBlobSize = _context.TenantOrders
              .Where(x => x.TenantID == id && x.ChannelCode == ChannelCodes.AppBlob).Sum(x => x.Amount);

            var appVersionSize = _context.TenantOrders
                .Where(x => x.TenantID == id && x.ChannelCode == ChannelCodes.AppVersion).Sum(x => x.Amount);

            return OK(new
            {
                database = new { size = tenant.DatabaseSize },
                appServer = new { size = appSize },
                blobServer = new { size = appBlobSize },
                releaseServer = new { size = appVersionSize }
            });
        }

        [HttpGet("CheckVersion")]
        [SwaggerOperation(OperationId = "TenantCheckVersion")]
        [EncryptResultFilter]
        public IActionResult CheckVersion(string channelCode,bool checkOnly = true)
        {
            if (channelCode.Equals(ChannelCodes.App))
            {
                if (!checkOnly)
                {
                    var appNewMigrations = _appContext.Database.GetPendingMigrations().ToList();

                    if (appNewMigrations.Count > 0)
                    {
                        try
                        {
                            _appContext.Database.Migrate();
                        }
                        catch(Exception ex)
                        {
                            return Error(ex.Message);
                        }
                    }
                }

                return OK(new
                {
                    versions = _appContext.Database.GetAppliedMigrations(),
                    newVersions = _appContext.Database.GetPendingMigrations()
                });
            }

            if (channelCode.Equals(ChannelCodes.Api))
            {
                if (!checkOnly)
                {
                    var apiNewMigrations = _apiContext.Database.GetPendingMigrations().ToList();

                    if (apiNewMigrations.Count > 0)
                    {
                        try
                        {
                            _apiContext.Database.Migrate();
                        }
                        catch (Exception ex)
                        {
                            return Error(ex.Message);
                        }
                    }
                }

                return OK(new
                {
                    versions = _apiContext.Database.GetAppliedMigrations(),
                    newVersions = _apiContext.Database.GetPendingMigrations()
                });
            }

            if (channelCode.Equals(ChannelCodes.Tenant))
            {
                if (!checkOnly)
                {
                    var tenantNewMigrations = _context.Database.GetPendingMigrations().ToList();

                    if (tenantNewMigrations.Count > 0)
                    {
                        try
                        {
                            _context.Database.Migrate();
                        }
                        catch (Exception ex)
                        {
                            return Error(ex.Message);
                        }
                    }
                }

                return OK(new
                {
                    versions = _context.Database.GetAppliedMigrations(),
                    newVersions = _context.Database.GetPendingMigrations()
                });
            }

            return OK(true);
        }

        //[HttpPut("{id}")]
        //[SwaggerOperation(OperationId = "TenantPut")]
        //public IActionResult Put(Tenant.Tenant tenant)
        //{
        //   _context.Entry(tenant).State = EntityState.Modified;

        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        return Error(ex.Message);
        //    }

        //    return OK(true);
        //}

        //[HttpPost]
        //[SwaggerOperation(OperationId = "TenantPost")]
        //public IActionResult Post(Tenant.Tenant tenant)
        //{
        //    tenant.OwnerId = UserID;

        //    _context.Tenants.Add(tenant);

        //    _context.SaveChanges();

        //    _dbCreator.EnsureCreated();

        //    return OK(new { id = tenant.ID });
        //}

        //[HttpDelete("{id}")]
        //[SwaggerOperation(OperationId = "TenantDelete")]
        //public IActionResult Delete(long id)
        //{
        //    var result = _context.Tenants
        //        .FirstOrDefault(x => x.ID == id && x.OwnerId == UserID);

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Tenants.Remove(result);

        //    _context.SaveChanges();

        //    return OK(true);
        //}
    }
}
