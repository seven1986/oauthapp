using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.MicroService.Host.Controllers
{
    public class ToolController : BasicController
    {
        public ToolController(
           ILogger<ConsentController> _logger,
           IStringLocalizer<ToolController> _l,
           TenantService _tenantService,
           TenantDbContext _tenantDb)
        {
            tenantService = _tenantService;
            tenantDb = _tenantDb;
            logger = _logger;
            l = _l;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
