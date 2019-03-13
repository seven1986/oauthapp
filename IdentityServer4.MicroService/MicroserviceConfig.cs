using System.ComponentModel;
using IdentityServer4.MicroService.Attributes;

namespace IdentityServer4.MicroService
{
    public class MicroserviceConfig
    {
        /// <summary>
        /// Client权限
        /// </summary>
        public class ClientScopes
        {
            #region ApiResourceController
            #region 微服务 - 列表
            [Description("微服务 - 列表")]
            [PolicyClaimValues("apiresource","get")]
            public const string ApiResourceGet = "scope:apiresource.get";
            #endregion

            #region 微服务 - 详情
            [Description("微服务 - 详情")]
            [PolicyClaimValues("apiresource","detail")]
            public const string ApiResourceDetail = "scope:apiresource.detail";
            #endregion

            #region 微服务 - 创建
            [Description("微服务 - 创建")]
            [PolicyClaimValues("apiresource","post")]
            public const string ApiResourcePost = "scope:apiresource.post";
            #endregion

            #region 微服务 - 更新
            [Description("微服务 - 更新")]
            [PolicyClaimValues("apiresource","put")]
            public const string ApiResourcePut = "scope:apiresource.put";
            #endregion

            #region 微服务 - 删除
            [Description("微服务 - 删除")]
            [PolicyClaimValues("apiresource","delete")]
            public const string ApiResourceDelete = "scope:apiresource.delete";
            #endregion

            #region 微服务 - 权限代码
            [Description("微服务 - 权限代码")]
            [PolicyClaimValues("apiresource","scopes")]
            public const string ApiResourceScopes = "scope:apiresource.scopes";
            #endregion

            #region 微服务 - 网关 - 发布或更新版本
            [Description("微服务 - 网关 - 发布或更新版本")]
            [PolicyClaimValues("apiresource","publish")]
            public const string ApiResourcePublish = "scope:apiresource.publish";
            #endregion

            #region 微服务 - 网关 - 创建修订版
            [Description("微服务 - 网关 - 创建修订版")]
            [PolicyClaimValues("apiresource","publishrevision")]
            public const string ApiResourcePublishRevision = "scope:apiresource.publishrevision";
            #endregion

            #region 微服务 - 网关 - 创建新版本
            [Description("微服务 - 网关 - 创建新版本")]
            [PolicyClaimValues("apiresource","publishversion")]
            public const string ApiResourcePublishVersion = "scope:apiresource.publishversion";
            #endregion

            #region 微服务 - 网关 - 上次发布配置
            [Description("微服务 - 网关 - 上次发布配置")]
            [PolicyClaimValues("apiresource","publishconfiguration")]
            public const string ApiResourcePublishConfiguration = "scope:apiresource.publishconfiguration";
            #endregion

            #region 微服务 - 网关 - 版本列表
            [Description("微服务 - 网关 - 版本列表")]
            [PolicyClaimValues("apiresource","versions")]
            public const string ApiResourceVersions = "scope:apiresource.versions";
            #endregion

            #region 微服务 - 网关 - 上线指定版本
            [Description("微服务 - 网关 - 上线指定版本")]
            [PolicyClaimValues("apiresource","setonlineversion")]
            public const string ApiResourceSetOnlineVersion = "scope:apiresource.setonlineversion";
            #endregion

            #region 微服务 - 网关 - OAuthServers
            [Description("微服务 - 网关 - OAuthServers")]
            [PolicyClaimValues("apiresource","authservers")]
            public const string ApiResourceAuthServers = "scope:apiresource.authservers";
            #endregion

            #region 微服务 - 网关 - 产品组
            [Description("微服务 - 网关 - 产品组")]
            [PolicyClaimValues("apiresource","products")]
            public const string ApiResourceProducts = "scope:apiresource.products";
            #endregion

            #region 微服务 - 修订内容 - 列表
            [Description("微服务 - 修订内容 - 列表")]
            [PolicyClaimValues("apiresource","releases")]
            public const string ApiResourceReleases = "scope:apiresource.releases";
            #endregion

