# 自定义验证器

!!! note ""
    自定义验证器，DemoGrantValidator演示了如何验证userName和password，然后颁发accessToken的过程。


=== "Startup.cs"
    ``` csharp linenums="1"
    public void ConfigureServices(IServiceCollection services)
        {
           services.AddOAuthApp(x =>
            {
                x.IdentityServerBuilder = builder =>
                {
                    builder.AddExtensionGrantValidator<DemoGrantValidator>();
                };
            }
        }
    ```
=== "DemoGrantValidator.cs"
    ``` csharp linenums="1"
    public class DemoGrantValidator: IExtensionGrantValidator
    {
        public string GrantType => "demo";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var username = context.Request.Raw.Get("username");

            var password = context.Request.Raw.Get("password");

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var extraInfo = new Dictionary<string, object>();
                extraInfo.Add("country", "china");
                extraInfo.Add("hobby", "coding,cooking,running");

              context.Result = new GrantValidationResult(
                subject: "uid_1",
                authenticationMethod: GrantType, customResponse: extraInfo);

            return;
        }
    }
    ```
  
