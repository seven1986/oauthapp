using System.ComponentModel;

namespace IdentityServer4.MicroService.Enums
{
    internal enum ApiResourceControllerEnums
    {
        #region Versions
        /// <summary>
        /// 版本列表 - 不存在的模板
        /// </summary>
        [Description("请填写Id")]
        Versions_IdCanNotBeNull = 300001,

        /// <summary>
        /// 版本列表 - 获取Api详情失败
        /// </summary>
        [Description("获取Api详情失败")]
        Versions_GetDetailFailed = 300002,

        /// <summary>
        /// 版本列表 - 获取Api版本失败
        /// </summary>
        [Description("获取Api版本失败")]
        Versions_GetVersionListFailed = 300003,
        #endregion

        #region PublishRevision
        /// <summary>
        /// 发布修订版 - 获取Api详情失败
        /// </summary>
        [Description("获取Api详情失败")]
        PublishRevision_GetDetailFailed = 300004,

        /// <summary>
        /// 发布修订版 - 发布修订版失败
        /// </summary>
        [Description("发布修订版失败")]
        PublishRevision_PublishFailed = 300005,

        /// <summary>
        /// 发布修订版 - 更新修订内容失败
        /// </summary>
        [Description("更新修订内容失败")]
        PublishRevision_CreateReleaseFailed = 300006,
        #endregion

        #region Releases
        /// <summary>
        /// 修订记录列表 - 不存在的模板
        /// </summary>
        [Description("请填写Id")]
        Releases_IdCanNotBeNull = 300007,

        /// <summary>
        /// 修订记录列表 - 获取列表失败
        /// </summary>
        [Description("获取修订记录列表失败")]
        Releases_GetVersionListFailed = 300008,
        #endregion

        /// <summary>
        /// 订阅者 - 添加订阅失败
        /// </summary>
        [Description("添加订阅失败")]
        Subscription_PostFailed =300009,


        /// <summary>
        /// 上线版本失败
        /// </summary>
        [Description("上线版本失败")]
        SetOnlineVersion_PostFailed =300010,
    }
}