            #region 微服务 - 修订内容 - 发布
            [Description("微服务 - 修订内容 - 发布")]
            [PolicyClaimValues("apiresource","postrelease")]
            public const string ApiResourcePostRelease = "scope:apiresource.postrelease";
            #endregion

            #region 微服务 - 修订内容 - 更新
            [Description("微服务 - 修订内容 - 更新")]
            [PolicyClaimValues("apiresource","putrelease")]
            public const string ApiResourcePutRelease = "scope:apiresource.putrelease";
            #endregion

            #region 微服务 - 修订内容 - 删除
            [Description("微服务 - 修订内容 - 删除")]
            [PolicyClaimValues("apiresource","deleterelease")]
            public const string ApiResourceDeleteRelease = "scope:apiresource.deleterelease";
            #endregion

            #region 微服务 - 订阅者 - 列表
            [Description("微服务 - 订阅者 - 列表")]
            [PolicyClaimValues("apiresource","subscriptions")]
            public const string ApiResourceSubscriptions = "scope:apiresource.subscriptions";
            #endregion

            #region 微服务 - 订阅者 - 验证邮箱
            [Description("微服务 - 订阅者 - 验证邮箱")]
            [PolicyClaimValues(true,"apiresource","verifyemail")]
            public const string ApiResourceVerifyEmail = "scope:apiresource.verifyemail";
            #endregion

            #region 微服务 - 包市场 - 列表
            [Description("微服务 - 包市场 - 列表")]
            [PolicyClaimValues(true,"apiresource","packages")]
            public const string ApiResourcePackages = "scope:apiresource.packages";
            #endregion

            #region 微服务 - 包市场 - 添加
            [Description("微服务 - 包市场 - 添加")]
            [PolicyClaimValues("apiresource","postpackages")]
            public const string ApiResourcePostPackage = "scope:apiresource.postpackages";
            #endregion

            #region 微服务 - 包市场 - 删除
            [Description("微服务 - 包市场 - 删除")]
            [PolicyClaimValues("apiresource","deletepackage")]
            public const string ApiResourceDeletePackage = "scope:apiresource.deletepackage";
            #endregion

            #region 微服务 - 包市场 - 更新
            [Description("微服务 - 包市场 - 更新")]
            [PolicyClaimValues("apiresource","putpackage")]
            public const string ApiResourcePutPackage = "scope:apiresource.putpackage";
            #endregion
            #endregion

            #region ClientController
            #region 客户端 - 列表
            [Description("客户端 - 列表")]
            [PolicyClaimValues("client","get")]
            public const string ClientGet = "scope:client.get";
            #endregion

            #region 客户端 - 详情
            [Description("客户端 - 详情")]
            [PolicyClaimValues("client","detail")]
            public const string ClientDetail = "scope:client.detail";
            #endregion

            #region 客户端 - 创建
            [Description("客户端 - 创建")]
            [PolicyClaimValues("client","post")]
            public const string ClientPost = "scope:client.post";
            #endregion

            #region 客户端 - 更新
            [Description("客户端 - 更新")]
            [PolicyClaimValues("client","put")]
            public const string ClientPut = "scope:client.put";
            #endregion

            #region 客户端 - 删除
            [Description("客户端 - 删除")]
            [PolicyClaimValues("client","delete")]
            public const string ClientDelete = "scope:client.delete";
            #endregion

            #region 客户端 - 创建令牌
            [Description("客户端 - 创建令牌")]
            [PolicyClaimValues("client","issuetoken")]
            public const string ClientIssueToken = "scope:client.issuetoken";
            #endregion

            #region 客户端 - 生成密钥
            [Description("客户端 - 生成密钥")]
            [PolicyClaimValues("client","postsecretkey")]
            public const string ClientPostSecretkey = "scope:client.postsecretkey";
            #endregion
            #endregion

            #region FileController
            #region 文件 - 上传视频或文档
            [Description("文件 - 上传视频或文档")]
            [PolicyClaimValues("file","post")]
            public const string FilePost = "scope:file.post";
            #endregion

