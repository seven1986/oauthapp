### Multi tenant for Microsoft.AspNetCore.Authentication

#### 每次会刷新Tenant的ClientId、ClientSecret
```
Facebook、Google、MicrosoftAccount、Twitter 拷贝官方代码，替换继承类为TenantOAuthHandler
```

更多第三方登录代码可以参考：

https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers


1，百度、支付宝、微信公众号
2，联合登录后传递token跳转