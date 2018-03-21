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
        /// 字段名：用于controller 的 action 标记
        /// 字段值：策略的名称
        /// 字段自定义属性：策略的权限集合，
        /// 聚合PolicyClaimValues所有的值（除了"all"），去重后登记到IdentityServer的ApiResource中去
        /// 例如PolicyClaimValues("id4.ms.create", "id4.ms.all", "all"),代表
        /// 当前id4.ms项目的create权限，或者 id4.ms.all权限，或者all权限
        /// </summary>
        public class ClientScopes
        {
            #region ApiResourceController
            [Description("微服务 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".apiresource.get",
                    MicroServiceName + ".apiresource.all",
                    MicroServiceName + ".all")]
            public const string ApiResourceGet = "scope:apiresource.get";

            [Description("微服务 - 详情")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.detail",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceDetail = "scope:apiresource.detail";

            [Description("微服务 - 创建")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.post",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePost = "scope:apiresource.post";

            [Description("微服务 - 更新")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.put",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePut = "scope:apiresource.put";

            [Description("微服务 - 删除")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.delete",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceDelete = "scope:apiresource.delete";

            [Description("微服务 - 发布到网关")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.publish",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePublish = "scope:apiresource.publish";

            [Description("微服务 - 上次发布配置")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.publishsetting",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePublishSetting = "scope:apiresource.publishsetting";

            [Description("微服务 - OAuthServers")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.authservers",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceAuthServers = "scope:apiresource.authservers";

            [Description("微服务 - 产品组")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.products",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceProducts = "scope:apiresource.products";
            #endregion

            #region ClientController
            [Description("客户端 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.get",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientGet = "scope:client.get";

            [Description("客户端 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.detail",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientDetail = "scope:client.detail";

            [Description("客户端 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.post",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientPost = "scope:client.post";

            [Description("客户端 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.put",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientPut = "scope:client.put";

            [Description("客户端 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.delete",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientDelete = "scope:client.delete";
            #endregion

            #region FileController
            [Description("文件 - 上传视频或文档")]
            [PolicyClaimValues(
                    MicroServiceName + ".file.post",
                    MicroServiceName + ".file.all",
                    MicroServiceName + ".all")]
            public const string FilePost = "scope:file.post";

            [Description("文件 - 上传图片")]
            [PolicyClaimValues(
                    MicroServiceName + ".file.image",
                    MicroServiceName + ".file.all",
                    MicroServiceName + ".all")]
            public const string FileImage = "scope:file.image";
            #endregion

            #region IdentityResourceController
            [Description("身份服务 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.get",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourceGet = "scope:identityresource.get";

            [Description("身份服务 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.detail",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourceDetail = "scope:identityresource.detail";

            [Description("身份服务 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.post",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourcePost = "scope:identityresource.post";

            [Description("身份服务 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.put",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourcePut = "scope:identityresource.put";

            [Description("身份服务 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.delete",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourceDelete = "scope:identityresource.delete";
            #endregion

            #region RoleController
            [Description("角色 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.get",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RoleGet = "scope:role.get";

            [Description("角色 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.detail",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RoleDetail = "scope:role.detail";

            [Description("角色 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.post",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RolePost = "scope:role.post";

            [Description("角色 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.put",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RolePut = "scope:role.put";

            [Description("角色 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.delete",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RoleDelete = "scope:role.delete";
            #endregion

            #region TenantController
            [Description("租户 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.get",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantGet = "scope:tenant.get";

            [Description("租户 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.detail",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantDetail = "scope:tenant.detail";

            [Description("租户 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.post",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantPost = "scope:tenant.post";

            [Description("租户 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.put",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantPut = "scope:tenant.put";

            [Description("租户 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.delete",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantDelete = "scope:tenant.delete";
            #endregion

            #region UserController
            [Description("用户 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.get",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserGet = "scope:user.get";

            [Description("用户 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.detail",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserDetail = "scope:user.detail";

            [Description("用户 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.post",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserPost = "scope:user.post";

            [Description("用户 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.put",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserPut = "scope:user.put";

            [Description("用户 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.delete",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserDelete = "scope:user.delete";

            [Description("用户 - 是否存在")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.head",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserHead = "scope:user.head";

            [Description("用户 - 注册 (需验证手机号，邮箱如果填写了也需要验证)")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.register",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserRegister = "scope:user.register";

            [Description("用户 - 注册 - 发送手机验证码")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.verifyphone",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserVerifyPhone = "scope:user.verifyphone";

            [Description("用户 - 注册 - 发送邮件验证码")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.verifyemail",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserVerifyEmail = "scope:user.verifyemail";
            #endregion
        }

        /// <summary>
        /// User权限定义
        /// 对应Token中的claim的permission字段
        /// 字段名：用于controller 的 action 标记
        /// 字段值：策略的名称
        /// 字段自定义属性：策略的权限集合，可按需设置User表的claims的permission属性
        /// </summary>
        public class UserPermissions
        {
            #region ApiResourceController
            [Description("微服务 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".apiresource.get",
                    MicroServiceName + ".apiresource.all",
                    MicroServiceName + ".all")]
            public const string ApiResourceGet = "permission:apiresource.get";

            [Description("微服务 - 详情")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.detail",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceDetail = "permission:apiresource.detail";

            [Description("微服务 - 创建")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.post",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePost = "permission:apiresource.post";

            [Description("微服务 - 更新")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.put",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePut = "permission:apiresource.put";

            [Description("微服务 - 删除")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.delete",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceDelete = "permission:apiresource.delete";

            [Description("微服务 - 发布到网关")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.publish",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePublish = "permission:apiresource.publish";

            [Description("微服务 - 上次发布配置")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.publishsetting",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourcePublishSetting = "permission:apiresource.publishsetting";

            [Description("微服务 - OAuthServers")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.authservers",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceAuthServers = "permission:apiresource.authservers";

            [Description("微服务 - 产品组")]
            [PolicyClaimValues(
                MicroServiceName + ".apiresource.products",
                MicroServiceName + ".apiresource.all",
                MicroServiceName + ".all")]
            public const string ApiResourceProducts = "permission:apiresource.products";
            #endregion

            #region ClientController
            [Description("客户端 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.get",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientGet = "permission:client.get";

            [Description("客户端 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.detail",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientDetail = "permission:client.detail";

            [Description("客户端 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.post",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientPost = "permission:client.post";

            [Description("客户端 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.put",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientPut = "permission:client.put";

            [Description("客户端 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".client.delete",
                    MicroServiceName + ".client.all",
                    MicroServiceName + ".all")]
            public const string ClientDelete = "permission:client.delete";
            #endregion

            #region FileController
            [Description("文件 - 上传视频或文档")]
            [PolicyClaimValues(
                    MicroServiceName + ".file.post",
                    MicroServiceName + ".file.all",
                    MicroServiceName + ".all")]
            public const string FilePost = "permission:file.post";

            [Description("文件 - 上传图片")]
            [PolicyClaimValues(
                    MicroServiceName + ".file.image",
                    MicroServiceName + ".file.all",
                    MicroServiceName + ".all")]
            public const string FileImage = "permission:file.image";
            #endregion

            #region IdentityResourceController
            [Description("身份服务 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.get",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourceGet = "permission:identityresource.get";

            [Description("身份服务 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.detail",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourceDetail = "permission:identityresource.detail";

            [Description("身份服务 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.post",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourcePost = "permission:identityresource.post";

            [Description("身份服务 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.put",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourcePut = "permission:identityresource.put";

            [Description("身份服务 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".identityresource.delete",
                    MicroServiceName + ".identityresource.all",
                    MicroServiceName + ".all")]
            public const string IdentityResourceDelete = "permission:identityresource.delete";
            #endregion

            #region RoleController
            [Description("角色 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.get",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RoleGet = "permission:role.get";

            [Description("角色 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.detail",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RoleDetail = "permission:role.detail";

            [Description("角色 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.post",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RolePost = "permission:role.post";

            [Description("角色 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.put",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RolePut = "permission:role.put";

            [Description("角色 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".role.delete",
                    MicroServiceName + ".role.all",
                    MicroServiceName + ".all")]
            public const string RoleDelete = "permission:role.delete";
            #endregion

            #region TenantController
            [Description("租户 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.get",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantGet = "permission:tenant.get";

            [Description("租户 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.detail",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantDetail = "permission:tenant.detail";

            [Description("租户 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.post",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantPost = "permission:tenant.post";

            [Description("租户 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.put",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantPut = "permission:tenant.put";

            [Description("租户 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".tenant.delete",
                    MicroServiceName + ".tenant.all",
                    MicroServiceName + ".all")]
            public const string TenantDelete = "permission:tenant.delete";
            #endregion

            #region UserController
            [Description("用户 - 列表")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.get",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserGet = "permission:user.get";

            [Description("用户 - 详情")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.detail",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserDetail = "permission:user.detail";

            [Description("用户 - 创建")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.post",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserPost = "permission:user.post";

            [Description("用户 - 更新")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.put",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserPut = "permission:user.put";

            [Description("用户 - 删除")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.delete",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserDelete = "permission:user.delete";

            [Description("用户 - 是否存在")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.head",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserHead = "permission:user.head";

            [Description("用户 - 注册 (需验证手机号，邮箱如果填写了也需要验证)")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.register",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserRegister = "permission:user.register";

            [Description("用户 - 注册 - 发送手机验证码")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.verifyphone",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserVerifyPhone = "permission:user.verifyphone";

            [Description("用户 - 注册 - 发送邮件验证码")]
            [PolicyClaimValues(
                    MicroServiceName + ".user.verifyemail",
                    MicroServiceName + ".user.all",
                    MicroServiceName + ".all")]
            public const string UserVerifyEmail = "permission:user.verifyemail";
            #endregion
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