            #region 文件 - 上传图片
            [Description("文件 - 上传图片")]
            [PolicyClaimValues("file","image")]
            public const string FileImage = "scope:file.image";
            #endregion
            #endregion

            #region IdentityResourceController
            #region 身份服务 - 列表
            [Description("身份服务 - 列表")]
            [PolicyClaimValues("identityresource","get")]
            public const string IdentityResourceGet = "scope:identityresource.get";
            #endregion

            #region 身份服务 - 详情
            [Description("身份服务 - 详情")]
            [PolicyClaimValues("identityresource", "detail")]
            public const string IdentityResourceDetail = "scope:identityresource.detail";
            #endregion

            #region 身份服务 - 创建
            [Description("身份服务 - 创建")]
            [PolicyClaimValues("identityresource", "post")]
            public const string IdentityResourcePost = "scope:identityresource.post";
            #endregion

            #region 身份服务 - 更新
            [Description("身份服务 - 更新")]
            [PolicyClaimValues("identityresource", "put")]
            public const string IdentityResourcePut = "scope:identityresource.put";
            #endregion

            #region 身份服务 - 删除
            [Description("身份服务 - 删除")]
            [PolicyClaimValues("identityresource", "delete")]
            public const string IdentityResourceDelete = "scope:identityresource.delete";
            #endregion
            #endregion

            #region RoleController
            #region 角色 - 列表
            [Description("角色 - 列表")]
            [PolicyClaimValues("role", "get")]
            public const string RoleGet = "scope:role.get";
            #endregion

            #region 角色 - 详情
            [Description("角色 - 详情")]
            [PolicyClaimValues("role", "detail")]
            public const string RoleDetail = "scope:role.detail";
            #endregion

            #region 角色 - 创建
            [Description("角色 - 创建")]
            [PolicyClaimValues("role", "post")]
            public const string RolePost = "scope:role.post";
            #endregion

            #region 角色 - 更新
            [Description("角色 - 更新")]
            [PolicyClaimValues("role", "put")]
            public const string RolePut = "scope:role.put";
            #endregion

            #region 角色 - 删除
            [Description("角色 - 删除")]
            [PolicyClaimValues("role", "delete")]
            public const string RoleDelete = "scope:role.delete";
            #endregion
            #endregion

            #region TenantController
            #region 租户 - 列表
            [Description("租户 - 列表")]
            [PolicyClaimValues("tenant","get")]
            public const string TenantGet = "scope:tenant.get";
            #endregion

            #region 租户 - 详情
            [Description("租户 - 详情")]
            [PolicyClaimValues("tenant","detail")]
            public const string TenantDetail = "scope:tenant.detail";
            #endregion

            #region 租户 - 创建
            [Description("租户 - 创建")]
            [PolicyClaimValues("tenant","post")]
            public const string TenantPost = "scope:tenant.post";
            #endregion

            #region 租户 - 更新
            [Description("租户 - 更新")]
            [PolicyClaimValues("tenant","put")]
            public const string TenantPut = "scope:tenant.put";
            #endregion

            #region 租户 - 删除
            [Description("租户 - 删除")]
            [PolicyClaimValues("tenant","delete")]
            public const string TenantDelete = "scope:tenant.delete";
            #endregion
            #endregion

            #region UserController
            #region 用户 - 列表
            [Description("用户 - 列表")]
            [PolicyClaimValues("user","get")]
            public const string UserGet = "scope:user.get";
            #endregion

            #region 用户 - 详情
            [Description("用户 - 详情")]
            [PolicyClaimValues("user","detail")]
            public const string UserDetail = "scope:user.detail";
            #endregion

            #region 用户 - 创建
            [Description("用户 - 创建")]
            [PolicyClaimValues("user","post")]
            public const string UserPost = "scope:user.post";
            #endregion

            #region 用户 - 更新
            [Description("用户 - 更新")]
            [PolicyClaimValues("user","put")]
            public const string UserPut = "scope:user.put";
            #endregion

