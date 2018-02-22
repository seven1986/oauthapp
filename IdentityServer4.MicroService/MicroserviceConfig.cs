using System;
using System.ComponentModel;

namespace IdentityServer4.MicroService
{
    public class MicroserviceConfig
    {
        /// <summary>
        /// 微服务名称
        /// </summary>
        public const string MicroServiceName = "ids4.ms";

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

        /// <summary>
        /// UserPermission,ClientScope
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
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PolicyClaimValuesAttribute : Attribute
    {
        public string[] ClaimsValues { get; set; }

        public PolicyClaimValuesAttribute() { }

        public PolicyClaimValuesAttribute(params string[] ClaimsValues)
        {
            this.ClaimsValues = ClaimsValues;
        }
    }
}
