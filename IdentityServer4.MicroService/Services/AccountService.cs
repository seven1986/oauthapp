using IdentityServer4.MicroService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.MicroService.MicroserviceConfig;

namespace IdentityServer4.MicroService.Services
{
    public class AppUserService
    {
        /// <summary>
        /// 创建用户
        /// 注意设置 parentUserID 上级用户ID
        /// </summary>
        /// <param name="TenantId">当前平台ID</param>
        /// <param name="userManager">db</param>
        /// <param name="userContext">db</param>
        /// <param name="user">新用户</param>
        /// <param name="roleIds">角色集合</param>
        /// <param name="permissions">权限集合</param>
        /// <param name="tenantIds">平台集合</param>
        /// <returns></returns>
        public static async Task<IdentityResult> CreateUser(
            long TenantId,
            UserManager<AppUser> userManager,
            IdentityDbContext userContext,
            AppUser user,
            List<long> roleIds,
            string permissions,
            List<long> tenantIds)
        {
            if (user.ParentUserID == 0)
            {
                user.ParentUserID = AppConstant.seedUserId;
            }

            IdentityResult result = null;

            // 如果没有设置密码
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                result = await userManager.CreateAsync(user);
            }

            else
            {
                result = await userManager.CreateAsync(user, user.PasswordHash);
            }

            if (result.Succeeded)
            {
                user.LineageIDs = user.Id.ToString();

                var ParentUser = userContext.Users
                                .Include(x => x.Distributors)
                                .FirstOrDefault(x => x.Id == user.ParentUserID);

                if (user.Id == AppConstant.seedUserId)
                {
                    var sql = $"UPDATE AspNetUsers set Lineage = '/{AppConstant.seedUserId}/' WHERE ID = {user.Id}";

                    await userContext.ExecuteScalarAsync(sql);
                }
                else
                {
                    //叠加上级关系链到 当前用户
                    var sql = "DECLARE @ParentLineage nvarchar(max)\r\n" +
                              $"SELECT @ParentLineage = Lineage.ToString() FROM AspNetUsers WHERE Id = {ParentUser.Id}\r\n" +
                              $"UPDATE AspNetUsers set Lineage = @ParentLineage + '{user.Id}/' WHERE Id = {user.Id}";

                    await userContext.ExecuteScalarAsync(sql);

                    sql = $"UPDATE AspNetUsers SET LineageIDs = REPLACE(Lineage.ToString(),'/',',')  WHERE Id = {user.Id}";

                    // 叠加上级关系链到 当前用户
                    await userContext.ExecuteScalarAsync(sql);


                    // 上面加入了当前创建用户的关系，
                    // 所以这里要更新 上级用户关系链
                    sql = "DECLARE @ParentLineage hierarchyid \r\n" +
                         $"SELECT @ParentLineage = Lineage FROM AspNetUsers WHERE Id = {ParentUser.Id} \r\n" +
                          "SELECT ',' + CONVERT(NVARCHAR, Id) from AspNetUsers where Lineage.IsDescendantOf(@ParentLineage) = 1 For xml path('')";

                    var LineageIDs = await userContext.ExecuteScalarAsync(sql);

                    if (LineageIDs != null)
                    {
                        ParentUser.LineageIDs = LineageIDs.ToString().Substring(1);
                    }
                }

                user.CreateDate = DateTime.UtcNow.AddHours(8);

                #region User Roles
                foreach (var roleId in roleIds)
                {
                    user.Roles.Add(new AppUserRole()
                    {
                        RoleId = roleId,
                    });
                }
                #endregion

                #region User Claims.Permissions
                if (!string.IsNullOrWhiteSpace(permissions))
                {
                    user.Claims.Add(new AppUserClaim()
                    {
                        ClaimType = ClaimTypes.UserPermission,
                        ClaimValue = string.Join(",", permissions),
                    });
                }
                #endregion

                #region User Tenants and User Distributions
                foreach (var tid in tenantIds)
                {
                    user.Tenants.Add(new AspNetUserTenant()
                    {
                        TenantId = tid
                    });

                    user.Distributors.Add(new AspNetUserDistributor()
                    {
                        TenantId = tid,
                    });
                }
                #endregion

                userContext.SaveChanges();

                //存在上级用户，更新统计
                if (ParentUser != null &&
                    user.Id != user.ParentUserID)
                {
                    #region update current tenant distributions

                    // 上级用户当前平台的分销详情
                    // 当前是平台A 就更新平台A的详情
                    // 当前是平台B 就更新平台B的详情
                    var ParentUserDistribution = ParentUser.Distributors.FirstOrDefault(x => x.TenantId == TenantId);
                    //上级用户关系链总用户数
                    var Members = userContext.Users.Where(x => x.ParentUserID == user.ParentUserID).Count();

                    //存在就更新，不存在就创建
                    if (ParentUserDistribution != null)
                    {
                        ParentUserDistribution.Members = Members;
                        ParentUserDistribution.MembersLastUpdate = DateTime.UtcNow.AddHours(8);
                    }
                    else
                    {
                        ParentUser.Distributors.Add(new AspNetUserDistributor()
                        {
                            Members = Members,
                            MembersLastUpdate = DateTime.UtcNow.AddHours(8)
                        });
                    }
                    #endregion

                    userContext.SaveChanges();
                }
            }

            return result;
        }
    }
}
