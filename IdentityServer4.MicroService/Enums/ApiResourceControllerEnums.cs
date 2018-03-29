using System.ComponentModel;

namespace IdentityServer4.MicroService.Enums
{
    internal enum ApiResourceControllerEnums
    {
        /// <summary>
        /// 不存在的模板
        /// </summary>
        [Description("请填写Id")]
        Revisions_IdCanNotBeNull = 300001,

        [Description("获取Api详情失败")]
        PublishRevision_GetDetailFailed = 300002,

        [Description("发布修订版失败")]
        PublishRevision_PublishFailed = 300003,

        [Description("更新修订内容失败")]
        PublishRevision_CreateReleaseFailed = 300004,
    }
}
