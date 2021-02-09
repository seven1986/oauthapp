using System;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuthApp.Data;
using OAuthApp.Tenant;
using OAuthApp.Services;
using OAuthApp.CacheKeys;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Shared;
using static OAuthApp.AppConstant;
using System.Security.Claims;
using IdentityModel;

namespace OAuthApp.Apis
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme)]
    public class ApiControllerBase : ControllerBase
    {
        #region Services
        public virtual IStringLocalizer l { get; set; }
        public virtual RedisService redis { get; set; }
        public virtual ITimeLimitedDataProtector protector { get; set; } 
        #endregion

        protected readonly Random random = new Random(DateTime.UtcNow.AddHours(8).Second);
        protected long UserId
        {
            get
            {
                var subClaim = UserClaims(ClaimTypes.NameIdentifier);

                if (subClaim != null)
                {
                    return long.Parse(subClaim.Value);
                }

                return 0L;
            }
        }

        protected Claim UserClaims(string claimType)
        {
            return ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(x => x.Type.Equals(claimType));
        }

        private string _UserLineage;
        public string UserLineage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_UserLineage))
                {
                    var cmd = "SELECT Lineage.ToString() FROM AspNetUsers WHERE Id = " + UserId;

                    var Lineage = db.ExecuteScalarAsync(cmd).Result;

                    if (Lineage != null)
                    {
                        _UserLineage = Lineage.ToString();
                    }
                }

                return _UserLineage;
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

        protected string ClientId
        {
            get
            {
                return UserClaims(JwtClaimTypes.ClientId).Value;
            }
        }

        /// <summary>
        /// 租户信息 from client access token
        /// </summary>
        protected long TenantId
        {
            get
            {
                var tenant = UserClaims(TenantConstant.TokenKey);

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
        public virtual UserDbContext db { get; set; }

        private TenantPrivateModel _tenant;
        public TenantPrivateModel Tenant
        {
            get
            {
                if (_tenant == null)
                {
                    _tenant = HttpContext.GetTenantWithProperties();
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
                    if (Tenant.Properties.ContainsKey(AzureApiManagementKeys.Host) &&
                    Tenant.Properties.ContainsKey(AzureApiManagementKeys.ApiId) &&
                    Tenant.Properties.ContainsKey(AzureApiManagementKeys.ApiKey))
                    {
                        _azureApim = new AzureApiManagementServices(
                            Tenant.Properties[AzureApiManagementKeys.Host],
                            Tenant.Properties[AzureApiManagementKeys.ApiId],
                            Tenant.Properties[AzureApiManagementKeys.ApiKey]);
                    }
                }

                return _azureApim;
            }
        }

        /// <summary>
        /// 根据枚举，返回值与名称的字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected List<ApiCodeModel> _Codes<T>()
        {
            var t = typeof(T);

            var items = t.GetFields()
                .Where(x => x.CustomAttributes.Count() > 0).ToList();

            var result = new List<ApiCodeModel>();

            foreach (var item in items)
            {
                var code = long.Parse(item.GetRawConstantValue().ToString());

                var codeName = item.Name;

                var desc = item.GetCustomAttribute<DescriptionAttribute>();

                var codeItem = new ApiCodeModel()
                {
                    Code = code,
                    Name = codeName,
                    Description = l != null ? l[desc.Description] : desc.Description
                };

                result.Add(codeItem);
            }

            return result;
        }

        protected object ExecuteScalar(DbContext db, string sql, params SqlParameter[] sqlparams)
        {
            using (var connection = db.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.AddRange(sqlparams);

                    return command.ExecuteScalar();
                }
            }
        }
        protected void ExecuteReader(DbContext db, string sql, Action<DbDataReader> action, params SqlParameter[] sqlparams)
        {
            using (var connection = db.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    command.Parameters.AddRange(sqlparams);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                        }
                    }
                }
            }
        }

        #region MD5
        /// <summary>
        /// MD5
        /// </summary>
        /// <returns></returns>
        protected string _MD5(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(str);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        } 
        #endregion

        #region 加解密
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str">加密过的字符串</param>
        /// <returns></returns>
        protected string Unprotect(string str) => protector.Unprotect(str);

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str">未加密的字符串</param>
        /// <param name="expiredIn">有效时间</param>
        /// <returns></returns>
        protected string Protect(string str, TimeSpan expiredIn) => protector.Protect(str, expiredIn); 
        #endregion
    }
}