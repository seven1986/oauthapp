# 配置短信、邮件服务

!!! note ""
    当用户注册成功、找回密码、验证手机号时，可以使用 [SendCloud服务](https://sendcloud.sohu.com) 并把对应的连接字符串复制到如下配置中。

=== "appsettings.json"
    ``` json linenums="1"
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
    ``` csharp linenums="1"
      public void ConfigureServices(IServiceCollection services)
          {
            services.AddSingleton<IEmailSender, EmailSender>();
          }
    ```
=== "EmailSender.cs"
    ``` csharp linenums="1"
    public class EmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
      {
          EmailService _sender;
  
          public EmailSender(EmailService sender)
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