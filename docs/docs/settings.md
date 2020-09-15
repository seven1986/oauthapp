# 配置说明

## 默认管理员

=== "Startup.cs"
``` csharp
public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityServer4MicroService(options=>
        {
            //默认：admin@admin.com
            options.DefaultUserAccount = "admin@admin.com"; 
            //默认为：123456aA!
            options.DefaultUserPassword = "123456aA!"; 
        });
    }
```


## 显示/隐藏API文档

!!! note "提示"
    如果需要在项目中隐藏OAuthApp的接口文档，可参考如下配置。
=== "Startup.cs"
``` csharp
public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityServer4MicroService(options=>
        {
            // 这只该属性会隐藏所有文档
            options.EnableAPIDocuments=false;

            // 或使用如下方法展示指定的API文档
            // IdentityServer4MicroServiceOptions.APIDocuments.Add
        });
    }
``` 



## 版本号

!!! note "提示"
    为api添加版本功能，在请求中实现如下方式：

    - /api/foo?api-version=1.0

    - /api/foo?api-version=2.0-Alpha

    - /api/foo?api-version=2015-05-01.3.0

    - /api/v1/foo

    - /api/v2.0-Alpha/foo

    - /api/v2015-05-01.3.0/foo

    更多可以[参考文档](https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format)


=== "Startup.cs"
    ``` csharp
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer4MicroService(options=>
            {
                // 默认为true
                options.EnableApiVersioning=true;
            });
        }
    ``` 
=== "DemoController.cs"
    ``` csharp
    [ApiVersion("2.0")]
    [ApiController]
    public class DemoController : ControllerBase
    {

    }
    ```


## 跨域


!!! note "提示" 
    允许跨域的url集合(默认读取配置文件的IdentityServer:Origins节点)，多个网址有英文逗号分隔。

=== "Startup.cs"
    ``` csharp
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer4MicroService(options=>
            {
                // 默认为true
                options.EnableCors=true;
            });
        }
    ``` 
=== "appsettings.json"
    ``` Javascript
    IdentityServer: {
        Origins: "http://127.0.0.1:8080,http://127.0.0.1:8888"
    }
    ```

## Aspnet Core Identity

!!! note "提示"
    具体配置项，可以[参考文档](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration)官方文档。

=== "Startup.cs"
``` csharp
public void ConfigureServices(IServiceCollection services)
    {
        services.AddIdentityServer4MicroService(options=>{
            
            options.AspNetCoreIdentityOptions = identityOptions =>{

                    identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                    identityOptions.Lockout.AllowedForNewUsers = true;

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
                    identityOptions.SignIn.RequireConfirmedPhoneNumber = false;

                    // Default User settings.
                    identityOptions.User.AllowedUserNameCharacters =
                            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    identityOptions.User.RequireUniqueEmail = true;
            }
        });
    }
```

## Identity Server 4

!!! note "提示"
    具体配置，可以参考[官方文档](https://identityserver4.readthedocs.io/en/latest/reference/options.html)

=== "Startup.cs"
    ``` csharp
    public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer4MicroService(options=>
            {
                // 通过下面的对象配置identityserver4
                // options.IdentityServerBuilder
                // options.IdentityServerOptions
            });
        }
    ``` 