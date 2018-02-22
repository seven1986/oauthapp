using IdentityServer4.MicroService.Models.AppTenantModels;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

namespace IdentityServer4.MicroService.Controllers
{
    public class BasicController:Controller
    {
        protected virtual TenantDbContext tenantDb { get; set; }
        protected virtual TenantService tenantService { get; set; }

        // public used for View Pages
        //
        //AppTenantPublicModel _pubTenant;
        //public AppTenantPublicModel pubTenant
        //{
        //    get
        //    {
        //        if (_pubTenant != null)
        //        {
        //            var currentTenant = HttpContext.Items[TenantConstant.CacheKey];

        //            if (currentTenant != null)
        //            {
        //                var tenantString = currentTenant.ToString();
        //                _pubTenant = JsonConvert.DeserializeObject<AppTenantPublicModel>(tenantString);
        //            }
        //            if (_pubTenant == null)
        //            {
        //                var DefaultTenant = tenantDb.Tenants.Include(x => x.Claims).FirstOrDefault();

        //                _pubTenant = new AppTenantPublicModel()
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
        private AppTenantPrivateModel _pvtTenant;
        public AppTenantPrivateModel pvtTenant
        {
            get
            {
                if (_pvtTenant == null)
                {
                    var tenantCache = tenantService.GetTenant(tenantDb, HttpContext.Request.Host.Value);

                    _pvtTenant = JsonConvert.DeserializeObject<AppTenantPrivateModel>(tenantCache.Item2);
                }

                return _pvtTenant;
            }
        }
    }
}
