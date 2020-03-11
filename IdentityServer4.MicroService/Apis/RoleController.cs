using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Models.Apis.Common;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// 角色
    /// </summary>
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Role")]
    [SwaggerTag("角色")]
    public class RoleController : ApiControllerBase
    {
        #region 构造函数
        public RoleController(
           UserDbContext _db,
           IStringLocalizer<RoleController> localizer)
        {
            db = _db;
            l = localizer;
        }
        #endregion

        #region 角色 - 列表
        /// <summary>
        /// 角色 - 列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:role.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:role.get")]
        [SwaggerOperation(OperationId = "RoleGet",
            Summary = "角色 - 列表",
            Description = "scope&permission：isms.role.get")]
        public async Task<PagingResult<AppRole>> Get()
        {
            var data = await db.Roles
                .Include(x => x.Claims)
                .ToListAsync();

            var total = await db.Roles.CountAsync();

            return new PagingResult<AppRole>(data, total, 0, total);
        }
        #endregion

        #region 角色 - 详情
        /// <summary>
        /// 角色 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:role.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:role.detail")]
        [SwaggerOperation(OperationId = "RoleDetail",
            Summary = "角色 - 详情",
            Description = "scope&permission：isms.role.detail")]
        public async Task<ApiResult<AppRole>> Get(int id)
        {
            var entity = await db.Roles
                .Include(x => x.Claims)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<AppRole>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<AppRole>(entity);
        }
        #endregion

        #region 角色 - 创建
        /// <summary>
        /// 角色 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:role.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:role.post")]
        [SwaggerOperation(OperationId = "RolePost",
            Summary = "角色 - 创建",
            Description = "scope&permission：isms.role.post")]
        public async Task<ApiResult<long>> Post([FromBody]AppRole value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            db.Add(value);

            await db.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region 角色 - 更新
        /// <summary>
        /// 角色 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:role.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:role.put")]
        [SwaggerOperation(OperationId = "RolePut",
            Summary = "角色 - 更新",
            Description = "scope&permission：isms.role.put")]
        public async Task<ApiResult<long>> Put([FromBody]AppRole value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            using (var tran = db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    db.Update(value);
                    db.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await db.Roles.Where(x => x.Id == value.Id)
                                    .Include(x => x.Claims)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
                    #endregion

                    #region Update Entity.Claims
                    if (value.Claims != null && value.Claims.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Claims.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Claims.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE AspNetRoleClaims WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlRaw($"DELETE AspNetRoleClaims WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                //var sql = new RawSqlString("UPDATE AspNetRoleClaims SET [ClaimType]=@ClaimType,[ClaimValue]=@ClaimValue WHERE Id = " + x.Id);

                                //var _params = new SqlParameter[]
                                //{
                                //    new SqlParameter("@ClaimType", DBNull.Value){  IsNullable=true },
                                //    new SqlParameter("@ClaimValue", DBNull.Value){  IsNullable=true },
                                //};

                                //if (!string.IsNullOrWhiteSpace(x.ClaimType)) { _params[0].Value = x.ClaimType; }
                                //if (!string.IsNullOrWhiteSpace(x.ClaimValue)) { _params[1].Value = x.ClaimValue; }

                                db.Database.ExecuteSqlRaw($"UPDATE AspNetRoleClaims SET [ClaimType]={x.ClaimType},[ClaimValue]={x.ClaimValue} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                var _params = new SqlParameter[]
                                {
                                    new SqlParameter("@RoleId", source.Id),
                                    new SqlParameter("@ClaimType", DBNull.Value){  IsNullable=true },
                                    new SqlParameter("@ClaimValue", DBNull.Value){  IsNullable=true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.ClaimType)) { _params[1].Value = x.ClaimType; }

                                if (!string.IsNullOrWhiteSpace(x.ClaimValue)) { _params[2].Value = x.ClaimValue; }

                                db.Database.ExecuteSqlRaw("INSERT INTO AspNetRoleClaims VALUES (@ClaimType,@ClaimValue,@RoleId)", _params);
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

        #region 角色 - 删除
        /// <summary>
        /// 角色 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:role.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:role.delete")]
        [SwaggerOperation(OperationId = "RoleDelete",
            Summary = "角色 - 删除",
            Description = "scope&permission：isms.role.delete")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            var entity = await db.Roles.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            db.Roles.Remove(entity);

            await db.SaveChangesAsync();

            return new ApiResult<long>(id);
        }
        #endregion

        #region 角色 - 错误码表
        /// <summary>
        /// 角色 - 错误码表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "RoleCodes",
            Summary = "角色 - 错误码表",
            Description = "角色码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<RoleControllerEnums>();

            return result;
        }
        #endregion
    }
}
