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
    启用ReDoc后，可为当前项目生成接口文档。
=== "Startup.cs"
``` csharp linenums="1"
public void ConfigureServices(IServiceCollection services)
    {
        services.AddOAuthApp(options=>
        {

            // 生成接口文档（默认为true）
            options.EnableReDoc = true;
            
            // 文档配置
            x.ReDocOptions = options =>
            {
                options.DocumentTitle = "OAuthApp";
            };

            // 文档个性化
            // 配置项参考 https://github.com/Redocly/redoc/blob/master/docs/redoc-vendor-extensions.md
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



## 调试工具

!!! note ""
    启用Swagger后，可更方便开发人员调试接口。
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
    为api添加版本功能，在请求中实现如下方式：

    * [x] /api/foo?api-version=1.0
    * [x] /api/foo?api-version=2.0-Alpha
    * [x] /api/foo?api-version=2015-05-01.3.0
    * [x] /api/v1/foo
    * [x] /api/v2.0-Alpha/foo
    * [x] /api/v2015-05-01.3.0/foo

    更多可以[参考文档](https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format)


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
    允许跨域的url集合(默认读取配置文件的IdentityServer:Origins节点)，多个网址有英文逗号分隔。

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
    具体相关配置，可以参考[官方文档](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration)

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
    具体相关配置，可以参考[官方文档](https://identityserver4.readthedocs.io/en/latest/reference/options.html)

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