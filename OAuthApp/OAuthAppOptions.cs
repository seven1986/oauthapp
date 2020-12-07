using System;
using System.Collections.Generic;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Interfaces;
using Swashbuckle.AspNetCore.ReDoc;

namespace OAuthApp
{
    public class OAuthAppOptions
    {
        /// <summary>
        /// 管理员账号，邮箱格式(默认admin@admin.com)
        /// </summary>
        public string DefaultUserAccount { get; set; } = "admin@admin.com";

        /// <summary>
        /// 管理员密码（默认123456aA!）
        /// </summary>
        public string DefaultUserPassword { get; set; } = "123456aA!";

        /// <summary>
        /// 当前项目的网址(默认读取配置文件的IdentityServer:Host节点)
        /// 如：https://127.0.0.1（必须为https，网址结尾无需带/）
        /// </summary>
        public Uri IdentityServerUri { get; set; }

        /// <summary>
        /// 启用权限（默认true）
        /// </summary>
        internal bool EnableAuthorizationPolicy { get; set; } = true;

        /// <summary>
        /// 启用SwaggerGen（默认true）。
        /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-3.1
        /// </summary>
        public bool EnableSwaggerGen { get; set; } = true;

        /// <summary>
        /// 启用IdentityServer4Microservice的API文档（默认true）。
        /// 可通过OAuthAppOptions.APIDocuments配置需要屏蔽的接口文档
        /// </summary>
        public bool EnableAPIDocuments
        {
            get
            {
                return !APIDocuments.Contains(APIDocumentEnums.ALL);
            }
            set
            {
                if(value==true)
                {
                    APIDocuments.Clear();
                }
                else
                {
                    HideIdentityServerDocument(APIDocumentEnums.ALL);
                }
            }
        }

        public static List<APIDocumentEnums> APIDocuments { get; set; } = new List<APIDocumentEnums>();

        /// <summary>
        /// 启用SwaggerUI（默认true）。
        /// https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-3.1
        /// </summary>
        public bool EnableSwaggerUI { get; set; } = true;

        /// <summary>
        /// 启用跨域（默认true）
        /// </summary>
        public bool EnableCors { get; set; } = true;

        /// <summary>
        /// 启用多语言（默认true）。
        /// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-3.1
        /// </summary>
        public bool EnableLocalization { get; set; } = true;

        /// <summary>
        /// 启用API版本号功能（默认true）。
        /// https://github.com/Microsoft/aspnet-api-versioning/wiki/How-to-Version-Your-Service
        /// </summary>
        public bool EnableApiVersioning { get; set; } = true;

        /// <summary>
        /// 启用WebEncoders（默认true）。
        /// 将 HtmlEncoder、JavaScriptEncoder和UrlEncoder添加到services中。
        /// https://docs.microsoft.com/zh-cn/dotnet/api/microsoft.extensions.dependencyinjection.encoderservicecollectionextensions.addwebencoders
        /// </summary>
        public bool EnableWebEncoders { get; set; } = true;

        /// <summary>
        /// 启用缓存（默认true）
        /// </summary>
        public bool EnableResponseCaching { get; set; } = true;

        /// <summary>
        /// 配置参考官方文档。
        /// https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-3.1
        /// </summary>
        public Action<IdentityOptions> AspNetCoreIdentityOptions { get; set; }

        /// <summary>
        /// 隐藏IdentityServer MicroserviceAPI文档
        /// </summary>
        internal void HideIdentityServerDocument(params APIDocumentEnums[] _documents)
        {
            APIDocuments.AddRange(_documents);
        }

        /// <summary>
        /// 允许跨域的url集合(默认读取配置文件的IdentityServer:Origins节点)，多个网址有英文逗号分隔。
        /// 为空代表不允许任何网址跨域
        /// </summary>
        public string Origins { get; set; }

        public Action<IIdentityServerBuilder> IdentityServerBuilder { get; set; }

        public Action<IdentityServerOptions> IdentityServerOptions { get; set; }

        /// <summary>
        /// 启用ReDoc（默认true）。
        /// </summary>
        public bool EnableReDoc { get; set; } = true;

        /// <summary>
        /// ReDoc配置
        /// </summary>
        public Action<ReDocOptions> ReDocOptions { get; set; }

        /// <summary>
        /// ReDoc扩展
        /// https://github.com/Redocly/redoc/blob/master/docs/redoc-vendor-extensions.md
        /// </summary>
        public Action<IDictionary<string, IOpenApiExtension>> ReDocExtensions { get; set; }

        /// <summary>
        /// 启用Client访问速率限制
        /// </summary>
        public bool EnableClientRateLimit { get; set; } = false;

        /// <summary>
        /// 启用IP访问速率限制
        /// </summary>
        public bool EnableIpRateLimit { get; set; } = false;
    }

    public enum APIDocumentEnums
    {
        ALL = 0,

        ApiResource = 1,

        ApiScope = 2,

        Client = 3,

        // CodeGen = 4,

        Blob = 5,

        IdentityResource = 6,

        Role = 7,

        Tenant = 8,

        User = 9,

        Package = 10,

        Authing = 11
    }
}
