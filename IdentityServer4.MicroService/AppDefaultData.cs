using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNet.Security.OAuth.Amazon;
using AspNet.Security.OAuth.LinkedIn;
using AspNet.Security.OAuth.Instagram;
using AspNet.Security.OAuth.Paypal;
using AspNet.Security.OAuth.Gitter;
using AspNet.Security.OAuth.Reddit;
using AspNet.Security.OAuth.VisualStudio;
using AspNet.Security.OAuth.WordPress;
using AspNet.Security.OAuth.Salesforce;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using static IdentityServer4.MicroService.MicroserviceConfig;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Attributes;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.MicroService.Services;

namespace IdentityServer4.MicroService
{
    public class AppDefaultData
    {
        /// <summary>
        /// 初始管理员
        /// </summary>
        public class Admin
        {
            public const string Email = "1@1.com";
            public const string PasswordHash = "123456aA!";
        }

        /// <summary>
        /// 初始租户
        /// </summary>
        public class Tenant
        {
            public static void InitOAuthens()
            {
                //auth login
                InsertOAuth(AmazonAuthenticationDefaults.AuthenticationScheme, typeof(AmazonAuthenticationHandler));
                InsertOAuth(FacebookDefaults.AuthenticationScheme, typeof(FacebookHandler2));
                InsertOAuth(GitHubDefaults.AuthenticationScheme, typeof(GitHubHandler));
                InsertOAuth(GitterAuthenticationDefaults.AuthenticationScheme, typeof(GitterAuthenticationHandler));
                InsertOAuth(GoogleDefaults.AuthenticationScheme, typeof(GoogleHandler2));
                InsertOAuth(InstagramAuthenticationDefaults.AuthenticationScheme, typeof(InstagramAuthenticationHandler));
                InsertOAuth(LinkedInAuthenticationDefaults.AuthenticationScheme, typeof(LinkedInAuthenticationHandler));
                InsertOAuth(MicrosoftAccountDefaults.AuthenticationScheme, typeof(MicrosoftAccountHandler2));
                InsertOAuth(PaypalAuthenticationDefaults.AuthenticationScheme, typeof(PaypalAuthenticationHandler));
                InsertOAuth(QQDefaults.AuthenticationScheme, typeof(QQHandler));
                InsertOAuth(RedditAuthenticationDefaults.AuthenticationScheme, typeof(RedditAuthenticationHandler));
                InsertOAuth(SalesforceAuthenticationDefaults.AuthenticationScheme, typeof(SalesforceAuthenticationHandler));
                InsertOAuth(TwitterDefaults.AuthenticationScheme, typeof(TwitterHandler2));
                InsertOAuth(VisualStudioAuthenticationDefaults.AuthenticationScheme, typeof(VisualStudioAuthenticationHandler));
                InsertOAuth(WeiboDefaults.AuthenticationScheme, typeof(WeiboHandler));
                InsertOAuth(WeixinDefaults.AuthenticationScheme, typeof(WeixinHandler));
                InsertOAuth(WordPressAuthenticationDefaults.AuthenticationScheme, typeof(WordPressAuthenticationHandler));
            }

            static void InsertOAuth(string scheme, Type handlerType, string clientId = "1", string clientSecret = "1")
            {
                TenantProperties.Add($"{scheme}:ClientId", clientId);
                TenantProperties.Add($"{scheme}:ClientSecret", clientSecret);

                OAuthHandlers.Add(scheme, handlerType);
            }

            public static string IdentityServerIssuerUri = "localhost:44309";
            public static string AppHostName = "localhost:44309";
            public static string Name = "微服务";
            public static string WebSite = "http://localhost:44309";

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

            public static Dictionary<string, Type> OAuthHandlers = new Dictionary<string, Type>();
        }

