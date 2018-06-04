using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Models.Shared;

namespace IdentityServer4.MicroService.Host.Controllers
{
    [SecurityHeaders]
    public class BasicController : Controller
    {
        protected virtual TenantDbContext tenantDb { get; set; }
        protected virtual TenantService tenantService { get; set; }

        // public used for View Pages
        //
        //TenantPublicModel _pubTenant;
        //public TenantPublicModel pubTenant
        //{
        //    get
        //    {
        //        if (_pubTenant != null)
        //        {
        //            var currentTenant = HttpContext.Items[TenantConstant.CacheKey];

        //            if (currentTenant != null)
        //            {
        //                var tenantString = currentTenant.ToString();
        //                _pubTenant = JsonConvert.DeserializeObject<TenantPublicModel>(tenantString);
        //            }
        //            if (_pubTenant == null)
        //            {
        //                var DefaultTenant = tenantDb.Tenants.Include(x => x.Claims).FirstOrDefault();

        //                _pubTenant = new TenantPublicModel()
        //                {
        //                    id = DefaultTenant.Id,
        //                    name = DefaultTenant.Name,
        //                    claims = DefaultTenant.Claims.ToDictionary(k => k.ClaimType, v => v.ClaimValue)
        //                };
        //            }
        //        }

        //        return _pubTenant;
        //    }
        //}

        // private use
        private TenantPrivateModel _pvtTenant;
        public TenantPrivateModel pvtTenant
        {
            get
            {
                if (_pvtTenant == null)
                {
                    var tenantCache = tenantService.GetTenant(tenantDb, HttpContext.Request.Host.Value);

                    _pvtTenant = JsonConvert.DeserializeObject<TenantPrivateModel>(tenantCache.Item2);
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