            #region 用户 - 删除
            [Description("用户 - 删除")]
            [PolicyClaimValues("user","delete")]
            public const string UserDelete = "scope:user.delete";
            #endregion

            #region 用户 - 是否存在
            [Description("用户 - 是否存在")]
            [PolicyClaimValues("user","head")]
            public const string UserHead = "scope:user.head";
            #endregion

            #region 用户 - 注册
            [Description("用户 - 注册")]
            [PolicyClaimValues("user","register")]
            public const string UserRegister = "scope:user.register";
            #endregion

            #region 用户 - 注册 - 发送手机验证码
            [Description("用户 - 注册 - 发送手机验证码")]
            [PolicyClaimValues("user","verifyphone")]
            public const string UserVerifyPhone = "scope:user.verifyphone";
            #endregion

            #region 用户 - 注册 - 发送邮件验证码
            [Description("用户 - 注册 - 发送邮件验证码")]
            [PolicyClaimValues("user","verifyemail")]
            public const string UserVerifyEmail = "scope:user.verifyemail";
            #endregion
            #endregion

            #region CodeGenController
            #region 代码生成 - 客户端列表
            [Description("代码生成 - 客户端列表")]
            [PolicyClaimValues("codegen","get")]
            public const string CodeGenClients = "scope:codegen.get";
            #endregion

            #region 代码生成 - 服务端列表
            [Description("代码生成 - 服务端列表")]
            [PolicyClaimValues("codegen","servers")]
            public const string CodeGenServers = "scope:codegen.servers";
            #endregion

            #region 代码生成 - NPM - 设置
            [Description("代码生成 - NPM - 设置")]
            [PolicyClaimValues("codegen","npmoptions")]
            public const string CodeGenNpmOptions = "scope:codegen.npmoptions";
            #endregion

            #region 代码生成 - NPM - 更新设置
            [Description("代码生成 - NPM - 更新设置")]
            [PolicyClaimValues("codegen","putnpmoptions")]
            public const string CodeGenPutNpmOptions = "scope:codegen.putnpmoptions";
            #endregion

            #region 代码生成 - Github - 设置
            [Description("代码生成 - Github - 设置")]
            [PolicyClaimValues("codegen","githuboptions")]
            public const string CodeGenGithubOptions = "scope:codegen.githuboptions";
            #endregion

            #region 代码生成 - Github - 更新设置
            [Description("代码生成 - Github - 更新设置")]
            [PolicyClaimValues("codegen","putgithuboptions")]
            public const string CodeGenPutGithubOptions = "scope:codegen.putgithuboptions";
            #endregion

            #region 代码生成 - Github - 同步
            [Description("代码生成 - Github - 同步")]
            [PolicyClaimValues("codegen","syncgithub")]
            public const string CodeGenSyncGithub = "scope:codegen.syncgithub";
            #endregion

            #region 代码生成 - 基本设置 - 获取
            [Description("代码生成 - 基本设置 - 获取")]
            [PolicyClaimValues("codegen","commonoptions")]
            public const string CodeGenCommonOptions = "scope:codegen.commonoptions";
            #endregion

            #region 代码生成 - 基本设置 - 更新
            [Description("代码生成 - 基本设置 - 更新")]
            [PolicyClaimValues("codegen","putcommonoptions")]
            public const string CodeGenPutCommonOptions = "scope:codegen.putcommonoptions";
            #endregion

            #region 代码生成 - SDK - 发布
            [Description("代码生成 - SDK - 发布")]
            [PolicyClaimValues("codegen","releasesdk")]
            public const string CodeGenReleaseSDK = "scope:codegen.releasesdk";
            #endregion

            #region 代码生成 - SDK - 预览生成代码
            [Description("代码生成 - SDK - 预览生成代码")]
            [PolicyClaimValues("codegen","gen")]
            public const string CodeGenGen = "scope:codegen.gen";
            #endregion

            #region 代码生成 - SDK - 发布记录
            [Description("代码生成 - SDK - 发布记录")]
            [PolicyClaimValues(true,"apiresource","history")]
            public const string CodeGenHistory = "scope:apiresource.history";
            #endregion
            #endregion
        }