        /// <summary>
        /// 初始Client(用来测试)
        /// </summary>
        public class TestClient
        {
            public const string ClientId = "test";
            public const string ClientName = "测试专用";
            public const string ClientSecret = "1";
            public static ICollection<string> AllowedGrantTypes = GrantTypes.CodeAndClientCredentials;
            public static List<string> RedirectUris = new List<string>()
            {
                "https://{0}/swagger/oauth2-redirect.html"
            };
        }

        /// <summary>
        /// 初始Client(后台使用)
        /// </summary>
        public class AdminPortalClient
        {
            public const string ClientId = "adminportal";

            public const string ClientName = "后台专用";

            public const string ClientSecret = "1";

            public static ICollection<string> AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials;

            public static ICollection<string> RedirectUris = new string[]{
                "http://localhost:4200/auth-callback",
                // for IdentityServer4.MicroService.UI
                "https://localhost:44386/callback.html",
                "https://localhost:44386/silent_callback.html",
            };

            public static ICollection<string> PostLogoutRedirectUris = new string[]{
                "http://localhost:4200/logout-callback",

                 // for IdentityServer4.MicroService.UI
                "http://localhost:44386/logout.html"
            };
        }

        public static IEnumerable<AppUser> GetUsers()
        {
            return new List<AppUser>
                {
                     new AppUser()
                     {
                        Email=Admin.Email,
                        UserName=Admin.Email,
                        PasswordHash=Admin.PasswordHash,
                        EmailConfirmed=true,
                        ParentUserID=AppConstant.seedUserId
                     }
                };
        }

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

        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
                {
                    #region Test Client
		            new Client
                    {
                        ClientId = TestClient.ClientId,
                        ClientName = TestClient.ClientName,
                        AllowedGrantTypes = TestClient.AllowedGrantTypes,
                        AllowAccessTokensViaBrowser = true,
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
                    },
	                #endregion

                    #region AdminPortal Client
		            new Client
                    {
                        ClientId = AdminPortalClient.ClientId,
                        ClientName = AdminPortalClient.ClientName,
                        AllowedGrantTypes = AdminPortalClient.AllowedGrantTypes,
                        AllowAccessTokensViaBrowser = true,
                        ClientSecrets =
                        {
                            new Secret(AdminPortalClient.ClientSecret.Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,

                        // 需要设置为true，否则token无法附加tenant相关信息
                        AlwaysSendClientClaims = true,
                        FrontChannelLogoutSessionRequired=false,
                        FrontChannelLogoutUri="",

                        RedirectUris = AdminPortalClient.RedirectUris,
                        PostLogoutRedirectUris = AdminPortalClient.PostLogoutRedirectUris,

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
                    new IdentityResources.OpenId(){  UserClaims={ "role", ClaimTypes.UserPermission } },
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone(),
                };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            var ControllerScopes = new List<Scope>();

            var ActionScopes = typeof(ClientScopes).GetFields().Select(x =>
            {
                var permissionValues =
                x.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues;

                var description =
                x.GetCustomAttribute<DescriptionAttribute>().Description;

                var _ControllerScope = permissionValues[1];

                if (!ControllerScopes.Any(scope => scope.Name.Equals(_ControllerScope)))
                {
                    var ControllerDescription = description.Split(new string[] { "-" },
                        StringSplitOptions.RemoveEmptyEntries)[0].Trim();

                    ControllerScopes.Add(new Scope(_ControllerScope, ControllerDescription + " - 所有权限"));
                }

                return new Scope(permissionValues[0], description);

            }).ToList();

            var ApplicationScope = new Scope(MicroServiceName + ".all", "所有权限");

            ActionScopes.AddRange(ControllerScopes);

            ActionScopes.Add(ApplicationScope);

            ActionScopes = ActionScopes.OrderBy(x => x.Name).ToList();

            return new List<ApiResource>
                {
                    new ApiResource()
                    {
                        Enabled =true,
                        ApiSecrets = {},
                        Name = MicroServiceName,
                        DisplayName = MicroServiceName,
                        Description = MicroServiceName,
                        Scopes = ActionScopes,
                        //需要使用的用户claims
                        UserClaims= {
                            ClaimTypes.UserPermission,
                            "role"
                        }
                    }
                };
        }

        /// <summary>
        /// 数据库初始化
        /// </summary>
        public static void InitializeDatabase(IApplicationBuilder app, IConfigurationRoot config)
        {
            Tenant.InitOAuthens();

            Tenant.AppHostName = config["IdentityServer"];

            Tenant.IdentityServerIssuerUri = Tenant.AppHostName;

            TestClient.RedirectUris[0] = string.Format(TestClient.RedirectUris[0], Tenant.AppHostName);

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                #region identityserver
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in GetClients())
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
                    foreach (var resource in GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                #endregion

                #region tenant
                var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                tenantDbContext.Database.Migrate();
                if (!tenantDbContext.Tenants.Any())
                {
                    #region Create Default Tenant
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
                    #endregion
                }
                #endregion

                var userContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                userContext.Database.Migrate();

                if (!userContext.Roles.Any())
                {
                    foreach (var role in GetRoles())
                    {
                        userContext.Roles.Add(role);
                    }
                    userContext.SaveChanges();
                }

                if (!userContext.Users.Any())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    var roleIds = userContext.Roles.Select(x => x.Id).ToList();

                    var permissions = typeof(UserPermissions).GetFields().Select(x => x.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues[0]).ToList();
                    permissions.Add(MicroServiceName + ".all");

                    var tenantIds = tenantDbContext.Tenants.Select(x => x.Id).ToList();

                    foreach (var _user in GetUsers())
                    {
                        var r = AppUserService.CreateUser(AppConstant.seedTenantId,
                             userManager,
                             userContext,
                             _user,
                             roleIds,
                            string.Join(",", permissions),
                            tenantIds).Result;

                        #region User Clients
                        var clientIds = context.Clients.Select(x => x.Id).ToList();
                        foreach (var cid in clientIds)
                        {
                            _user.Clients.Add(new AspNetUserClient()
                            {
                                ClientId = cid
                            });
                        }
                        #endregion

                        #region User ApiResources
                        var apiIds = context.ApiResources.Select(x => x.Id).ToList();
                        foreach (var apiId in apiIds)
                        {
                            _user.ApiResources.Add(new AspNetUserApiResource()
                            {
                                ApiResourceId = apiId,
                            });
                        }
                        #endregion

                        userContext.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 邮件发送模板
        /// 需要对接sendcloud平台
        /// https://sendcloud.sohu.com
        /// </summary>
        public enum SendCloudMailTemplates
        {
            #region 用户注册 - 邮箱验证
            /// <summary>
            /// 用户注册 - 邮箱验证
            /// 变量：%code%
            /// </summary>
            [EmailConfig("邮箱验证")]
            verify_email,
            #endregion

            #region 订阅微服务 - 邮箱验证
            /// <summary>
            /// 订阅微服务 - 邮箱验证
            /// 变量：
            /// %SubscritionUrl%
            /// %DelSubscritionUrl%
            /// %apiId%
            /// %serviceName%
            /// </summary>
            [EmailConfig("验证邮箱")]
            verify_apiresource_subscription,
            #endregion

            #region 忘记密码
            /// <summary>
            /// 忘记密码
            /// 变量：%callbackUrl%
            /// </summary>
            [EmailConfig("reset password")]
            reset_password,
            #endregion

            #region 用户登录 - 邮箱安全码
            /// <summary>
            /// 用户登录 - 邮箱安全码
            /// 变量：%code%
            /// </summary>
            [EmailConfig("登录验证码")]
            security_code,
            #endregion

            #region 用户注册 - 激活邮箱
            /// <summary>
            /// 用户注册 - 激活邮箱
            /// 变量：%name%,%url%
            /// </summary>
            [EmailConfig("%name%请激活您的邮箱")]
            test_template_active,
            #endregion
        }
    }
}
