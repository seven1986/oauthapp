using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Enums;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.IdentityResourceController;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// 标识
    /// </summary>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("IdentityResource")]
    [SwaggerTag("#### 标识管理")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class IdentityResourceController : ApiControllerBase
    {
        #region Services
        //Database
        readonly ConfigurationDbContext configDb;
        #endregion

        #region 构造函数
        public IdentityResourceController(
            ConfigurationDbContext _configDb,
            IStringLocalizer<IdentityResourceController> localizer)
        {
            configDb = _configDb;
            l = localizer;
        }
        #endregion

        #region 标识 - 列表
        /// <summary>
        /// 标识 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:identityresource.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:identityresource.get")]
        [SwaggerOperation(OperationId = "IdentityResourceGet",
            Summary = "标识 - 列表",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.identityresource.get | oauthapp.identityresource.get |")]
        public async Task<PagingResult<IdentityResource>> Get([FromQuery]PagingRequest<IdentityResourceGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<IdentityResource>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = configDb.IdentityResources.AsQueryable();

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.Name))
            {
                query = query.Where(x => x.Name.Equals(value.q.Name));
            }
            #endregion

            #region total
            var result = new PagingResult<IdentityResource>()
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
                    .Include(x => x.UserClaims)
                    .ToListAsync();
                #endregion

                result.data = data;
            }

            return result;
        }
        #endregion

        #region  标识 - 详情
        /// <summary>
        ///  标识 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:identityresource.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:identityresource.detail")]
        [SwaggerOperation(OperationId = "IdentityResourceDetail",
            Summary = " 标识 - 详情",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.identityresource.detail | oauthapp.identityresource.detail |")]
        public async Task<ApiResult<IdentityResource>> Get(int id)
        {
            var entity = await configDb.IdentityResources
                .Where(x => x.Id == id)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<IdentityResource>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<IdentityResource>(entity);
        }
        #endregion

        #region  标识 - 创建
        /// <summary>
        ///  标识 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:identityresource.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:identityresource.post")]
        [SwaggerOperation(OperationId = "IdentityResourcePost",
            Summary = " 标识 - 创建",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.identityresource.post | oauthapp.identityresource.post |")]
        public async Task<ApiResult<long>> Post([FromBody]IdentityResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            configDb.Add(value);

            await configDb.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region  标识 - 更新
        /// <summary>
        ///  标识 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:identityresource.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:identityresource.put")]
        [SwaggerOperation(OperationId = "IdentityResourcePut",
            Summary = "标识 - 更新",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.identityresource.put | oauthapp.identityresource.put |")]
        public ApiResult<bool> Put([FromBody] IdentityResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var Entity = configDb.IdentityResources.Where(x => x.Id == value.Id)
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
            Entity.Created = value.Created;
            Entity.Updated = value.Updated.GetValueOrDefault();
            Entity.NonEditable = value.NonEditable;

            #region Properties
            if (Entity.Properties != null && Entity.Properties.Count > 0)
            {
                Entity.Properties.Clear();
            }
            if (value.Properties != null && value.Properties.Count > 0)
            {
                Entity.Properties = value.Properties
                  .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                  .Select(x => new IdentityResourceProperty()
                  {
                      IdentityResource = Entity,
                      IdentityResourceId = value.Id,
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
                    .Select(x => new IdentityResourceClaim()
                    {
                        IdentityResource = Entity,
                        IdentityResourceId = value.Id,
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

        #region  标识 - 删除
        /// <summary>
        ///  标识 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:identityresource.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:identityresource.delete")]
        [SwaggerOperation(OperationId = "IdentityResourceDelete",
            Summary = "标识 - 删除",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.identityresource.delete | oauthapp.identityresource.delete |")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            var entity = await configDb.IdentityResources.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            configDb.IdentityResources.Remove(entity);

            await configDb.SaveChangesAsync();

            return new ApiResult<long>(id);
        } 
        #endregion

        #region  标识 - 错误码表
        /// <summary>
        ///  标识 - 错误码表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "IdentityResourceCodes",
            Summary = "标识 - 错误码表",
            Description = "标识错误码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<IdentityResourceControllerEnums>();

            return result;
        }
        #endregion
    }
}