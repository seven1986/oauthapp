using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace IdentityServer4.MicroService
{
    public class AppConstant
    {
        /// <summary>
        /// https://identityserver4.readthedocs.io/en/release/topics/add_apis.html
        /// </summary>
        public const string AppAuthenScheme = "token";

        /// <summary>
        /// seed ID for user
        /// </summary>
        public const long seedUserId = 1;

        /// <summary>
        /// seed ID for tenant
        /// </summary>
        public const long seedTenantId = 1;

        /// <summary>
        /// 项目名称
        /// </summary>
        public static string AssemblyName = Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// 微服务名称，必须是英文
        /// </summary>
        public const string MicroServiceName = "isms";

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public const bool InitializeDatabase = true;
    }

    public class PolicyConfig
    {
        public string ControllerName { get; set; }

        public List<string> Scopes { get; set; } = new List<string>();

        public List<string> Permissions { get; set; } = new List<string>();
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class DefaultRoles
    {
        /// <summary>
        ///  用户
        /// </summary>
        [DisplayName("用户")]
        public const string User = "user";

        /// <summary>
        /// 合作商
        /// </summary>
        [DisplayName("合作商")]
        public const string Partner = "partner";

        /// <summary>
        /// 开发者
        /// </summary>
        [DisplayName("开发者")]
        public const string Developer = "developer";

        /// <summary>
        /// 管理员
        /// </summary>
        [DisplayName("管理员")]
        public const string Administrator = "administrator";
    }

    /// <summary>
    /// PolicyKey(ClaimType)
    /// </summary>
    public class PolicyKey
    {
        /// <summary>
        /// for User
        /// </summary>
        public const string UserPermission = "permission";

        /// <summary>
        /// for client
        /// </summary>
        public const string ClientScope = "scope";
    }
}
