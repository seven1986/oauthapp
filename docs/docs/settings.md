# OAuthApp配置

## 默认管理员

=== "Startup.cs"
``` csharp linenums="1"
public void ConfigureServices(IServiceCollection services)
    {
        services.AddOAuthApp(options=>
        {
            //默认：admin@admin.com
            options.DefaultUserAccount = "admin@admin.com"; 
            //默认为：123456aA!
            options.DefaultUserPassword = "123456aA!"; 
        });
    }
```


## 接口文档

!!! note ""
    设置 **EnableReDoc=true** 后，可为当前项目生成接口文档。 [ReDocExtensions](http://www.oauthapp.com/doc-extensions)
=== "Startup.cs"
``` csharp linenums="1"
public void ConfigureServices(IServiceCollection services)
    {
        services.AddOAuthApp(options=>
        {

            // 生成接口文档（默认为true）
            options.EnableReDoc = true;
            
            // 文档配置
            options.ReDocOptions = _options =>
            {
                _options.DocumentTitle = "OAuthApp";
                _options.RoutePrefix = "docs";
                _options.SpecUrl("/swagger/v1/swagger.json");
                _options.EnableUntrustedSpec();
                _options.ScrollYOffset(10);
                _options.HideHostname();
                _options.HideDownloadButton();
                _options.ExpandResponses("200,201");
                _options.RequiredPropsFirst();
                _options.HideLoading();                  
                _options.DisableSearch();
                _options.SortPropsAlphabetically();
                _options.OnlyRequiredInSamples();
                _options.NoAutoAuth();
                _options.PathInMiddlePanel();
                _options.NativeScrollbars();
            };

            // 文档个性化
            options.ReDocExtensions = (Extensions =>
                  {
                      Extensions.Add("x-logo", new OpenApiObject
                      {
                          ["url"] = new OpenApiString("https://www.oauthapp.com/images/oauthapp.png"),
                          ["backgroundColor"] = new OpenApiString("#ffffff"),
                          ["altText"] = new OpenApiString("OAuthApp.com"),
                      });
                  });

        });
    }
``` 

## 访问限流

!!! note ""
    设置 **EnableClientRateLimit=true、EnableIpRateLimit=true** 后，可控制`ClientIP`、`ClientID`的访问频率。

=== "Startup.cs"
    ``` csharp linenums="1"
    ublic void ConfigureServices(IServiceCollection services)
       {
           services.AddOAuthApp(options=>
           {
               // 启用Client限流（默认为false）
               options.EnableClientRateLimit = true;
    
               // 启用Ip限流（默认为false）
               options.EnableIpRateLimit = true;
           });
       }
    ```
=== "appsettings.json(Ip限流)"
    ``` javascript linenums="1"
    "IpRateLimiting": {
       "EnableEndpointRateLimiting": false,
       "StackBlockedRequests": false,
       "RealIpHeader": "X-Real-IP",
       "ClientIdHeader": "X-ClientId",
       "HttpStatusCode": 429,
       "IpWhitelist": [ "127.0.0.1", "::1/10", "192.168.0.0/24" ],
       "EndpointWhitelist": [ "get:/api/license", "*:/api/status" ],
       "ClientWhitelist": [ "dev-id-1", "dev-id-2" ],
       "GeneralRules": [
         {
           "Endpoint": "*",
           "Period": "1s",
           "Limit": 2
         },
         {
           "Endpoint": "*",
           "Period": "15m",
           "Limit": 100
         },
         {
           "Endpoint": "*",
           "Period": "12h",
           "Limit": 1000
         },
         {
           "Endpoint": "*",
           "Period": "7d",
           "Limit": 10000
         }
       ]
     }
    ```
=== "appsettings.json(Client限流)"
    ``` javascript linenums="1"
    "ClientRateLimiting": {
    "EnableEndpointRateLimiting": false,
    "StackBlockedRequests": false,
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "EndpointWhitelist": [ "get:/api/license", "*:/api/status" ],
    "ClientWhitelist": [ "dev-id-1", "dev-id-2" ],
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1s",
        "Limit": 2
      },
      {
        "Endpoint": "*",
        "Period": "15m",
        "Limit": 100
      },
      {
        "Endpoint": "*",
        "Period": "12h",
        "Limit": 1000
      },
      {
        "Endpoint": "*",
        "Period": "7d",
        "Limit": 10000
      }
    ]
    }
    ```




## 调试工具

!!! note ""
    设置 **EnableSwaggerGen=true、EnableSwaggerUI=true、EnableAPIDocuments=true** ，可方便开发人员调试接口，也可通过 **OAuthAppOptions.APIDocuments.Add** 隐藏指定文档

=== "Startup.cs"
``` csharp linenums="1"
public void ConfigureServices(IServiceCollection services)
    {
        services.AddOAuthApp(options=>
        {
            //生成swagger.json文档（默认为true）
            options.EnableSwaggerGen = true;
            
            //启用swaggerUI（默认为true），EnableSwaggerGen必须为true
            options.EnableSwaggerUI = true;

            //在swaggerUI中隐藏OAuthApp的API文档（默认为true），当前项目的API仍会展示
            options.EnableAPIDocuments=true;

            //在swaggerUI中隐藏指定OAuthApp的API文档
            //OAuthAppOptions.APIDocuments.Add
        });
    }
``` 






## 版本号

!!! note ""
    设置 **EnableApiVersioning=true** 后，可启用版本功能。例如：

    * [x] /api/foo?api-version=1.0
    * [x] /api/foo?api-version=2.0-Alpha
    * [x] /api/foo?api-version=2015-05-01.3.0
    * [x] /api/v1/foo
    * [x] /api/v2.0-Alpha/foo
    * [x] /api/v2015-05-01.3.0/foo

    [aspnet-api-versioning](https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format)


=== "Startup.cs"
    ``` csharp linenums="1"
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddOAuthApp(options=>
            {
                // 默认为true
                options.EnableApiVersioning=true;
            });
        }
    ``` 
=== "DemoController.cs"
    ``` csharp linenums="1"
    [ApiVersion("2.0")]
    [ApiController]
    public class DemoController : ControllerBase
    {

    }
    ```


## 跨域

!!! note "" 
    设置 **EnableCors=true** 后，可以启用跨域功能。

=== "Startup.cs"
    ``` csharp linenums="1"
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddOAuthApp(options=>
            {
                // 默认为true
                options.EnableCors=true;
            });
        }
    ``` 
=== "appsettings.json"
    ``` Javascript linenums="1"
    IdentityServer: {
        Origins: "http://127.0.0.1:8080,http://127.0.0.1:8888"
    }
    ```

## Aspnet Core Identity

!!! note ""
    相关配置可参考[Configure ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration)

=== "Startup.cs"
``` csharp linenums="1"
public void ConfigureServices(IServiceCollection services)
    {
        services.AddOAuthApp(options=>{
            
            options.AspNetCoreIdentityOptions = identityOptions =>{

            identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            identityOptions.Lockout.MaxFailedAccessAttempts = 5;
            identityOptions.Lockout.AllowedForNewUsers = true
            // Default Password settings.
            // 需要介于 0-9 的密码
            identityOptions.Password.RequireDigit = false; 
            // 要求密码中的小写字符
            identityOptions.Password.RequireLowercase = false; 
            // 需要在密码中的非字母数字字符
            identityOptions.Password.RequireNonAlphanumeric = false; 
            // 需要大写字符的密码
            identityOptions.Password.RequireUppercase = false;  
            // 密码最小长度
            identityOptions.Password.RequiredLength = 6; 
            // 要求在密码中非重复字符数
            identityOptions.Password.RequiredUniqueChars = 1;  
            // Default SignIn settings.
            identityOptions.SignIn.RequireConfirmedEmail = false;
            identityOptions.SignIn.RequireConfirmedPhoneNumber = false
            // Default User settings.
            identityOptions.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            identityOptions.User.RequireUniqueEmail = true;
            }
        });
    }
```

## Identity Server 4

!!! note ""
    相关配置可参考[IdentityServer Options](https://identityserver4.readthedocs.io/en/latest/reference/options.html)

=== "Startup.cs"
    ``` csharp linenums="1"
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddOAuthApp(options=>
            {
                // 通过下面的对象配置identityserver4
                // options.IdentityServerBuilder
                // options.IdentityServerOptions
            });
        }
    ``` 