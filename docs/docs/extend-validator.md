# 使用现有用户颁发accessToken

!!! note "提示"
    自定义验证器。


=== "Startup.cs"
    ``` csharp
    public void ConfigureServices(IServiceCollection services)
        {
           services.AddIdentityServer4MicroService(x =>
            {
                x.IdentityServerBuilder = builder =>
                {
                    builder.AddExtensionGrantValidator<DemoGrantValidator>();
                };
            }
        }
    ```
=== "DemoGrantValidator.cs"
    ``` csharp
    public class DemoGrantValidator: IExtensionGrantValidator
    {
        public string GrantType => "demo";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");

            var input_password = context.Request.Raw.Get("password");

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(input_password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var extraInfo = new Dictionary<string, object>();
                extraInfo.Add("country", "china");
                extraInfo.Add("hobby", "reading");

              context.Result = new GrantValidationResult(
                subject: "用户ID1",
                authenticationMethod: GrantType, customResponse: extraInfo);

            return;
        }
    }
    ```
  
