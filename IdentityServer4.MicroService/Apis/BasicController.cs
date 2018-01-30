using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.AppTenantModels;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    [ServiceFilter(typeof(ApiTracker.ApiTracker), IsReusable = true)]
    [Authorize(AuthenticationSchemes = AppAuthenScheme)]
    public class BasicController : ControllerBase
    {
        public virtual IStringLocalizer l { get; set; }
        public virtual RedisService redis { get; set; }

        protected readonly Random random = new Random(DateTime.UtcNow.AddHours(8).Second);

        protected long UserId
        {
            get
            {
                return long.Parse(User.Claims.FirstOrDefault(x => x.Type.Equals("sub")).Value);
            }
        }

        protected string ModelErrors()
        {
            var errObject = new JObject();

            foreach (var errKey in ModelState.Keys)
            {
                var errValues = ModelState[errKey];

                var errMessages = errValues.Errors.Select(x => !string.IsNullOrWhiteSpace(x.ErrorMessage) ? l[x.ErrorMessage] : x.Exception.Message).ToList();

                if (errMessages.Count > 0)
                {
                    errObject.Add(errKey, JToken.FromObject(errMessages));
                }
            }

            return JsonConvert.SerializeObject(errObject);
        }

        /// <summary>
        /// 租户信息 from client access token
        /// </summary>
        protected long TenantId
        {
            get
            {
                var tenant = User.Claims.
                    Where(x => x.Type.Contains(TenantConstant.TokenKey)).FirstOrDefault();

                if (tenant != null)
                {
                    var _tenantId = JObject.Parse(tenant.Value)["id"].ToString();

                    return long.Parse(_tenantId);
                }

                return 1L;
            }
        }

        public virtual TenantService tenantService { get; set; }
        public virtual TenantDbContext tenantDb { get; set; }

        private AppTenantPrivateModel _tenant;
        public AppTenantPrivateModel Tenant
        {
            get
            {
                if (_tenant == null)
                {
                    var tenantCache = tenantService.GetTenant(tenantDb, HttpContext.Request.Host.Value);

                    _tenant = JsonConvert.DeserializeObject<AppTenantPrivateModel>(tenantCache.Item2);
                }

                return _tenant;
            }
        }

        private AzureApiManagementServices _azureApim;
        public AzureApiManagementServices AzureApim
        {
            get
            {
                if (_azureApim == null)
                {
                    if (Tenant.properties.ContainsKey(AzureApiManagementConsts.Host) &&
                    Tenant.properties.ContainsKey(AzureApiManagementConsts.ApiId) &&
                    Tenant.properties.ContainsKey(AzureApiManagementConsts.ApiKey))
                    {
                        _azureApim = new AzureApiManagementServices(
                            Tenant.properties[AzureApiManagementConsts.Host],
                            Tenant.properties[AzureApiManagementConsts.ApiId],
                            Tenant.properties[AzureApiManagementConsts.ApiKey]);
                    }
                }

                return _azureApim;
            }
        }
    }
}