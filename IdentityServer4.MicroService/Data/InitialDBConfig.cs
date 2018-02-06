using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.MicroService.Tenant;
using static IdentityServer4.MicroService.AppConstant;
using System.ComponentModel;
using System.Reflection;

namespace IdentityServer4.MicroService.Data
{
    /// <summary>
    ///  For 初始化数据
    /// </summary>
    public class InitialDBConfig
    {
        public class IdentityServer
        {
            public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(){  UserClaims={ "role","permission" } },
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone(),
                };
            }

            public static IEnumerable<ApiResource> GetApiResources()
            {
                return new List<ApiResource>
                {
                    new ApiResource()
                    {
                        Enabled =true,
                        ApiSecrets = {
                            new Secret("1".Sha256()),
                            new Secret("2".Sha256(),"desc",DateTime.UtcNow.AddYears(1)),
                        },
                        Name = "campaign.core.identity",
                        DisplayName = "campaign.core.identity",
                        Description = "campaign.core.identity",
                        Scopes = {
                            new Scope(MicroServiceName + ".approve","批准"),
                            new Scope(MicroServiceName + ".create","创建"),
                            new Scope(MicroServiceName + ".delete","删除"),
                            new Scope(MicroServiceName + ".read","读取"),
                            new Scope(MicroServiceName + ".reject","拒绝"),
                            new Scope(MicroServiceName + ".update","更新"),
                            new Scope(MicroServiceName + ".upload","上传"),
                            new Scope(MicroServiceName + ".all","所有权限")
                        },
                          
                        //需要使用的用户claims
                        UserClaims= {
                            "permission",
                            "role"
                        }
                    }
                };
            }

            public static IEnumerable<Client> GetClients()
            {
                // client credentials client
                return new List<Client>
                {
                    #region Campaign.H5Game
		            new Client
                    {
                        ClientId = "xingbangames",
                        ClientName = "xingbangames",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("1".Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,

                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris = { "http://localhost:5002/signin-oidc" },
                        PostLogoutRedirectUris = { "http://localhost:5002/signout-callback" },

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            MicroServiceName + ".all",
                        },
                        AllowOfflineAccess = true,
                        RequireConsent=false
                    }, 
	                #endregion
                    #region 开放平台接口测试专用
		            new Client
                    {
                        ClientId = "h5game",
                        ClientName = "h5game",
                        AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("1".Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,

                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris = { "https://portal.ixingban.com/docs/services/59efe9dd88269013808d7cf0/console/oauth2/implicit/callback" },
                        PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            MicroServiceName+ ".all",
                            "campaign.game.all"
                        },
                        AllowOfflineAccess = true,
                    }, 
	                #endregion
                    #region Campaign后台
		            new Client
                    {
                        ClientId = "adminportal",
                        ClientName = "adminportal",
                        AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("1".Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,

                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris = { "https://campaigncore-adminportal.chinacloudsites.cn/auth-callback","http://localhost:4200/auth-callback" },
                        PostLogoutRedirectUris = { "http://localhost:4200/logout-callback","https://campaigncore-adminportal.chinacloudsites.cn/logout-callback" },

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            MicroServiceName + ".all",
                            "campaign.game.all"
                        },
                        AllowOfflineAccess = true
                    },
	                #endregion
                    #region API测试专用
		            new Client
                    {
                        ClientId = "test",
                        ClientName = "API测试专用",
                        AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                        ClientSecrets =
                        {
                            new Secret("1".Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,

                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris = { "https://ids.ixingban.com/swagger/o2c.html" },

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            MicroServiceName + ".all",
                            "campaign.game.all"
                        },
                        AllowOfflineAccess = true
                    }
	                #endregion
                };
            }
        }

        public class Identity
        {
            public static IEnumerable<AppRole> GetRoles()
            {
                var result = new List<AppRole>();

                var roles = typeof(Roles).GetFields();

                foreach (var role in roles)
                {
                    var roleName = role.GetRawConstantValue().ToString();

                    var roleDisplayName = role.GetCustomAttribute<DisplayNameAttribute>().DisplayName;

                    result.Add(new AppRole
                    {
                        Name = roleName,
                        NormalizedName = roleDisplayName,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                return result;
            }

            public static IEnumerable<AppUser> GetUsers()
            {
                return new List<AppUser>
                {
                     new AppUser()
                     {
                        Email="1@1.com",
                        UserName="1@1.com",
                        PasswordHash="123456aA!",
                        EmailConfirmed=true
                     }
                };
            }
        }

        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                tenantDbContext.Database.Migrate();

                #region identityserver
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in InitialDBConfig.IdentityServer.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in InitialDBConfig.IdentityServer.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in InitialDBConfig.IdentityServer.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                #endregion

                #region user,userroles,userclients
                var userContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                userContext.Database.Migrate();
                if (!userContext.Roles.Any())
                {
                    foreach (var role in InitialDBConfig.Identity.GetRoles())
                    {
                        userContext.Roles.Add(role);
                    }
                    userContext.SaveChanges();
                }
                if (!userContext.Users.Any())
                {
                    var roleIds = userContext.Roles.Where(x =>
                               x.Name.Equals(AppConstant.Roles.Users) ||
                               x.Name.Equals(AppConstant.Roles.Administrators)).Select(x => x.Id).ToList();

                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    foreach (var user in InitialDBConfig.Identity.GetUsers())
                    {
                        var r = userManager.CreateAsync(user, user.PasswordHash).Result;

                        if (r.Succeeded)
                        {
                            foreach (var roleId in roleIds)
                            {
                                user.Roles.Add(new AppUserRole()
                                {
                                    RoleId = roleId,
                                    UserId = user.Id
                                });
                            }
                        }
                    }

                    userContext.SaveChanges();

                    #region user and clients
                    var usersIds = userContext.Users.Select(x => x.Id).ToList();
                    var clientIds = context.Clients.Select(x => x.Id).ToList();
                    foreach (var v in usersIds)
                    {
                        foreach (var vv in clientIds)
                        {
                            userContext.UserClients.Add(new AspNetUserClient()
                            {
                                UserId = v,
                                ClientId = vv
                            });
                        }
                    }
                    userContext.SaveChanges(); 
                    #endregion
                }
                #endregion
            }
        }
    }
}
