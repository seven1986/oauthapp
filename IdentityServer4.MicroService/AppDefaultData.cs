using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.MicroService.Services;
using Newtonsoft.Json;

namespace IdentityServer4.MicroService
{
    public class AppDefaultData
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public class Admin
        {
            public static string Email = "admin@admin.com";

            public static string PasswordHash = "123456aA!";
        }

        #region SwaggerClient
        /// <summary>
        /// SwaggerClient - SwaggerUI使用
        /// </summary>
        public class SwaggerClient
        {
            public static string ClientId = "swagger";
            public static string ClientName = "swagger";
            public static string ClientSecret = "swagger";
            public static List<string> AllowedGrantTypes = GrantTypes.CodeAndClientCredentials.ToList();
        }
        #endregion

        #region IdentityServer4Client
        /// <summary>
        /// IdentityServer4Client - 后台使用
        /// </summary>
        public class IdentityServer4Client
        {
            public static string ClientId = "identityserver4";

            public static string ClientName = "identityserver4";

            public static string ClientSecret = "identityserver4";

            public static List<string> AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials.ToList();

            public static List<string> PostLogoutRedirectUris = new List<string>();
        }
        #endregion

        /// <summary>
        /// 租户
        /// </summary>
        public class Tenant
        {
            public static string WebSite = "";//http://localhost:44309
            public static string IdentityServerIssuerUri = "";//localhost:44309
            public static string AppHostName = "";//localhost:44309
            public static string Name = "微服务";

            public static Dictionary<string, string> TenantProperties =
                    new Dictionary<string, string>()
                {
                     // Default Tenant Properties
                     { TenantDefaultProperty.AdminSite, ""},
                     { TenantDefaultProperty.Description, ""},
                     { TenantDefaultProperty.EnterpriseEmail, ""},
                     { TenantDefaultProperty.Keywords, ""},
                     { TenantDefaultProperty.PortalSite, ""},
                     { TenantDefaultProperty.Summary, ""},
                     { TenantDefaultProperty.Tracking, ""},
                     { TenantDefaultProperty.WebSite, ""},
                     { TenantDefaultProperty.Favicon, "favicon.ico"},
                     
                    //  Azure Api Management
                    { AzureApiManagementKeys.Host, ""},
                    { AzureApiManagementKeys.ApiId, "integration"},
                    { AzureApiManagementKeys.ApiKey, ""},
                    { AzureApiManagementKeys.AuthorizationServerId, ""},
                    { AzureApiManagementKeys.ProductId, ""},
                    { AzureApiManagementKeys.PortalUris, ""},
                    { AzureApiManagementKeys.DelegationKey, ""},
                };
        }

