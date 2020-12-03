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
using OAuthApp.Data;
using OAuthApp.Enums;
using OAuthApp.Models.Apis.Common;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// 角色
    /// </summary>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Role")]
    [SwaggerTag("#### 用户角色管理")]
    [Produces("application/json")]
    [Consumes("application/json")]
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
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.role.get | oauthapp.role.get |")]
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
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.role.detail | oauthapp.role.detail |")]
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
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.role.post | oauthapp.role.post |")]
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
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.role.put | oauthapp.role.put |")]
        public ApiResult<bool> Put([FromBody]AppRole value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }


            db.Attach(value).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
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
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.role.delete | oauthapp.role.delete |")]
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
