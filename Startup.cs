using OAuthApp.Data;
using OAuthApp.Models;
using OAuthApp.Services;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace OAuthApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHostedService<ChannelWorker>();

            services.AddRazorPages()
             .AddRazorRuntimeCompilation()
             .AddNewtonsoftJson(options =>
             {
                 //设置时间格式
                 options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                 //忽略循环引用
                 options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                 //数据格式按原样输出
                 //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                 //忽略空值
                 //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
             });

            services.AddControllersWithViews()
             .AddSessionStateTempDataProvider();

            services.AddMemoryCache();

            services.AddSession();

            services.AddHttpContextAccessor();

            services.AddScoped<TenantProvider>();
            services.AddScoped<TokenProvider>();
            services.AddScoped<TenantDbCreator>();
            services.AddScoped<ITenantUser, TenantUser>();
            services.AddScoped<UploadService>();
            
            InitDatabase(services);

            InitApiVersioning(services);

            InitSwaggerGen(services);

            InitCors(services);

            InitAuthorization(services);

            InitAuthentication(services);

            InitTenantDB(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                #region Swagger
                app.UseSwagger(x =>
                {
                    x.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer> {
                        new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                    });

                });
                app.UseSwaggerUI(c =>
                {
                    var provider = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                    c.DocExpansion(DocExpansion.None);

                    c.EnableValidator();
                });
                #endregion
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("cors");

            app.UseHttpsRedirection();

            InitSubAppFileSetting(app,env);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<TenantMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        void InitTenantDB(IServiceCollection services)
        {
            if (!Directory.Exists(AppConst.TenantDBPath))
            {
                Directory.CreateDirectory(AppConst.TenantDBPath);
            }

            var dbContext = services.BuildServiceProvider()
                .GetRequiredService<TenantDbContext>();

            dbContext.Database.EnsureCreated();
        }
        
        void InitAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
                    new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());
            });
        }

        void InitAuthentication(IServiceCollection services)
        {
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            var jwtSettings = new JwtSettings();
            Configuration.Bind("JwtSettings", jwtSettings);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, AppConst.AssemblyName, options =>
                {
                    options.LoginPath = "/Home/SignIn/";
                    options.AccessDeniedPath = "/Home/SignIn/";
                    //options.Events.OnSigningIn = (x) =>
                    //{
                    //    var tenant = x.HttpContext.GetTenantContext();
                    //    if(tenant.Properties.ContainsKey(TenantDefaults.Properties.SSOSignInDomain))
                    //    {
                    //        x.CookieOptions.Domain =
                    //        tenant.Properties[TenantDefaults.Properties.SSOSignInDomain];
                    //    }
                    //    return Task.CompletedTask;
                    //};
                })
                //.AddJixiuH5Auth()
                //.AddWebHookAuth()
                .AddJwtBearer(options =>
                {
                    options.ClaimsIssuer = jwtSettings.Issuer;
                    options.Audience = jwtSettings.Audience;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                            ),

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        void InitDatabase(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(AppConst.TenantDBPath));
            services.AddScoped<AppDbContext>();

            services.AddDbContext<TenantDbContext>(options => options.UseSqlite(AppConst.TenantDBConnection));
            services.AddScoped<TenantDbContext>();

            services.AddDbContext<ApiDbContext>(options => options.UseSqlite(AppConst.TenantDBPath));
            services.AddScoped<ApiDbContext>();
        }

        void InitApiVersioning(IServiceCollection services)
        {
            #region ApiVersioning
            services.AddVersionedApiExplorer();
            services.AddApiVersioning(x => {
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
            });
            #endregion
        }

        void InitCors(IServiceCollection services)
        {
            #region Cors
            var Cors = Configuration["Cors"].Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //允许一个或多个来源可以跨域
            services.AddCors(options =>
            {
                options.AddPolicy("cors", policy =>
                {
                    policy.WithOrigins(Cors)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            #endregion
        }

        void InitSwaggerGen(IServiceCollection services)
        {
            #region SwaggerGen
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();

                // Add JWT Authentication
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    }
                );

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }});


                var provider = services.BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    var info = new OpenApiInfo
                    {
                        Title = AppConst.AssemblyName,
                        Version = description.ApiVersion.ToString(),
                        License = new OpenApiLicense()
                        {
                            Name = "MIT",
                            Url = new Uri("https://spdx.org/licenses/MIT.html")
                        }
                    };

                    c.SwaggerDoc(description.GroupName, info);
                }

                var SiteSwaggerFilePath = Path.Combine(
                    PlatformServices.Default.Application.ApplicationBasePath,
                    AppConst.AssemblyName + ".xml");

                if (File.Exists(SiteSwaggerFilePath))
                {
                    c.IncludeXmlComments(SiteSwaggerFilePath);
                }
            });
            #endregion
        }

        /// 设置二级目录未SPA服务器模式
        void InitSubAppFileSetting(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var directoryInfo = new DirectoryInfo(Path.Combine(env.WebRootPath, "tenant"));

            app.MapWhen(context => context.Request.Path.StartsWithSegments($"/{directoryInfo.Name}"), _appBuilder =>
            {
                _appBuilder.UseSpa(spa =>
                {
                    spa.Options.SourcePath = $"/{directoryInfo.Name}";
                    spa.Options.DefaultPage = $"/{directoryInfo.Name}/index.html";
                });

                app.UseSpaStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider($"{directoryInfo.FullName}"),
                    RequestPath = $"/{directoryInfo.Name}"
                });
            });
        }
    }
}
