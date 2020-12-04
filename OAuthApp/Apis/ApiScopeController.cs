using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Data;
using OAuthApp.Tenant;
using OAuthApp.Enums;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using OAuthApp.Models.Apis.Common;
using static OAuthApp.AppConstant;
using OAuthApp.Models.Apis.ApiScopeController;

namespace OAuthApp.Apis
{
    /// <summary>
    /// APIScope
    /// </summary>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("ApiScope")]
    [SwaggerTag("#### API权限管理")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApiScopeController : ApiControllerBase
    {
        #region Services
        //Database
        readonly ConfigurationDbContext configDb;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_configDb"></param>
        /// <param name="_userDb"></param>
        /// <param name="_localizer"></param>
        /// <param name="_tenantService"></param>
        /// <param name="_tenantDb"></param>
        /// <param name="_provider"></param>
        public ApiScopeController(
            ConfigurationDbContext _configDb,
            UserDbContext _userDb,
            IStringLocalizer<ApiScopeController> _localizer,
            TenantService _tenantService,
            TenantDbContext _tenantDb,
            IDataProtectionProvider _provider)
        {
            configDb = _configDb;
            db = _userDb;
            l = _localizer;
            tenantDb = _tenantDb;
            tenantService = _tenantService;
            protector = _provider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();
        }
        #endregion

        #region API
        #region API - 列表
        /// <summary>
        /// API - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiscope.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiscope.get")]
        [SwaggerOperation(
            OperationId = "ApiScopeGet",
            Summary = "API - 列表",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.apiscope.get | oauthapp.apiscope.get |")]
        public async Task<PagingResult<ApiScope>> Get([FromQuery]PagingRequest<ApiScopeGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<ApiScope>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = configDb.ApiScopes.AsQueryable();

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.name))
            {
                query = query.Where(x => x.Name.Equals(value.q.name));
            }

            if (value.q.expandProperties)
            {
                query = query.Include(x => x.Properties);
            }

            if (value.q.expandClaims)
            {
                query = query.Include(x => x.UserClaims);
            }
            #endregion

            #region total
            var result = new PagingResult<ApiScope>()
            {
                skip = value.skip.Value,
                take = value.take.Value,
                total = await query.CountAsync()
            };
            #endregion

            if (result.total > 0)
            {
                #region orderby
                if (!string.IsNullOrWhiteSpace(value.orderby))
                {
                    if (value.asc.Value)
                    {
                        query = query.OrderBy(value.orderby);
                    }
                    else
                    {
                        query = query.OrderByDescending(value.orderby);
                    }
                }
                #endregion

                #region pagingWithData
                var data = await query.Skip(value.skip.Value).Take(value.take.Value)
                    .ToListAsync();
                #endregion

                result.data = data;
            }

            return result;
        }
        #endregion

        #region API - 详情
        /// <summary>
        /// API - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiscope.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiscope.detail")]
        [SwaggerOperation(OperationId = "ApiScopeDetail",
            Summary = "API - 详情",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.apiscope.detail | oauthapp.apiscope.detail |")]
        public async Task<ApiResult<ApiScope>> Get(long id)
        {
            var query = configDb.ApiScopes.AsQueryable();

            var entity = await query
                .Where(x => x.Id == id)
                .Include(x => x.Properties)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<ApiScope>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<ApiScope>(entity);
        }
        #endregion

        #region API - 创建
        /// <summary>
        /// API - 创建
        /// </summary>
        /// <param name="value">ID</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiscope.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiscope.post")]
        [SwaggerOperation(
            OperationId = "ApiScopePost",
            Summary = "API - 创建",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.apiscope.post | oauthapp.apiscope.post |")]
        public ApiResult<long> Post([FromBody]ApiScope value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            configDb.Add(value);

            try
            {
                configDb.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<long>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = 0
                };
            }

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region API - 更新
        /// <summary>
        /// API - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiscope.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiscope.put")]
        [SwaggerOperation(
            OperationId = "ApiScopePut",
            Summary = "API - 更新",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.apiscope.put | oauthapp.apiscope.put |")]
        public ApiResult<bool> Put([FromBody]ApiScope value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var Entity = configDb.ApiScopes.Where(x => x.Id == value.Id)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .FirstOrDefault();

            if (Entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            if (!string.IsNullOrWhiteSpace(value.Name) &&
               !value.Name.Equals(Entity.Name))
            {
                Entity.Name = value.Name;
            }

            if (!string.IsNullOrWhiteSpace(value.DisplayName) &&
               !value.DisplayName.Equals(Entity.DisplayName))
            {
                Entity.DisplayName = value.DisplayName;
            }

            if (!string.IsNullOrWhiteSpace(value.Description) &&
              !value.Description.Equals(Entity.Description))
            {
                Entity.Description = value.Description;
            }

            Entity.Enabled = value.Enabled;
            Entity.Required = value.Required;
            Entity.Emphasize = value.Emphasize;
            Entity.ShowInDiscoveryDocument = value.ShowInDiscoveryDocument;

            #region Properties
            if (Entity.Properties != null && Entity.Properties.Count > 0)
            {
                Entity.Properties.Clear();
            }
            if (value.Properties != null && value.Properties.Count > 0)
            {
                Entity.Properties = value.Properties
                   .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                   .Select(x => new ApiScopeProperty()
                   {
                       Scope = Entity,
                       ScopeId = value.Id,
                       Key = x.Key,
                       Value = x.Value
                   }).ToList();
            }
            #endregion

            #region UserClaims
            if (Entity.UserClaims != null && Entity.UserClaims.Count > 0)
            {
                Entity.UserClaims.Clear();
            }
            if (value.UserClaims != null && value.UserClaims.Count > 0)
            {
                Entity.UserClaims = value.UserClaims
                    .Where(x => !string.IsNullOrWhiteSpace(x.Type))
                    .Select(x => new ApiScopeClaim()
                    {
                        Scope = Entity,
                        ScopeId = value.Id,
                        Type = x.Type
                    }).ToList();
            }
            #endregion

            try
            {
                configDb.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = false
                };
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region API - 删除
        /// <summary>
        /// API - 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:apiscope.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:apiscope.delete")]
        [SwaggerOperation(
            OperationId = "ApiScopeDelete",
            Summary = "API - 删除",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.apiscope.delete | oauthapp.apiscope.delete |")]
        public ApiResult<bool> Delete(long id)
        {
            var entity = configDb.ApiScopes.Where(x => x.Id == id)
                .Include(x => x.Properties)
                .Include(x => x.UserClaims)
                .FirstOrDefault();

            if (entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound);
            }

            configDb.ApiScopes.Remove(entity);

            try
            {
                configDb.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message)
                {
                    data = false
                };
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region API - 错误码表
        /// <summary>
        /// API - 错误码表
        /// </summary>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "ApiScopeCodes",
            Summary = "API - 错误码表",
            Description = "API代码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<ApiScopeControllerEnums>();

            return result;
        }
        #endregion
        #endregion
    }
}