        public static IEnumerable<Client> GetClients(string MicroServiceName,Uri ServerUrl)
        {
            // client credentials client

            SwaggerClient.AllowedGrantTypes.Add("mobile_code");
            IdentityServer4Client.AllowedGrantTypes.Add("mobile_code");
            return new List<Client>
                {
                    #region SwaggerClient
		            new Client
                    {
                        ClientId = SwaggerClient.ClientId,
                        ClientName = SwaggerClient.ClientName,
                        AllowedGrantTypes = SwaggerClient.AllowedGrantTypes,
                        AllowAccessTokensViaBrowser = true,
                        ClientSecrets =
                        {
                            new Secret(SwaggerClient.ClientSecret.Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,
                        // 需要设置为true，否则token无法附加tenant相关信息
                        AlwaysSendClientClaims = true,
                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",
                        RedirectUris = {
                            $"{ServerUrl.OriginalString}/swagger/oauth2-redirect.html",
                            $"{ServerUrl.OriginalString}/tool"
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            MicroServiceName + ".all"
                        },
                        AllowOfflineAccess = true
                    },
	                #endregion

                    #region IdentityServer4Client
		            new Client
                    {
                        ClientId = IdentityServer4Client.ClientId,
                        ClientName = IdentityServer4Client.ClientName,
                        AllowedGrantTypes = IdentityServer4Client.AllowedGrantTypes,
                        AllowAccessTokensViaBrowser = true,
                        ClientSecrets =
                        {
                            new Secret(IdentityServer4Client.ClientSecret.Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,

                        // 需要设置为true，否则token无法附加tenant相关信息
                        AlwaysSendClientClaims = true,
                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris ={
                            $"{ServerUrl.OriginalString}/tool"
                        },
                        //PostLogoutRedirectUris = AdminPortalClient.PostLogoutRedirectUris,

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

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
                {
                    new IdentityResources.OpenId(){  UserClaims={ "role", PolicyKey.UserPermission } },
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone(),
                };
        }

        public static IEnumerable<ApiResource> GetApiResources(string MicroServiceName)
        {
            var ActionList = new List<Scope>()
            {
                new Scope(MicroServiceName + ".all", "所有权限")
            };

            return new List<ApiResource>
                {
                    new ApiResource()
                    {
                        Enabled =true,

                        ApiSecrets = {},

                        Name = MicroServiceName,

                        DisplayName = MicroServiceName,

                        Description = MicroServiceName,

                        Scopes = ActionList,

                        //需要使用的用户claims
                        UserClaims= {
                            PolicyKey.UserPermission,
                            "role"
                        },

                        Properties =new Dictionary<string,string>()
                    }
                };
        }

        /// <summary>
        /// 数据库初始化
        /// </summary>
        public static void InitializeDatabase(IApplicationBuilder app,string MicroServiceName,Uri ServerUrl)
        {
            Tenant.AppHostName = Tenant.IdentityServerIssuerUri = ServerUrl.Authority;

            Tenant.WebSite = ServerUrl.OriginalString;

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                #region PersistedGrantDb
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate(); 
                #endregion

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in GetClients(MicroServiceName,ServerUrl))
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in GetApiResources(MicroServiceName))
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                #region TenantDb
                var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                tenantDbContext.Database.Migrate();
                Data_Seeding_Tenants(tenantDbContext);
                #endregion

                #region IdentityDb
                var userContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                userContext.Database.Migrate();
                Data_Seeding_Users(userContext, tenantDbContext, userManager, context, MicroServiceName);
                #endregion
            }
        }

        static void Data_Seeding_Tenants(TenantDbContext tenantDbContext)
        {
            if (!tenantDbContext.Tenants.Any())
            {
                var tenant = new AppTenant()
                {
                    CacheDuration = 600,
                    CreateDate = DateTime.UtcNow,
                    IdentityServerIssuerUri = Tenant.IdentityServerIssuerUri,
                    LastUpdateTime = DateTime.UtcNow,
                    Name = Tenant.Name,
                    OwnerUserId = AppConstant.seedUserId, //默认设置为1
                    Status = TenantStatus.Enable,
                    Theme = "default"
                };
                tenant.Hosts.Add(new AppTenantHost() { HostName = Tenant.AppHostName });
                tenant.Properties.AddRange(Tenant.TenantProperties.Select(x => new AppTenantProperty() { Key = x.Key, Value = x.Value }));
                tenantDbContext.Tenants.Add(tenant);
                tenantDbContext.SaveChanges();
            }
        }

        static void Data_Seeding_Users(
            UserDbContext userContext, 
            TenantDbContext tenantDbContext,
            UserManager<AppUser> userManager,
            ConfigurationDbContext identityserverDbContext,
            string MicroServiceName)
        {
            if (!userContext.Roles.Any())
            {
                var roles = typeof(DefaultRoles).GetFields();

                foreach (var role in roles)
                {
                    var roleName = role.GetRawConstantValue().ToString();

                    var roleDisplayName = role.GetCustomAttribute<DisplayNameAttribute>().DisplayName;

                    userContext.Roles.Add(new AppRole
                    {
                        Name = roleName,
                        NormalizedName = roleDisplayName,
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    });
                }

                userContext.SaveChanges();
            }

            if (!userContext.Users.Any())
            {
                var roleIds = userContext.Roles.Select(x => x.Id).ToList();

                var tenantIds = tenantDbContext.Tenants.Select(x => x.Id).ToList();

                var user = new AppUser()
                {
                    Email = Admin.Email,
                    UserName = Admin.Email,
                    PasswordHash = Admin.PasswordHash,
                    EmailConfirmed = true,
                    ParentUserID = AppConstant.seedUserId
                };

                var r = AppUserService.CreateUser(AppConstant.seedTenantId,
                     userManager,
                     userContext,
                     user,
                     roleIds,
                    $"{MicroServiceName}.all",
                    tenantIds).Result;

                #region User Clients
                var clientIds = identityserverDbContext.Clients.Select(x => x.Id).ToList();
                foreach (var cid in clientIds)
                {
                    user.Clients.Add(new AspNetUserClient()
                    {
                        ClientId = cid
                    });
                }
                #endregion

                #region User ApiResources
                var apiIds = identityserverDbContext.ApiResources.Select(x => x.Id).ToList();
                foreach (var apiId in apiIds)
                {
                    user.ApiResources.Add(new AspNetUserApiResource()
                    {
                        ApiResourceId = apiId,
                    });
                }
                #endregion

                userContext.SaveChanges();
            }
        }

        ///// <summary>
        ///// 邮件发送模板
        ///// 需要对接sendcloud平台
        ///// https://sendcloud.sohu.com
        ///// </summary>
        //public enum SendCloudMailTemplates
        //{
        //    #region 用户注册 - 邮箱验证
        //    /// <summary>
        //    /// 用户注册 - 邮箱验证
        //    /// 变量：%code%
        //    /// </summary>
        //    [EmailConfig("邮箱验证")]
        //    verify_email,
        //    #endregion

        //    #region 订阅微服务 - 邮箱验证
        //    /// <summary>
        //    /// 订阅微服务 - 邮箱验证
        //    /// 变量：
        //    /// %SubscritionUrl%
        //    /// %DelSubscritionUrl%
        //    /// %apiId%
        //    /// %serviceName%
        //    /// </summary>
        //    [EmailConfig("验证邮箱")]
        //    verify_apiresource_subscription,
        //    #endregion

        //    #region 忘记密码
        //    /// <summary>
        //    /// 忘记密码
        //    /// 变量：%callbackUrl%
        //    /// </summary>
        //    [EmailConfig("reset password")]
        //    reset_password,
        //    #endregion

        //    #region 用户登录 - 邮箱安全码
        //    /// <summary>
        //    /// 用户登录 - 邮箱安全码
        //    /// 变量：%code%
        //    /// </summary>
        //    [EmailConfig("登录验证码")]
        //    security_code,
        //    #endregion

        //    #region 用户注册 - 激活邮箱
        //    /// <summary>
        //    /// 用户注册 - 激活邮箱
        //    /// 变量：%name%,%url%
        //    /// </summary>
        //    [EmailConfig("%name%请激活您的邮箱")]
        //    test_template_active,
        //    #endregion
        //}
    }
}