        /// <summary>
        /// User权限
        /// </summary>
        public class UserPermissions
        {
            #region ApiResourceController
            #region 微服务 - 列表
            [Description("微服务 - 列表")]
            [PolicyClaimValues("apiresource", "get")]
            public const string ApiResourceGet = "permission:apiresource.get";
            #endregion

            #region 微服务 - 详情
            [Description("微服务 - 详情")]
            [PolicyClaimValues("apiresource", "detail")]
            public const string ApiResourceDetail = "permission:apiresource.detail";
            #endregion

            #region 微服务 - 创建
            [Description("微服务 - 创建")]
            [PolicyClaimValues("apiresource", "post")]
            public const string ApiResourcePost = "permission:apiresource.post";
            #endregion

            #region 微服务 - 更新
            [Description("微服务 - 更新")]
            [PolicyClaimValues("apiresource", "put")]
            public const string ApiResourcePut = "permission:apiresource.put";
            #endregion

            #region 微服务 - 删除
            [Description("微服务 - 删除")]
            [PolicyClaimValues("apiresource", "delete")]
            public const string ApiResourceDelete = "permission:apiresource.delete";
            #endregion

            #region 微服务 - 权限代码
            [Description("微服务 - 权限代码")]
            [PolicyClaimValues("apiresource", "scopes")]
            public const string ApiResourceScopes = "permission:apiresource.scopes";
            #endregion

            #region 微服务 - 网关 - 发布或更新版本
            [Description("微服务 - 网关 - 发布或更新版本")]
            [PolicyClaimValues("apiresource", "publish")]
            public const string ApiResourcePublish = "permission:apiresource.publish";
            #endregion

            #region 微服务 - 网关 - 创建修订版
            [Description("微服务 - 网关 - 创建修订版")]
            [PolicyClaimValues("apiresource", "publishrevision")]
            public const string ApiResourcePublishRevision = "permission:apiresource.publishrevision";
            #endregion

            #region 微服务 - 网关 - 创建新版本
            [Description("微服务 - 网关 - 创建新版本")]
            [PolicyClaimValues("apiresource", "publishversion")]
            public const string ApiResourcePublishVersion = "permission:apiresource.publishversion";
            #endregion

            #region 微服务 - 网关 - 上次发布配置
            [Description("微服务 - 网关 - 上次发布配置")]
            [PolicyClaimValues("apiresource", "publishconfiguration")]
            public const string ApiResourcePublishConfiguration = "permission:apiresource.publishconfiguration";
            #endregion

            #region 微服务 - 网关 - 版本列表
            [Description("微服务 - 网关 - 版本列表")]
            [PolicyClaimValues("apiresource", "versions")]
            public const string ApiResourceVersions = "permission:apiresource.versions";
            #endregion

            #region 微服务 - 网关 - 上线指定版本
            [Description("微服务 - 网关 - 上线指定版本")]
            [PolicyClaimValues("apiresource", "setonlineversion")]
            public const string ApiResourceSetOnlineVersion = "permission:apiresource.setonlineversion";
            #endregion

            #region 微服务 - 网关 - OAuthServers
            [Description("微服务 - 网关 - OAuthServers")]
            [PolicyClaimValues("apiresource", "authservers")]
            public const string ApiResourceAuthServers = "permission:apiresource.authservers";
            #endregion

            #region 微服务 - 网关 - 产品组
            [Description("微服务 - 网关 - 产品组")]
            [PolicyClaimValues("apiresource", "products")]
            public const string ApiResourceProducts = "permission:apiresource.products";
            #endregion

            #region 微服务 - 修订内容 - 列表
            [Description("微服务 - 修订内容 - 列表")]
            [PolicyClaimValues("apiresource", "releases")]
            public const string ApiResourceReleases = "permission:apiresource.releases";
            #endregion

            #region 微服务 - 修订内容 - 发布
            [Description("微服务 - 修订内容 - 发布")]
            [PolicyClaimValues("apiresource", "postrelease")]
            public const string ApiResourcePostRelease = "permission:apiresource.postrelease";
            #endregion

