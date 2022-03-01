using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Tenant;
using Swashbuckle.AspNetCore.Annotations;
using System;
using Microsoft.AspNetCore.Http;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("租户属性")]
    public class TenantPropertiesController : BaseController
    {
        private readonly TenantDbContext _context;
        private readonly TenantContext _tenant;

        public TenantPropertiesController(TenantDbContext context,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "TenantProperties")]
        [EncryptResultFilter]
        public IActionResult List()
        {
            var result = _context.TenantProperties
                .Where(x => x.TenantID == _tenant.Id)
                .OrderByDescending(x => x.ID)
                .ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "TenantProperty")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.TenantProperties
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "TenantPropertyPut")]
        public IActionResult Put(TenantProperty tenantProperty)
        {
            _context.Entry(tenantProperty).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "TenantPropertyPost")]
        public IActionResult Post(TenantProperty tenantProperty)
        {
            _context.TenantProperties.Add(tenantProperty);

            _context.SaveChanges();

            return OK(new { id = tenantProperty.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "TenantPropertyDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.TenantProperties
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            _context.TenantProperties.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
