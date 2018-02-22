using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using IdentityServer4.MicroService.Models.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static IdentityServer4.MicroService.MicroserviceConfig;

namespace IdentityServer4.MicroService.Services
{
    public class AccountService
    {
        private readonly IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            IIdentityServerInteractionService interaction,
            IHttpContextAccessor httpContextAccessor,
            IClientStore clientStore)
        {
            _interaction = interaction;
            _httpContextAccessor = httpContextAccessor;
            _clientStore = clientStore;
        }

        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            var user = _httpContextAccessor.HttpContext.User;
            if (user == null || user.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            var user =  _httpContextAccessor.HttpContext.User;
            if (user != null)
            {
                var idp = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    if (vm.LogoutId == null)
                    {
                        // if there's no current logout context, we need to create one
                        // this captures necessary info from the current logged in user
                        // before we signout and redirect away to the external IdP for signout
                        vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                    }

                    vm.ExternalAuthenticationScheme = idp;
                }
            }

            return vm;
        }


        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="TenantId">当前平台ID</param>
        /// <param name="userManager"></param>
        /// <param name="userContext"></param>
        /// <param name="user">新用户</param>
        /// <param name="roleIds">角色集合</param>
        /// <param name="permissions">权限集合</param>
        /// <param name="tenantIds">平台集合</param>
        /// <param name="parentUserID">上级用户ID</param>
        /// <returns></returns>
        public static async Task<IdentityResult> CreateUser(
            long TenantId,
            UserManager<AppUser> userManager,
            IdentityDbContext userContext,
            AppUser user,
            List<long> roleIds,
            string permissions,
            List<long> tenantIds,
            long parentUserID = 1)
        {
            var result = await userManager.CreateAsync(user, user.PasswordHash);

            if (result.Succeeded)
            {
                user.LineageIDs = user.Id.ToString();

                var ParentUser = userContext.Users
                                .Include(x => x.Distributions)
                                .FirstOrDefault(x => x.Id == parentUserID);

                if (user.Id == 1)
                {
                    user.Lineage = "/1/";
                    user.LineageIDs = "1";
                }
                else
                {
                    //叠加上级用户关系链
                    user.Lineage = $"{ParentUser.Lineage}{user.Id}/";

                    //更新上级用户关系链
                    using (var con = userContext.Database.GetDbConnection())
                    {
                        con.Open();

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT ','+CONVERT(NVARCHAR,ID) from yy_User where Lineage.IsDescendantOf(hierarchyid::Parse ('{ParentUser.Lineage}')) = 1	For xml path('')";

                            var LineageIDs =  cmd.ExecuteScalar();

                            if(LineageIDs!=null)
                            {
                                var _LineageIDs = LineageIDs.ToString().Substring(1);

                                ParentUser.LineageIDs = _LineageIDs;
                            }
                        }
                    }
                }

                user.CreateDate = DateTime.UtcNow.AddHours(8);

                #region User Roles
                foreach (var roleId in roleIds)
                {
                    user.Roles.Add(new AppUserRole()
                    {
                        RoleId = roleId,
                        UserId = user.Id
                    });
                }
                #endregion

                #region User Claims.Permissions
                if (!string.IsNullOrWhiteSpace(permissions))
                {
                    user.Claims.Add(new AppUserClaim()
                    {
                        UserId = user.Id,
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
                        UserId = user.Id,
                        TenantId = tid
                    });

                    user.Distributions.Add(new AspNetUserDistribution()
                    {
                        TenantId = tid,
                        UserId = user.Id
                    });
                }
                #endregion

                userContext.SaveChanges();

                //存在上级用户
                if (user.Id != parentUserID &&
                    ParentUser != null)
                {
                    #region update current tenant distributions
                    // 上级用户当前平台的分销详情
                    // 当前是平台A 就更新平台A的详情
                    // 当前是平台B 就更新平台B的详情
                    var ParentUserDistribution = ParentUser.Distributions.FirstOrDefault(x => x.TenantId == TenantId);
                    //上级用户关系链总用户数
                    var Members = userContext.Users.Where(x => x.ParentUserID == parentUserID).Count();

                    //存在就更新，不存在就创建
                    if (ParentUserDistribution != null)
                    {
                        ParentUserDistribution.Members = Members;
                        ParentUserDistribution.MembersLastUpdate = DateTime.UtcNow;
                    }
                    else
                    {
                        ParentUser.Distributions.Add(new AspNetUserDistribution()
                        {
                            Members = Members,
                            MembersLastUpdate = DateTime.UtcNow
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
