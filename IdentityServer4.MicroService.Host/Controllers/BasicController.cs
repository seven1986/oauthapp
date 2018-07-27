using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Models.Shared;
using Microsoft.Extensions.Localization;
using IdentityServer4.MicroService.Host.Attributes;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.MicroService.Host.Controllers
{
    [SecurityHeaders]
    public class BasicController : Controller
    {
        protected virtual TenantDbContext tenantDb { get; set; }
        protected virtual TenantService tenantService { get; set; }
        protected virtual IStringLocalizer l { get; set; }
        protected virtual ILogger logger { get; set; }

        private TenantPrivateModel _pvtTenant;
        public TenantPrivateModel pvtTenant
        {
            get
            {
                if (_pvtTenant == null)
                {
                    var tenantCache = tenantService.GetTenant(tenantDb, HttpContext.Request.Host.Value);

                    _pvtTenant = tenantCache.Item2;
                }

                return _pvtTenant;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ViewBag.pvtTenant = pvtTenant;

            base.OnActionExecuted(context);
        }
    }
}
