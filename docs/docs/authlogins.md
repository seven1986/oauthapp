# 对接第三方OAuth应用

!!! note ""

    可以为每个租户的站点启用第三方登录功能，目前支持的平台如下

| 平台      | 属性名                          |
| ----------- | ------------------------------------ |
| `微信` | Weixin: ClientId、Weixin: ClientSecret |
| `微博` | Weibo: ClientId、Weibo: ClientSecret |
| `QQ` | QQ: ClientId、QQ: ClientSecret |
| `MicrosoftAccount` | Microsoft: ClientId、Microsoft: ClientSecret |
| `Google` | Google: ClientId、Google: ClientSecret |
| `Facebook` | Facebook: ClientId、Facebook: ClientSecret |
| `GitHub` | GitHub: ClientId、GitHub: ClientSecret |

## 使用说明

### 配置微信登录

!!! note ""

    参考如下代码，为租户添加微信登录配置

=== "TenantController.cs"

    ``` csharp linenums="1"
    public class TenantController : ApiControllerBase
    {
        private readonly TenantDbContext _tenantDb;

        public TenantController(TenantDbContext tenantDb)
        {
            _tenantDb=tenantDb;

            var tenantItem = new AppTenant()
            {
                Name = "OAuthapp",
                Description = "Test"
            };

            tenantItem.Properties.Add(new AppTenantProperty()
            {
                Key = "Weixin:ClientId",
                Value = "11111"
            });

            tenantItem.Properties.Add(new AppTenantProperty()
            {
                Key = "Weixin:ClientSecret",
                Value = "11111"
            });

            _tenantDb.Tenants.Add(tenantItem);
             tenantDb.SaveChanges();
        }
    }
    ``` 

### 在前端页面使用

!!! note ""

    参考如下代码，在界面中展示微信登陆

=== "Controllers/LoginController.cs"

    ``` csharp linenums="1"
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
            
        public LoginController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

         [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm =  (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult Post(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Page("./Post", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
    }
    ```

=== "Views/Login/Index.cshtml"

    ``` csharp linenums="1"
    @{
            <form  method="post">
            @foreach (var provider in Model.ExternalLogins)
            {
                <button type="submit" name="provider" value="@provider.Name">@provider.DisplayName</button>
            }
            </form>
    }
    ```