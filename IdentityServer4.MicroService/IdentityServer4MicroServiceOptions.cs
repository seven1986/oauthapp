using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.MicroService
{
    public class IdentityServer4MicroServiceOptions
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
        /// 当前项目的网址(默认读取IdentityServer:Host)
        /// 如：https://127.0.0.1（必须为https，网址结尾无需带/）
        /// </summary>
        public Uri IdentityServerUri { get; set; }

        /// <summary>
        /// 启用权限（默认true）
        /// </summary>
        public bool EnableAuthorizationPolicy { get; set; } = true;

        /// <summary>
        /// 启用SwaggerGen（默认true）
        /// </summary>
        public bool EnableSwaggerGen { get; set; } = true;

        /// <summary>
        /// 启用IdentityServer4Microservice的SwaggerGen（默认true）
        /// </summary>
        public bool EnableISMSSwaggerGen { get; set; } = true;

        /// <summary>
        /// 启用SwaggerUI（默认true）
        /// </summary>
        public bool EnableSwaggerUI { get; set; } = true;

        /// <summary>
        /// 启用跨域（默认true）
        /// </summary>
        public bool EnableCors { get; set; } = true;

        /// <summary>
        /// 启用多语言（默认true）
        /// </summary>
        public bool EnableLocalization { get; set; } = true;

        /// <summary>
        /// 启用版本（默认true）
        /// </summary>
        public bool EnableApiVersioning { get; set; } = true;

        /// <summary>
        /// 启用WebEncoders
        /// </summary>
        public bool EnableWebEncoders { get; set; } = true;

        /// <summary>
        /// 启用缓存（默认true）
        /// </summary>
        public bool EnableResponseCaching { get; set; } = true;

        /// <summary>
        /// 配置参考官方文档
        /// </summary>
        public Action<IdentityOptions> AspNetCoreIdentityOptions { get; set; }

        /// <summary>
        /// 隐藏IdentityServer MicroserviceAPI文档
        /// </summary>
        public void HideIdentityServerDocument()
        {
            ISAPIDocuments.Add(IdentityServerAPIDocuments.ALL);
        }

        /// <summary>
        /// 隐藏IdentityServer MicroserviceAPI文档
        /// </summary>
        public void HideIdentityServerDocument(params IdentityServerAPIDocuments[] APIDocuments)
        {
            ISAPIDocuments.AddRange(APIDocuments);
        }

        internal static List<IdentityServerAPIDocuments> ISAPIDocuments { get; set; } = new List<IdentityServerAPIDocuments>();
    }

    public enum IdentityServerAPIDocuments
    {
        ALL = 0,

        ApiResource = 1,

        Client = 2,

        CodeGen = 3,

        File = 4,

        IdentityResource = 5,

        Role = 6,

        Tenant = 7,

        User = 8
    }
}
