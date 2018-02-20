using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
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
using Microsoft.Extensions.Configuration;

namespace IdentityServer4.MicroService.Data
{
    /// <summary>
    ///  数据库 - 数据初始化
    /// </summary>
    public class InitialDBConfig
    {
        public class IdentityServer
        {
            public static IEnumerable<IdentityResource> GetIdentityResources()
            {
                return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(){  UserClaims={ "role", ClaimTypes.UserPermission } },
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone(),
                };
            }

            public static IEnumerable<ApiResource> GetApiResources()
            {
                var scopes = typeof(ClientScopes).GetFields().Select(x =>
                {
                    var permissionValues = 
                    x.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues;

                    var description =
                    x.GetCustomAttribute<DescriptionAttribute>().Description;

                    return new Scope(permissionValues[0], description);

                }).ToList();

                scopes.Add(new Scope(MicroServiceName + ".all", "所有权限"));

                return new List<ApiResource>
                {
                    new ApiResource()
                    {
                        Enabled =true,
                        ApiSecrets = {},
                        Name = MicroServiceName,
                        DisplayName = MicroServiceName,
                        Description = MicroServiceName,
                        Scopes = scopes,
                        //需要使用的用户claims
                        UserClaims= {
                            ClaimTypes.UserPermission,
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
                    #region Default Test Client
		            new Client
                    {
                        ClientId = TestClient.ClientId,
                        ClientName = TestClient.ClientName,
                        AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                        ClientSecrets =
                        {
                            new Secret(TestClient.ClientSecret.Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,
                        // 需要设置为true，否则token无法附加tenant相关信息
                        AlwaysSendClientClaims = true,
                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris = TestClient.RedirectUris,

                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            MicroServiceName + ".all"
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
                        Email=DefaultAdmin.Email,
                        UserName=DefaultAdmin.UserName,
                        PasswordHash=DefaultAdmin.PasswordHash,
                        EmailConfirmed=true
                     }
                };
            }
        }

        public static void InitializeDatabase(IApplicationBuilder app, IConfigurationRoot config)
        {
            DefaultData.AppHostName = config["IdentityServer"];

            DefaultData.IdentityServerIssuerUri = DefaultData.AppHostName;

            TestClient.RedirectUris[0] = string.Format(TestClient.RedirectUris[0], DefaultData.AppHostName);

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                #region identityserver
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityServer.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in IdentityServer.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiResources.Any())
                {
                    foreach (var resource in IdentityServer.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                #endregion

                var userContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                userContext.Database.Migrate();

                if (!userContext.Roles.Any())
                {
                    foreach (var role in Identity.GetRoles())
                    {
                        userContext.Roles.Add(role);
                    }
                    userContext.SaveChanges();
                }

                if (!userContext.Users.Any())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    foreach (var _user in Identity.GetUsers())
                    {
                        var r = userManager.CreateAsync(_user, _user.PasswordHash).Result;

                        if (r.Succeeded)
                        {
                            _user.LineageIDs = _user.Id.ToString();
                            _user.Lineage = "/1/";
                            _user.CreateDate = DateTime.UtcNow.AddHours(8);
                            _user.Distribution = new AspNetUserDistribution()
                            {
                                UserId = _user.Id,
                            };

                            #region User Roles
                            var roleIds = userContext.Roles.Select(x => x.Id).ToList();
                            foreach (var roleId in roleIds)
                            {
                                _user.Roles.Add(new AppUserRole()
                                {
                                    RoleId = roleId,
                                    UserId = _user.Id
                                });
                            }
                            #endregion

                            #region User Permissions
                            var permissions = typeof(UserPermissions).GetFields().Select(x => x.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues[0]).ToList();
                            permissions.Add(MicroServiceName + ".all");
                            _user.Claims.Add(new AppUserClaim()
                            {
                                UserId = _user.Id,
                                ClaimType = ClaimTypes.UserPermission,
                                ClaimValue = string.Join(",", permissions),
                            });
                            #endregion

                            #region User ApiResources
                            var apiIds = context.ApiResources.Select(x => x.Id).ToList();
                            foreach (var apiId in apiIds)
                            {
                                userContext.UserApiResources.Add(new AspNetUserApiResource()
                                {
                                    ApiResourceId = apiId,
                                    UserId = _user.Id
                                });
                            }
                            #endregion

                            #region User Clients
                            var clientIds = context.Clients.Select(x => x.Id).ToList();
                            foreach (var cid in clientIds)
                            {
                                userContext.UserClients.Add(new AspNetUserClient()
                                {
                                    UserId = _user.Id,
                                    ClientId = cid
                                });
                            }
                            #endregion
                           
                            var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                            tenantDbContext.Database.Migrate();

                            if (!tenantDbContext.Tenants.Any())
                            {
                                #region Create Default Tenant
                                var tenant = new AppTenant()
                                {
                                    CacheDuration = 600,
                                    CreateDate = DateTime.UtcNow,
                                    IdentityServerIssuerUri = DefaultData.IdentityServerIssuerUri,
                                    LastUpdateTime = DateTime.UtcNow,
                                    Name = "默认",
                                    OwnerUserId = _user.Id,
                                    Status = TenantStatus.Enable,
                                    Theme = "default"
                                };
                                tenant.Hosts.Add(new AppTenantHost() { HostName = DefaultData.AppHostName });
                                tenant.Properties.AddRange(DefaultData.TenantProperties.Select(x => new AppTenantProperty() { Key = x.Key, Value = x.Value }));
                                tenantDbContext.Tenants.Add(tenant);
                                tenantDbContext.SaveChanges();
                                #endregion

                                #region User Tenants
                                _user.Tenants.Add(new AspNetUserTenant()
                                {
                                    AppUserId = _user.Id,
                                    AppTenantId = tenant.Id
                                });
                                #endregion
                            }
                        }
                    }

                    userContext.SaveChanges();
                }
            }
        }
    }
}