            #region 微服务 - 修订内容 - 更新
            [Description("微服务 - 修订内容 - 更新")]
            [PolicyClaimValues("apiresource", "putrelease")]
            public const string ApiResourcePutRelease = "permission:apiresource.putrelease";
            #endregion

            #region 微服务 - 修订内容 - 删除
            [Description("微服务 - 修订内容 - 删除")]
            [PolicyClaimValues("apiresource", "deleterelease")]
            public const string ApiResourceDeleteRelease = "permission:apiresource.deleterelease";
            #endregion

            #region 微服务 - 订阅者 - 列表
            [Description("微服务 - 订阅者 - 列表")]
            [PolicyClaimValues("apiresource", "subscriptions")]
            public const string ApiResourceSubscriptions = "permission:apiresource.subscriptions";
            #endregion

            #region 微服务 - 订阅者 - 验证邮箱
            [Description("微服务 - 订阅者 - 验证邮箱")]
            [PolicyClaimValues(true, "apiresource","verifyemail")]
            public const string ApiResourceVerifyEmail = "permission:apiresource.verifyemail";
            #endregion

            #region 微服务 - 包市场 - 列表
            [Description("微服务 - 包市场 - 列表")]
            [PolicyClaimValues(true, "apiresource","packages")]
            public const string ApiResourcePackages = "permission:apiresource.packages";
            #endregion

            #region 微服务 - 包市场 - 添加
            [Description("微服务 - 包市场 - 添加")]
            [PolicyClaimValues("apiresource", "postpackages")]
            public const string ApiResourcePostPackage = "permission:apiresource.postpackages";
            #endregion

            #region 微服务 - 包市场 - 删除
            [Description("微服务 - 包市场 - 删除")]
            [PolicyClaimValues("apiresource", "deletepackage")]
            public const string ApiResourceDeletePackage = "permission:apiresource.deletepackage";
            #endregion

            #region 微服务 - 包市场 - 更新
            [Description("微服务 - 包市场 - 更新")]
            [PolicyClaimValues("apiresource", "putpackage")]
            public const string ApiResourcePutPackage = "permission:apiresource.putpackage";
            #endregion
            #endregion

            #region ClientController
            #region 客户端 - 列表
            [Description("客户端 - 列表")]
            [PolicyClaimValues("client", "get")]
            public const string ClientGet = "permission:client.get";
            #endregion

            #region 客户端 - 详情
            [Description("客户端 - 详情")]
            [PolicyClaimValues("client", "detail")]
            public const string ClientDetail = "permission:client.detail";
            #endregion

            #region 客户端 - 创建
            [Description("客户端 - 创建")]
            [PolicyClaimValues("client", "post")]
            public const string ClientPost = "permission:client.post";
            #endregion

            #region 客户端 - 更新
            [Description("客户端 - 更新")]
            [PolicyClaimValues("client", "put")]
            public const string ClientPut = "permission:client.put";
            #endregion

            #region 客户端 - 删除
            [Description("客户端 - 删除")]
            [PolicyClaimValues("client", "delete")]
            public const string ClientDelete = "permission:client.delete";
            #endregion

            #region 客户端 - 创建令牌
            [Description("客户端 - 创建令牌")]
            [PolicyClaimValues("client", "issuetoken")]
            public const string ClientIssueToken = "permission:client.issuetoken";
            #endregion

            #region 客户端 - 生成密钥
            [Description("客户端 - 生成密钥")]
            [PolicyClaimValues("client", "postsecretkey")]
            public const string ClientPostSecretkey = "permission:client.postsecretkey";
            #endregion
            #endregion

            #region FileController
            #region 文件 - 上传视频或文档
            [Description("文件 - 上传视频或文档")]
            [PolicyClaimValues("file", "post")]
            public const string FilePost = "permission:file.post";
            #endregion

            #region 文件 - 上传图片
            [Description("文件 - 上传图片")]
            [PolicyClaimValues("file", "image")]
            public const string FileImage = "permission:file.image";
            #endregion
            #endregion

