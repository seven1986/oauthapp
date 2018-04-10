using System.ComponentModel;

namespace IdentityServer4.MicroService.Enums
{
    internal enum CodeGenControllerEnums
    {
        /// <summary>
        /// 获取swagger文档失败
        /// </summary>
        [Description("获取swagger文档失败")]
        GenerateClient_GetSwaggerFialed = 200001,

        /// <summary>
        /// 不存在的模板
        /// </summary>
        [Description("不存在的模板")]
        GenerateClient_GetTemplateFialed = 200002,


        /// <summary>
        /// 不存在的配置
        /// </summary>
        [Description("不存在的配置")]
        NpmOptions_GetOptionsFialed = 200003,
    }
}
