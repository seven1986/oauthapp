# 使用短信、邮件服务

!!! note "提示"
    当用户注册成功、找回密码、验证手机号时，可以使用[sendcloud服务](https://sendcloud.sohu.com)，并把对应的连接字符串复制到如下配置中。

=== "appsettings.json"
    ``` json
      "IdentityServer": {
        "SMS": {
          "apiUser": "",
          "apiKey": ""
        },
        "Email": {
          "apiUser": "",
          "apiKey": "",
          "fromEmail": "",
          "fromName": ""
        }
    }
    ```


## 配置邮件服务

=== "Startup.cs"
  ``` csharp
  public void ConfigureServices(IServiceCollection services)
        {
          services.AddSingleton<IEmailSender, EmailSender>();
        }
  ```
=== "EmailSender.cs"
  ``` csharp
    public class EmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        IdentityServer4.MicroService.Services.EmailService _sender;

        public EmailSender(IdentityServer4.MicroService.Services.EmailService sender)
        {
            _sender = sender;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return _sender.SendEmailAsync(
                 "邮件模板的key",
                 subject,
                 new string[1] { email },
                 new Dictionary<string, string[]>()
                 {
                    { "%subject%",new string[1]{ subject } },
                    { "%message%",new string[1]{ message } }
                 });
        }
    }
  ```
  