            #region IdentityResourceController
            #region 身份服务 - 列表
            [Description("身份服务 - 列表")]
            [PolicyClaimValues("identityresource", "get")]
            public const string IdentityResourceGet = "permission:identityresource.get";
            #endregion

            #region 身份服务 - 详情
            [Description("身份服务 - 详情")]
            [PolicyClaimValues("identityresource", "detail")]
            public const string IdentityResourceDetail = "permission:identityresource.detail";
            #endregion

            #region 身份服务 - 创建
            [Description("身份服务 - 创建")]
            [PolicyClaimValues("identityresource", "post")]
            public const string IdentityResourcePost = "permission:identityresource.post";
            #endregion

            #region 身份服务 - 更新
            [Description("身份服务 - 更新")]
            [PolicyClaimValues("identityresource", "put")]
            public const string IdentityResourcePut = "permission:identityresource.put";
            #endregion

            #region 身份服务 - 删除
            [Description("身份服务 - 删除")]
            [PolicyClaimValues("identityresource", "delete")]
            public const string IdentityResourceDelete = "permission:identityresource.delete";
            #endregion
            #endregion

            #region RoleController
            #region 角色 - 列表
            [Description("角色 - 列表")]
            [PolicyClaimValues("role", "get")]
            public const string RoleGet = "permission:role.get";
            #endregion

            #region 角色 - 详情
            [Description("角色 - 详情")]
            [PolicyClaimValues("role", "detail")]
            public const string RoleDetail = "permission:role.detail";
            #endregion

            #region 角色 - 创建
            [Description("角色 - 创建")]
            [PolicyClaimValues("role", "post")]
            public const string RolePost = "permission:role.post";
            #endregion

            #region 角色 - 更新
            [Description("角色 - 更新")]
            [PolicyClaimValues("role", "put")]
            public const string RolePut = "permission:role.put";
            #endregion

            #region 角色 - 删除
            [Description("角色 - 删除")]
            [PolicyClaimValues("role", "delete")]
            public const string RoleDelete = "permission:role.delete";
            #endregion
            #endregion

            #region TenantController
            #region 租户 - 列表
            [Description("租户 - 列表")]
            [PolicyClaimValues("tenant", "get")]
            public const string TenantGet = "permission:tenant.get";
            #endregion

            #region 租户 - 详情
            [Description("租户 - 详情")]
            [PolicyClaimValues("tenant", "detail")]
            public const string TenantDetail = "permission:tenant.detail";
            #endregion

            #region 租户 - 创建
            [Description("租户 - 创建")]
            [PolicyClaimValues("tenant", "post")]
            public const string TenantPost = "permission:tenant.post";
            #endregion

            #region 租户 - 更新
            [Description("租户 - 更新")]
            [PolicyClaimValues("tenant", "put")]
            public const string TenantPut = "permission:tenant.put";
            #endregion

            #region 租户 - 删除
            [Description("租户 - 删除")]
            [PolicyClaimValues("tenant", "delete")]
            public const string TenantDelete = "permission:tenant.delete";
            #endregion
            #endregion

            #region UserController
            #region 用户 - 列表
            [Description("用户 - 列表")]
            [PolicyClaimValues("user", "get")]
            public const string UserGet = "permission:user.get";
            #endregion

            #region 用户 - 详情
            [Description("用户 - 详情")]
            [PolicyClaimValues("user", "detail")]
            public const string UserDetail = "permission:user.detail";
            #endregion

            #region 用户 - 创建
            [Description("用户 - 创建")]
            [PolicyClaimValues("user", "post")]
            public const string UserPost = "permission:user.post";
            #endregion

            #region 用户 - 更新
            [Description("用户 - 更新")]
            [PolicyClaimValues("user", "put")]
            public const string UserPut = "permission:user.put";
            #endregion

            #region 用户 - 删除
            [Description("用户 - 删除")]
            [PolicyClaimValues("user", "delete")]
            public const string UserDelete = "permission:user.delete";
            #endregion

