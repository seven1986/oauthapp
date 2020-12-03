# 多租户模式

!!! note ""

    OAuthApp会根据当前请求的域名自动解析租户信息，并附加到请求上下文中。

## 配置说明

### 公网环境

!!! note "操作步骤"

    - 1，部署OAuthApp程序到具有独立IP的云服务器
    - 2，将1个备案过的域名做泛解析到云服务器（例如：`*.oauthapp.com` CNAME 到 `服务器IP`）
    - 3，上传支持二级域名通配的SSL域名证书到服务器，确保通过域名访问到网站都是HTTPS协议

### 本地环境

!!! note "操作步骤"

    - 1，本地运行OAuthApp程序，并记录访问地址（比如：127.0.0.1:44391）
    - 2，修改`hosts`文件，添加若干二级域名映射到本地OAuthApp的服务器地址（比如：127.0.0.1:44391）

## 租户管理

!!! note ""

    租户数据库 `TenantDbContext` 负责维护二级域名与租户实体的关系，可参考如下代码进行管理。

=== "TenantController.cs"

    ``` csharp linenums="1"
    public class TenantController : ApiControllerBase
    {
        private readonly TenantDbContext _tenantDb;

        public TenantController(TenantDbContext tenantDb)
        {
            _tenantDb=tenantDb;
        }

        // 获取租户
        public List<AppTenant> Get()
        {
            return tenantDb.Tenants.ToList();
        }

        // 创建租户
         public bool Post(AppTenant value)
        {
            tenantDb.Tenants.Add(value);
            tenantDb.SaveChanges();
            return true;
        }

        // 更新租户
         public bool Put(AppTenant value)
        {
            tenantDb.Attach(value).State = EntityState.Modified;
            tenantDb.SaveChanges();
            return true;
        }

        // 删除租户
         public bool Delete(long id)
        {
            var item = tenantDb.Tenants.Find(id);

            if(item!=null)
            {
                tenantDb.Tenants.Remove(item);
                tenantDb.SaveChanges();
            }
            return true;
        }
    }
    ```

## 开始使用

!!! note ""

    在请求中获取当前租户信息并显示出来。

### 在控制器使用

=== "Controllers/HomeController.cs"

    ``` csharp linenums="1"
    public class HomeController : ApiControllerBase
    {
        public IActionResult Index()
        {
            // Tenant_Claims 用于客户端等非机密的场景，Claim可存储对外开放的数据
            ViewBag.Tenant_Claims = Tenant.TenantValidatorHelper.GetTenantWithClaims(Request.HttpContext);
            
            // Tenant_Properties 用于服务端场景，Property可存储密钥数据
            ViewBag.Tenant_Properties = Tenant.TenantValidatorHelper.GetTenantWithProperties(Request.HttpContext);

            return View();
        }
    }
    ```

### 在视图中使用

=== "Views/Home/Index.cshtml"

    ``` html linenums="1"
    @{
        var TenantContext = OAuthApp.Tenant.TenantValidatorHelper.GetTenantWithProperties(Context);
     }

    <p>@TenantContext.LogoUri</p>
    <p>@TenantContext.Name</p>
    <p>@TenantContext.IdentityServerIssuerUri</p>
    <p>@TenantContext.OwnerUserId</p>
    <p>@TenantContext.Description</p>
    <p>@TenantContext.Id</p>

    @{
        foreach(var item in TenantContext.Properties)
        {
            <p>
               <b>@item.Key</b> :<span>@item.Value</span>
            </p>
        }
    }
    ```