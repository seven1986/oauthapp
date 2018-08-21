using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.IdentityResourceController;
using static IdentityServer4.MicroService.AppConstant;
using static IdentityServer4.MicroService.MicroserviceConfig;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// 身份服务
    /// </summary>
    [Route("IdentityResource")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Administrators)]
    public class IdentityResourceController : BasicController
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

        #region 身份服务 - 列表
        /// <summary>
        /// 身份服务 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.identityresource.get</code>
        /// <label>User Permissions：</label><code>ids4.ms.identityresource.get</code>
        /// </remarks>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.IdentityResourceGet)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.IdentityResourceGet)]
        [SwaggerOperation("IdentityResource/Get")]
        public async Task<PagingResult<IdentityResource>> Get(PagingRequest<IdentityResourceGetRequest> value)
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

        #region 身份服务 - 详情
        /// <summary>
        /// 身份服务 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.identityresource.detail</code>
        /// <label>User Permissions：</label><code>ids4.ms.identityresource.detail</code>
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.IdentityResourceDetail)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.IdentityResourceDetail)]
        [SwaggerOperation("IdentityResource/Detail")]
        public async Task<ApiResult<IdentityResource>> Get(int id)
        {
            var entity = await configDb.IdentityResources
                .Where(x => x.Id == id)
                .Include(x => x.UserClaims)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<IdentityResource>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<IdentityResource>(entity);
        }
        #endregion

        #region 身份服务 - 创建
        /// <summary>
        /// 身份服务 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.identityresource.post</code>
        /// <label>User Permissions：</label><code>ids4.ms.identityresource.post</code>
        /// </remarks>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.IdentityResourcePost)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.IdentityResourcePost)]
        [SwaggerOperation("IdentityResource/Post")]
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

        #region 身份服务 - 更新
        /// <summary>
        /// 身份服务 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.identityresource.put</code>
        /// <label>User Permissions：</label><code>ids4.ms.identityresource.put</code>
        /// </remarks>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.IdentityResourcePut)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.IdentityResourcePut)]
        [SwaggerOperation("IdentityResource/Put")]
        public async Task<ApiResult<long>> Put([FromBody]IdentityResource value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            using (var tran = configDb.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    configDb.Update(value);
                    configDb.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await configDb.IdentityResources.Where(x => x.Id == value.Id)
                                     .Include(x => x.UserClaims)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync();
                    #endregion

                    #region Update Entity.Claims
                    if (value.UserClaims != null && value.UserClaims.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.UserClaims.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.UserClaims.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE IdentityClaims WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                configDb.Database.ExecuteSqlCommand($"DELETE IdentityClaims WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.UserClaims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                configDb.Database.ExecuteSqlCommand($"UPDATE IdentityClaims SET [Type]={x.Type} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.UserClaims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                configDb.Database.ExecuteSqlCommand($"INSERT INTO IdentityClaims VALUES ({source.Id},{x.Type})");
                            });
                        }
                        #endregion
                    }
                    #endregion

                    tran.Commit();
                }

                catch (Exception ex)
                {
                    tran.Rollback();

                    return new ApiResult<long>(l,
                        BasicControllerEnums.ExpectationFailed,
                        ex.Message);
                }
            }

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region 身份服务 - 删除
        /// <summary>
        /// 身份服务 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// <label>Client Scopes：</label><code>ids4.ms.identityresource.delete</code>
        /// <label>User Permissions：</label><code>ids4.ms.identityresource.delete</code>
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.IdentityResourceDelete)]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.IdentityResourceDelete)]
        [SwaggerOperation("IdentityResource/Delete")]
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

        #region 身份服务 - 错误码表
        /// <summary>
        /// 身份服务 - 错误码表
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 身份服务代码对照表
        /// </remarks>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation("IdentityResource/Codes")]
        public List<ErrorCodeModel> Codes()
        {
            var result = _Codes<IdentityResourceControllerEnums>();

            return result;
        }
        #endregion
    }
}