            #region 用户 - 是否存在
            [Description("用户 - 是否存在")]
            [PolicyClaimValues("user", "head")]
            public const string UserHead = "permission:user.head";
            #endregion

            #region 用户 - 注册
            [Description("用户 - 注册")]
            [PolicyClaimValues("user", "register")]
            public const string UserRegister = "permission:user.register";
            #endregion

            #region 用户 - 注册 - 发送手机验证码
            [Description("用户 - 注册 - 发送手机验证码")]
            [PolicyClaimValues("user", "verifyphone")]
            public const string UserVerifyPhone = "permission:user.verifyphone";
            #endregion

            #region 用户 - 注册 - 发送邮件验证码
            [Description("用户 - 注册 - 发送邮件验证码")]
            [PolicyClaimValues("user", "verifyemail")]
            public const string UserVerifyEmail = "permission:user.verifyemail";
            #endregion
            #endregion

            #region CodeGenController
            #region 代码生成 - 客户端列表
            [Description("代码生成 - 客户端列表")]
            [PolicyClaimValues("codegen", "get")]
            public const string CodeGenClients = "permission:codegen.get";
            #endregion

            #region 代码生成 - 服务端列表
            [Description("代码生成 - 服务端列表")]
            [PolicyClaimValues("codegen", "servers")]
            public const string CodeGenServers = "permission:codegen.servers";
            #endregion

            #region 代码生成 - NPM - 设置
            [Description("代码生成 - NPM - 设置")]
            [PolicyClaimValues("codegen", "npmoptions")]
            public const string CodeGenNpmOptions = "permission:codegen.npmoptions";
            #endregion

            #region 代码生成 - NPM - 更新设置
            [Description("代码生成 - NPM - 更新设置")]
            [PolicyClaimValues("codegen", "putnpmoptions")]
            public const string CodeGenPutNpmOptions = "permission:codegen.putnpmoptions";
            #endregion

            #region 代码生成 - Github - 设置
            [Description("代码生成 - Github - 设置")]
            [PolicyClaimValues("codegen", "githuboptions")]
            public const string CodeGenGithubOptions = "permission:codegen.githuboptions";
            #endregion

            #region 代码生成 - Github - 更新设置
            [Description("代码生成 - Github - 更新设置")]
            [PolicyClaimValues("codegen", "putgithuboptions")]
            public const string CodeGenPutGithubOptions = "permission:codegen.putgithuboptions";
            #endregion

            #region 代码生成 - Github - 同步
            [Description("代码生成 - Github - 同步")]
            [PolicyClaimValues("codegen", "syncgithub")]
            public const string CodeGenSyncGithub = "permission:codegen.syncgithub";
            #endregion

            #region 代码生成 - 基本设置 - 获取
            [Description("代码生成 - 基本设置 - 获取")]
            [PolicyClaimValues("codegen", "commonoptions")]
            public const string CodeGenCommonOptions = "permission:codegen.commonoptions";
            #endregion

            #region 代码生成 - 基本设置 - 更新
            [Description("代码生成 - 基本设置 - 更新")]
            [PolicyClaimValues("codegen", "putcommonoptions")]
            public const string CodeGenPutCommonOptions = "permission:codegen.putcommonoptions";
            #endregion

            #region 代码生成 - SDK - 发布
            [Description("代码生成 - SDK - 发布")]
            [PolicyClaimValues("codegen", "releasesdk")]
            public const string CodeGenReleaseSDK = "permission:codegen.releasesdk";
            #endregion

            #region 代码生成 - SDK - 预览生成代码
            [Description("代码生成 - SDK - 预览生成代码")]
            [PolicyClaimValues("codegen", "gen")]
            public const string CodeGenGen = "permission:codegen.gen";
            #endregion

            #region 代码生成 - SDK - 发布记录
            [Description("代码生成 - SDK - 发布记录")]
            [PolicyClaimValues(true, "apiresource", "history")]
            public const string CodeGenHistory = "permission:apiresource.history";
            #endregion
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
}
