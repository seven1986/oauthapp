using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Tenant;
using Swashbuckle.AspNetCore.Annotations;

namespace OAuthApp.Apis
{
    [SwaggerTag("租户站点")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TenantHostsController : BaseController
    {
        private readonly TenantDbContext _context;

        public TenantHostsController(TenantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "TenantHosts")]
        public IActionResult List(long tenantID)
        {
            var result = _context.TenantHosts
                .Where(x => x.TenantID == tenantID)
                .OrderByDescending(x => x.ID)
                .ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "TenantHost")]
        public IActionResult Get(long id)
        {
            var result = _context.TenantHosts
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "TenantHostPut")]
        public IActionResult Put(TenantHost tenantHost)
        {
            _context.Entry(tenantHost).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "TenantHostPost")]
        public IActionResult Post(TenantHost tenantHost)
        {
            _context.TenantHosts.Add(tenantHost);

            _context.SaveChanges();

            return OK(new { id = tenantHost.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "TenantHostDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.TenantHosts
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            _context.TenantHosts.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
