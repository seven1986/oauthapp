using System.Collections.Generic;
using System.Reflection;

namespace OAuthApp
{
    public class AppConstant
    {
        public const string AuthorizeEndpoint = "/connect/authorize";

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
        /// 标识服务的scope，必须是英文
        /// </summary>
        public const string MicroServiceName = "oauthapp";

        /// <summary>
        /// 初始化数据库。
        /// 如果初始化过数据库，可以设置为这个参数为false，优化性能。
        /// </summary>
        public static bool InitializeDatabase = true;

        public static List<string> WhiteList_Clients = new List<string>()
        {
            MicroServiceName
        };

        public static List<string> WhiteList_RedirectUris = new List<string>()
        { 
            "https://oauth.pstmn.io/v1/callback" // post man
        };
    }

    public class PropertyKeys
    {
        public const string LogoutRedirectUri = "CheckPostLogoutRedirectUri";

        public const string RedirectUri = "CheckPostLogoutRedirectUri";
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
        public const string User = "user";

        /// <summary>
        /// 合作商
        /// </summary>
        public const string Partner = "partner";

        /// <summary>
        /// 开发者
        /// </summary>
        public const string Developer = "developer";

        /// <summary>
        /// 管理员
        /// </summary>
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