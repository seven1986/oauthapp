using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using OAuthApp.ApiModels.TenantServersController;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Http;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("服务器")]
    public class TenantServersController : BaseController
    {
        private readonly TenantDbContext _context;
        private readonly TenantContext _tenant;

        public TenantServersController(TenantDbContext context,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
        }

        [HttpGet("Market")]
        [SwaggerOperation(OperationId = "TenantServerMarket")]
        [AllowAnonymous]
        [EncryptResultFilter]
        public IActionResult Market(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                tag = "site,public";
            }

            var result = _context.Query<MarketResponse>(
                @"SELECT * FROM TenantServers WHERE Tag ='" + tag + "' AND (TenantID = 0 OR TenantID = @TenantID) ORDER BY ID DESC", new
                {
                    TenantID = _tenant.Id
                });

            return OK(result);
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "TenantServers")]
        [EncryptResultFilter]
        public IActionResult List()
        {
            var result = _context.TenantServers.Where(x => x.TenantID == _tenant.Id &&
            x.UserID == UserID).ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "TenantServer")]
        [AllowAnonymous]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.TenantServers
                .FirstOrDefault(x => x.ID == id && !x.IsDelete && x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        //[HttpPut("{id}")]
        //[SwaggerOperation(OperationId = "TenantServerPut")]
        //public IActionResult Put(long id, TenantServer tenantServer)
        //{
        //    if (id != tenantServer.ID ||
        //        !_context.TenantServers.Any(x => x.TenantID == _tenant.Id && x.UserID == UserID))
        //    {
        //        return NotFound();
        //    }

        //    tenantServer.TenantID = _tenant.Id;
        //    tenantServer.UserID = UserID;

        //    _context.Entry(tenantServer).State = EntityState.Modified;

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
        //[SwaggerOperation(OperationId = "TenantServerPost")]
        //public IActionResult Post(TenantServer tenantServer)
        //{
        //    tenantServer.TenantID = _tenant.Id;

        //    tenantServer.UserID = UserID;

        //    _context.TenantServers.Add(tenantServer);

        //    _context.SaveChanges();

        //    return OK(new { id = tenantServer.ID });
        //}

        //[HttpDelete("{id}")]
        //[SwaggerOperation(OperationId = "TenantServerDelete")]
        //public IActionResult Delete(long id)
        //{
        //    var result = _context.TenantServers
        //        .FirstOrDefault(x => x.ID == id && x.UserID == UserID && !x.IsDelete);

        //    if (result == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TenantServers.Remove(result);

        //    _context.SaveChanges();

        //    return OK(true);
        //}
    }
}
