using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Codes;
using IdentityServer4.MicroService.Models.CommonModels;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    [Route("Role")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class RoleController : BasicController
    {
        #region Services
        // database
        readonly ApplicationDbContext db;
        #endregion

        public RoleController(
            ApplicationDbContext _db,
            IStringLocalizer<RoleController> localizer)
        {
            db = _db;
            l = localizer;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("Role/Get")]
        public async Task<PagingResult<AppRole>> Get()
        {
            var data = await db.Roles
                .Include(x => x.Claims)
                .ToListAsync();

            var total = await db.Roles.CountAsync();

            return new PagingResult<AppRole>(data, total, 0, total);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("Role/Detail")]
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Create)]
        [SwaggerOperation("Role/Post")]
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

        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Update)]
        [SwaggerOperation("Role/Put")]
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
                                var sql = string.Format("DELETE AspNetRoleClaims WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                var sql = new RawSqlString("UPDATE AspNetRoleClaims SET [ClaimType]=@ClaimType,[ClaimValue]=@ClaimValue WHERE Id = " + x.Id);

                                var _params = new SqlParameter[]
                                {
                                    new SqlParameter("@ClaimType", DBNull.Value){  IsNullable=true },
                                    new SqlParameter("@ClaimValue", DBNull.Value){  IsNullable=true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.ClaimType)) { _params[0].Value = x.ClaimType; }
                                if (!string.IsNullOrWhiteSpace(x.ClaimValue)) { _params[1].Value = x.ClaimValue; }

                                db.Database.ExecuteSqlCommand(sql, _params);
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                var sql = new RawSqlString("INSERT INTO AspNetRoleClaims VALUES (@ClaimType,@ClaimValue,@RoleId)");

                                var _params = new SqlParameter[]
                                {
                                    new SqlParameter("@RoleId", source.Id),
                                    new SqlParameter("@ClaimType", DBNull.Value){  IsNullable=true },
                                    new SqlParameter("@ClaimValue", DBNull.Value){  IsNullable=true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.ClaimType)) { _params[1].Value = x.ClaimType; }
                                if (!string.IsNullOrWhiteSpace(x.ClaimValue)) { _params[2].Value = x.ClaimValue; }

                                db.Database.ExecuteSqlCommand(sql, _params);
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

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Delete)]
        [SwaggerOperation("Role/Delete")]
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
    }
}
