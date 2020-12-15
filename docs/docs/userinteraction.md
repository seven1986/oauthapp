# 自定义UI交互界面

!!! note ""

    用户登陆的方式分为3种：

    - 1，**系统内置登陆** ：用账号/邮箱/手机号登陆自己的系统。

    - 2，**外部OAuth应用登陆自己的系统** ：使用微信/微博/QQ/Github账号登录自己的系统。

    - 3，**自身系统提供登陆服务给外部** ：创建OAuth应用供第三方使用，把自己系统的用户与外部系统的用户链接起来。

    通过这3种方式，系统与系统之间的用户数据也就联系在一起了。


## 单点登陆UI交互
当我们的系统对外提供单点登录服务时，可自定义UI交互界面，如：登陆页、退出页，用户授权页，授权管理页。

!!! note ""
    https://localhost:4200 是自定义的前端页面地址，与服务器交互时，会自动跳转到指定的页面，前端页面结合[Authing控制器](https://www.oauthapp.com/docs/index.html#tag/Authing)即可实现验证流程。

=== "Startup.cs"

    ``` csharp linenums="1"
     public void ConfigureServices(IServiceCollection services)
     {
          services.AddOAuthApp(x =>
            {
                x.IdentityServerOptions = options =>
                {
                    options.UserInteraction.LoginUrl = "https://localhost:4200/sso/signin";
                    options.UserInteraction.DeviceVerificationUrl = "https://localhost:4200/sso/signin-device";
                    options.UserInteraction.LogoutUrl = "https://localhost:4200/sso/signout";
                    options.UserInteraction.ConsentUrl = "https://localhost:4200/sso/consent";
                    options.UserInteraction.ErrorUrl = "https://localhost:4200/sso/error";
                };
            });
     }
    ```



## 多租户单点登陆UI交互

!!! note ""
    多租户模式下，假设有3个站点分别提供单点登录服务，又需要不同的UI交互界面，进行以下配置即可

    - https://app1.oauthapp.com

    - https://app2.oauthapp.com

    - https://app3.oauthapp.com


=== "Controllers/HomeController.cs"

    ``` csharp linenums="1"

    public class TenantController : ApiControllerBase
    {
    private readonly TenantDbContext _tenantDb;

    public TenantController(TenantDbContext tenantDb)
    {
        _tenantDb=tenantDb;

        var app1 = new AppTenant()
        {
            Name = "app1",
            IdentityServerIssuerUri = "app1.oauthapp.com"
        };
        app1.Hosts.Add(new AppTenantHost() { HostName = "app1.oauthapp.com" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LoginUrl, Value = "https://localhost:4100/sso/signin" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LoginReturnUrlParameter, Value = "app1" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LogoutUrl, Value = "https://localhost:4100/sso/signout" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LogoutIdParameter, Value = "app1" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ConsentUrl, Value = "https://localhost:4100/sso/consent" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ConsentReturnUrlParameter, Value = "app1" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ErrorUrl, Value = "https://localhost:4100/sso/error" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ErrorIdParameter, Value = "app1" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.CustomRedirectReturnUrlParameter, Value = "app1" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.CookieMessageThreshold, Value = "2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.DeviceVerificationUrl, Value = "https://localhost:4100/sso/signin-device" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.DeviceVerificationUserCodeParameter, Value = "app1" });
        tenantDb.Tenants.Add(app1);

        var app2 = new AppTenant()
        {
            Name = "app2",
            IdentityServerIssuerUri = "app2.oauthapp.com"
        };
        app1.Hosts.Add(new AppTenantHost() { HostName = "app2.oauthapp.com" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LoginUrl, Value = "https://localhost:4300/sso/signin" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LoginReturnUrlParameter, Value = "app2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LogoutUrl, Value = "https://localhost:4300/sso/signout" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.LogoutIdParameter, Value = "app2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ConsentUrl, Value = "https://localhost:4300/sso/consent" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ConsentReturnUrlParameter, Value = "app2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ErrorUrl, Value = "https://localhost:4300/sso/error" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.ErrorIdParameter, Value = "app2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.CustomRedirectReturnUrlParameter, Value = "app2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.CookieMessageThreshold, Value = "2" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.DeviceVerificationUrl, Value = "https://localhost:4300/sso/signin-device" });
        app1.Properties.Add(new AppTenantProperty() { Key = UserInteractionKeys.DeviceVerificationUserCodeParameter, Value = "app2" });
        tenantDb.Tenants.Add(app2);

        // app3 略

        tenantDb.SaveChanges();
    }
    }
    ```