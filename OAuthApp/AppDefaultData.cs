using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using OAuthApp.Data;
using OAuthApp.Tenant;
using OAuthApp.CacheKeys;
using OAuthApp.Services;
using IdentityServer4;

namespace OAuthApp
{
    public class AppDefaultData
    {
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

        #region OAuthAppClient
        /// <summary>
        /// OAuthAppClient - 后台使用
        /// </summary>
        public class OAuthAppClient
        {
            public static string ClientId = "oauthapp";

            public static string ClientName = "oauthapp";

            public static string ClientSecret = "oauthapp";

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

            public static string Name = AppConstant.MicroServiceName;

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

        public static IEnumerable<Client> GetClients(Uri ServerUrl)
        {
            // client credentials client
            var additionGrantTypes = new List<string>() { "mobile_code", "openid_oauth" };

            additionGrantTypes.ForEach(_grantType =>
            {
                SwaggerClient.AllowedGrantTypes.Add(_grantType);
                OAuthAppClient.AllowedGrantTypes.Add(_grantType);
            });

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
                            IdentityServerConstants.StandardScopes.Address,
                            IdentityServerConstants.StandardScopes.Email,
                            IdentityServerConstants.StandardScopes.Phone,
                            IdentityServerConstants.StandardScopes.OfflineAccess,
                            "role",
                            "permission",
                            AppConstant.MicroServiceName + ".all"
                        },
                        AllowOfflineAccess = true,
                        RequirePkce=false
                    },
	                #endregion

                    #region OAuthApp
		            new Client
                    {
                        ClientId = OAuthAppClient.ClientId,
                        ClientName = OAuthAppClient.ClientName,
                        AllowedGrantTypes = OAuthAppClient.AllowedGrantTypes,
                        AllowAccessTokensViaBrowser = true,
                        ClientSecrets =
                        {
                            new Secret(OAuthAppClient.ClientSecret.Sha256())
                        },
                        BackChannelLogoutSessionRequired=false,
                        BackChannelLogoutUri="",
                        ConsentLifetime=969000,
                        RequireConsent=true,

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
                            IdentityServerConstants.StandardScopes.Address,
                            IdentityServerConstants.StandardScopes.Email,
                            IdentityServerConstants.StandardScopes.Phone,
                            IdentityServerConstants.StandardScopes.OfflineAccess,
                            "role",
                            "permission",
                            AppConstant.MicroServiceName + ".all"
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
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                    new IdentityResources.Address(),
                    new IdentityResources.Email(),
                    new IdentityResources.Phone(),
                    new IdentityResource("role","Your user role",new List<string>(){ "role"}),
                    new IdentityResource("permission","Your user permission",new List<string>(){ "permission"})
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
                {
                    new ApiResource()
                    {
                        Enabled = true,

                        ApiSecrets = {},

                        Name = AppConstant.MicroServiceName,

                        DisplayName = AppConstant.MicroServiceName,

                        Description = AppConstant.MicroServiceName,

                        Scopes = new List<string>()
                        {
                            $"{AppConstant.MicroServiceName}.all"
                        },
                         
                        Properties =new Dictionary<string,string>()
                    }
                };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
                {
                    new ApiScope()
                    {
                        Enabled = true,

                        Name =  $"{AppConstant.MicroServiceName}.all",

                        DisplayName = AppConstant.MicroServiceName,

                        Description = AppConstant.MicroServiceName,

                         UserClaims=new List<string>()
                         {
                             "role","permission"
                         },

                        Properties =new Dictionary<string,string>()
                    }
                };
        }

        /// <summary>
        /// 数据库初始化
        /// </summary>
        public static void InitializeDatabase(IApplicationBuilder app, OAuthAppOptions options)
        {
            Tenant.AppHostName = Tenant.IdentityServerIssuerUri = options.IdentityServerUri.Authority;

            Tenant.WebSite = options.IdentityServerUri.OriginalString;

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                #region PersistedGrantDb
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                #endregion

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in GetClients(options.IdentityServerUri))
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

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in GetApiScopes())
                    {
                        context.ApiScopes.Add(resource.ToEntity());
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
                Data_Seeding_Users(userContext, tenantDbContext, userManager, context, options);
                #endregion

                #region SDKDb
                var SDKDbContext = scope.ServiceProvider.GetRequiredService<SdkDbContext>();
                SDKDbContext.Database.Migrate();
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

                tenant.Properties.AddRange(Tenant.TenantProperties.Select(x => new AppTenantProperty()
                {
                    Key = x.Key,
                    Value = x.Value
                }));

                tenantDbContext.Tenants.Add(tenant);
                tenantDbContext.SaveChanges();
            }
        }

        static void Data_Seeding_Users(
            UserDbContext userContext, 
            TenantDbContext tenantDbContext,
            UserManager<AppUser> userManager,
            ConfigurationDbContext identityserverDbContext,
            OAuthAppOptions options)
        {
            if (!userContext.Roles.Any())
            {
                var roles = typeof(DefaultRoles).GetFields();

                foreach (var role in roles)
                {
                    var roleName = role.GetRawConstantValue().ToString();

                    userContext.Roles.Add(new AppRole
                    {
                        Name = roleName,
                        NormalizedName = roleName,
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
                    Email = options.DefaultUserAccount,
                    UserName = options.DefaultUserAccount,
                    PasswordHash = options.DefaultUserPassword,
                    EmailConfirmed = true,
                    ParentUserID = AppConstant.seedUserId
                };

                var r = AppUserService.CreateUser(AppConstant.seedTenantId,
                     userManager,
                     userContext,
                     user,
                     roleIds,
                    $"{AppConstant.MicroServiceName}.all",
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
    }
}
