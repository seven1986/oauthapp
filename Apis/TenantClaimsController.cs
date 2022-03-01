using System;
using System.Linq;
using OAuthApp.Filters;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace OAuthApp.Apis
{
    [SwaggerTag("租户声明")]
    [ApiExplorerSettings(IgnoreApi =true)]
    public class TenantClaimsController : BaseController
    {
        private readonly TenantDbContext _context;

        public TenantClaimsController(TenantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "TenantClaims")]
        [EncryptResultFilter]
        public IActionResult List(long tenantID)
        {
            var result = _context.TenantClaims
                .Where(x => x.TenantID == tenantID)
                .OrderByDescending(x => x.ID)
                .ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "TenantClaim")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.TenantClaims
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "TenantClaimPut")]
        public IActionResult Put(TenantClaim tenantClaim)
        {
            _context.Entry(tenantClaim).State = EntityState.Modified;

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

        [HttpPost]
        [SwaggerOperation(OperationId = "TenantClaimPost")]
        public IActionResult Post(TenantClaim tenantClaim)
        {
            _context.TenantClaims.Add(tenantClaim);

            _context.SaveChanges();

            return OK(new { id = tenantClaim.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "TenantClaimDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.TenantClaims
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            _context.TenantClaims.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }

    }
}
