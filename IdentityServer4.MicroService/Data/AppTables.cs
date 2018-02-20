using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.MicroService
{
    #region tables
    public class AppUser : IdentityUser<long>
    {
        public long ParentUserID { get; set; }

        [Column(TypeName = "sys.hierarchyid")]
        public string Lineage { get; set; }

        public string LineageIDs { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// 是否已删除账号
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual List<AppUserRole> Roles { get; } = new List<AppUserRole>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual List<AppUserClaim> Claims { get; } = new List<AppUserClaim>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual List<AppUserLogin> Logins { get; } = new List<AppUserLogin>();

        /// <summary>
        /// Navigation property for this users token accounts.
        /// </summary>
        public virtual List<AppUserToken> Tokens { get; } = new List<AppUserToken>();

        /// <summary>
        /// 所属租户
        /// </summary>
        public virtual List<AspNetUserTenant> Tenants { get; } = new List<AspNetUserTenant>();

        /// <summary>
        /// 管理file
        /// </summary>
        public virtual List<AspNetUserFile> Files { get; } = new List<AspNetUserFile>();

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        public double Stature { get; set; }

        /// <summary>
        /// 体重
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后更新日期
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string TypeIDs { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 销售团队
        /// </summary>
        public AspNetUserDistribution Distribution { get; set; }
    }

    public class AppRole : IdentityRole<long>
    {
        /// <summary>
        /// Navigation property for the claims this role belongs to.
        /// </summary>
        public virtual List<AppRoleClaim> Claims { get; } = new List<AppRoleClaim>();
    }

    public class AppRoleClaim : IdentityRoleClaim<long>
    {
    }

    public class AppUserRole : IdentityUserRole<long>
    {
    }

    public class AppUserClaim : IdentityUserClaim<long>
    {
    }

    public class AppUserLogin : IdentityUserLogin<long>
    {
    }

    public class AppUserToken : IdentityUserToken<long>
    {
    }

    /// <summary>
    /// relation between User and Client
    /// </summary>
    [Table("AspNetUserClients")]
    public class AspNetUserClient
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        // 外键
        public long ClientId { get; set; }
    }

    [Table("AspNetUserFiles")]
    public class AspNetUserFile
    {
        public long Id { get; set; }

        public long AppUserId { get; set; }

        /// <summary>
        /// 0图片
        /// 1视频
        /// 2文档
        /// </summary>
        public FileTypes FileType { get; set; }

        public string Files { get; set; }
    }

    /// <summary>
    ///  relation between User and Tenant
    /// </summary>
    [Table("AspNetUserTenants")]
    public class AspNetUserTenant
    {
        public long Id { get; set; }

        public long AppUserId { get; set; }

        // 外键
        public long AppTenantId { get; set; }
    }

    /// <summary>
    /// relation between User and ApiResource
    /// </summary>
    [Table("AspNetUserApiResources")]
    public class AspNetUserApiResource
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        // 外键
        public long ApiResourceId { get; set; }
    }

    public enum FileTypes
    {
        Image = 0,
        Video = 1,
        Doc = 2
    }

    /// <summary>
    /// 销售团队
    /// </summary>
    [Table("AspNetUserDistribution")]
    public class AspNetUserDistribution
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// 团队成员数
        /// </summary>
        public long Members { get; set; }

        /// <summary>
        /// 团队成员数 最后更新时间
        /// </summary>
        public DateTime MembersLastUpdate{ get; set; }

        /// <summary>
        /// 销售额
        /// </summary>
        public decimal Sales { get; set; }

        /// <summary>
        /// 销售额更新时间
        /// </summary>
        public DateTime SalesLastUpdate { get; set; }

        /// <summary>
        /// 总利润
        /// </summary>
        public decimal Earned { get; set; }

        /// <summary>
        /// 差价
        /// </summary>
        public decimal EarnedDiff { get; set; }

        /// <summary>
        /// 差价最后更新时间
        /// </summary>
        public DateTime EarnedDiffLastUpdate { get; set; }

        /// <summary>
        /// 我的提成
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 我的提成最后更新时间
        /// </summary>
        public DateTime CommissionLastUpdate { get; set; }

        /// <summary>
        /// 1级分销提成
        /// </summary>
        public decimal CommissionLv1 { get; set; }
        /// <summary>
        /// 1级分销提成最后更新时间
        /// </summary>
        public DateTime CommissionLv1LastUpdate { get; set; }

        /// <summary>
        /// 2级分销提成
        /// </summary>
        public decimal CommissionLv2 { get; set; }
        /// <summary>
        /// 2级分销提成最后更新时间
        /// </summary>
        public DateTime CommissionLv2LastUpdate { get; set; }

        /// <summary>
        /// 3级分销提成
        /// </summary>
        public decimal CommissionLv3 { get; set; }
        /// <summary>
        /// 3级分销提成最后更新时间
        /// </summary>
        public DateTime CommissionLv3LastUpdate { get; set; }
    }
    #endregion

    public class AppConstant
    {
        /// <summary>
        /// swagger 测试用client，数据库初始化数据
        /// </summary>
        public class TestClient
        {
            public const string ClientId = "test";
            public const string ClientName = "API测试专用";
            public const string ClientSecret = "1";
            public static List<string> RedirectUris = new List<string>()
            { 
                "https://{0}/swagger/o2c.html"
            };
        }

        /// <summary>
        /// 默认管理员账号密码
        /// </summary>
        public class DefaultAdmin
        {
            public const string Email = "1@1.com";
            public const string UserName = "1@1.com";
            public const string PasswordHash = "123456aA!";
        }

        /// <summary>
        /// 微服务名称
        /// </summary>
        public const string MicroServiceName = "ids4.ms";

        /// <summary>
        /// https://identityserver4.readthedocs.io/en/release/topics/add_apis.html
        /// Campaign.Core.IdentityServer.AuthenticationScheme
        /// </summary>
        public const string AppAuthenScheme = "token";

        /// <summary>
        /// 策略
        /// 对应Identity的ClaimType
        /// </summary>
        public class ClaimTypes
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

        /// <summary>
        /// Client权限定义
        /// 对应Token中的claim的scope字段
        /// 字段名：用去controller 的 action 标记
        /// 字段值：策略的名称
        /// 字段自定义属性：策略的权限集合，
        /// 聚合PolicyClaimValues所有的值（除了"all"），去重后登记到IdentityServer的ApiResource中去
        /// 例如PolicyClaimValues("id4.ms.create", "id4.ms.all", "all"),代表
        /// 当前id4.ms项目的create权限，或者 id4.ms.all权限，或者all权限
        /// </summary>
        public class ClientScopes
        {
            [Description("创建")]
            [PolicyClaimValues(MicroServiceName + ".create", MicroServiceName + ".all")]
            public const string Create = "scope:create";

            [Description("读取")]
            [PolicyClaimValues(MicroServiceName + ".read", MicroServiceName + ".all")]
            public const string Read = "scope:read";

            [Description("更新")]
            [PolicyClaimValues(MicroServiceName + ".update", MicroServiceName + ".all")]
            public const string Update = "scope:update";

            [Description("删除")]
            [PolicyClaimValues(MicroServiceName + ".delete", MicroServiceName + ".all")]
            public const string Delete = "scope:delete";

            [Description("批准")]
            [PolicyClaimValues(MicroServiceName + ".approve", MicroServiceName + ".all")]
            public const string Approve = "scope:approve";

            [Description("拒绝")]
            [PolicyClaimValues(MicroServiceName + ".reject", MicroServiceName + ".all")]
            public const string Reject = "scope:reject";

            [Description("上传")]
            [PolicyClaimValues(MicroServiceName + ".upload", MicroServiceName + ".all")]
            public const string Upload = "scope:upload";
        }

        /// <summary>
        /// User权限定义
        /// 对应Token中的claim的permission字段
        /// 字段名：用去controller 的 action 标记
        /// 字段值：策略的名称
        /// 字段自定义属性：策略的权限集合，可按需设置User表的claims的permission属性
        /// </summary>
        public class UserPermissions
        {
            [PolicyClaimValues(MicroServiceName + ".create", MicroServiceName + ".all")]
            public const string Create = "permission:create";

            [PolicyClaimValues(MicroServiceName + ".read", MicroServiceName + ".all")]
            public const string Read = "permission:read";

            [PolicyClaimValues(MicroServiceName + ".update", MicroServiceName + ".all")]
            public const string Update = "permission:update";

            [PolicyClaimValues(MicroServiceName + ".delete", MicroServiceName + ".all")]
            public const string Delete = "permission:delete";

            [PolicyClaimValues(MicroServiceName + ".approve", MicroServiceName + ".all")]
            public const string Approve = "permission:approve";

            [PolicyClaimValues(MicroServiceName + ".reject", MicroServiceName + ".all")]
            public const string Reject = "permission:reject";

            [PolicyClaimValues(MicroServiceName + ".upload", MicroServiceName + ".all")]
            public const string Upload = "permission:upload";
        }

        /// <summary>
        /// 角色
        /// </summary>
        public class Roles
        {
            /// <summary>
            ///  用户
            /// </summary>
            [DisplayName("用户")]
            public const string Users = "users";

            /// <summary>
            /// 合作商
            /// </summary>
            [DisplayName("合作商")]
            public const string Partners = "partners";

            /// <summary>
            /// 开发者
            /// </summary>
            [DisplayName("开发者")]
            public const string Developer = "developer";

            /// <summary>
            /// 管理员
            /// </summary>
            [DisplayName("管理员")]
            public const string Administrators = "administrators";
        }

        public class RedisKeys
        {
            #region 电话号码
            /// <summary>
            /// （缓存KEY）
            /// 24小时内发送短信验证码的次数计数
            /// Key的格式：{手机号}
            /// Value的格式： 次数，调用SDK的increase方法即可，每次加1
            /// </summary>
            public const string Limit_24Hour_Verify_Phone = "PhoneVerifyCode-Limit24Hour:";

            /// <summary>
            /// 24小时内发送短信验证码的上限
            /// </summary>
            public const int Limit_24Hour_Verify_MAX_Phone = 10;

            /// <summary>
            /// （缓存KEY）
            /// Key的格式：{手机号}
            /// Value的格式：当前时间的tick            
            /// </summary>
            public const string LastTime_SendCode_Phone = "PhoneVerifyCode-LastSend:";

            /// <summary>
            /// 每次发送验证码的最小间隔时长（秒）
            /// </summary>
            public const int MinimumTime_SendCode_Phone = 60;

            /// <summary>
            /// （缓存KEY）
            /// Key的格式：{手机号} + ":" + {验证码}
            /// Value的格式：任意
            /// </summary>
            public const string VerifyCode_Phone = "PhoneVerifyCode:";

            /// <summary>
            /// 发送手机验证码后，该验证码在服务器的有效时长（秒）
            /// </summary>
            public const int VerifyCode_Expire_Phone = 300;
            #endregion

            #region 邮箱地址
            /// <summary>
            /// （缓存KEY）
            /// 24小时内发送邮件验证码的次数计数
            /// Key的格式：{手机号}
            /// Value的格式： 次数，调用SDK的increase方法即可，每次加1
            /// </summary>
            public const string Limit_24Hour_Verify_Email = "EmailVerifyCode-Limit24Hour:";

            /// <summary>
            /// 24小时内发送邮件验证码的上限
            /// </summary>
            public const int Limit_24Hour_Verify_MAX_Email = 999999;

            /// <summary>
            /// （缓存KEY）
            /// Key的格式：{邮箱地址}
            /// Value的格式：当前时间的tick            
            /// </summary>
            public const string LastTime_SendCode_Email = "EmailVerifyCode-LastSend:";

            /// <summary>
            /// 每次发送验证码的最小间隔时长（秒）
            /// </summary>
            public const int MinimumTime_SendCode_Email = 60;

            /// <summary>
            /// （缓存KEY）
            /// Key的格式：{邮箱地址} + ":" + {验证码}
            /// Value的格式：任意
            /// </summary>
            public const string VerifyCode_Email = "EmailVerifyCode:";

            /// <summary>
            /// 发送邮箱验证码后，该验证码在服务器的有效时长（秒）
            /// </summary>
            public const int VerifyCode_Expire_Email = 1800;
            #endregion
        }

        public class AzureApiManagementConsts
        {
            /// <summary>
            /// 
            /// </summary>
            public const string Host = "Azure:ApiManagement:Host";

            /// <summary>
            /// 
            /// </summary>
            public const string ApiId = "Azure:ApiManagement:ApiId";

            /// <summary>
            /// 
            /// </summary>
            public const string ApiKey = "Azure:ApiManagement:ApiKey";

            /// <summary>
            /// 
            /// </summary>
            public const string AuthorizationServerId = "Azure:ApiManagement:AuthorizationServerId";

            /// <summary>
            /// 
            /// </summary>
            public const string ProductId = "Azure:ApiManagement:ProductId";

            /// <summary>
            /// 
            /// </summary>
            public const string PortalUris = "Azure:ApiManagement:PortalUris";
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PolicyClaimValuesAttribute: Attribute
    {
        public string[] ClaimsValues { get; set; }

        public PolicyClaimValuesAttribute() { }

        public PolicyClaimValuesAttribute(params string[] ClaimsValues)
        {
            this.ClaimsValues = ClaimsValues;
        }
    }

    public class DefaultData
    {
        public static string IdentityServerIssuerUri = "localhost:44309";
        public static string AppHostName = "localhost:44309";

        public static Dictionary<string, string> TenantProperties =
            new Dictionary<string, string>()
        {
                //auth login
            {"Weixin:ClientId","" },
            { "Weixin:ClientSecret", ""},
            { "Weibo:ClientId", ""},
            { "Weibo:ClientSecret", ""},
            { "GitHub:ClientId", ""},
            { "GitHub:ClientSecret", ""},
            { "QQ:ClientId", ""},
            { "QQ:ClientSecret", ""},
            { "Facebook:ClientId", ""},
            { "Facebook:ClientSecret", ""},
            { "Microsoft:ClientId", ""},
            { "Microsoft:ClientSecret", ""},
            { "Google:ClientId", ""},
            { "Google:ClientSecret", ""},
            { "Twitter:ClientId", ""},
            { "Twitter:ClientSecret", ""},

            //AzureApiManagement
            { "Azure:ApiManagement:Host", ""},
            { "Azure:ApiManagement:ApiId", ""},
            { "Azure:ApiManagement:ApiKey", ""},
            { "Azure:ApiManagement:AuthorizationServerId", ""},
            { "Azure:ApiManagement:ProductId", ""},
            { "Azure:ApiManagement:PortalUris", ""}, 
        };
    }
}
