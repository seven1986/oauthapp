using System;
using System.ComponentModel;
using IdentityServer4.MicroService.Enums;
using Microsoft.Extensions.Localization;

namespace IdentityServer4.MicroService.Models.Apis.Common
{
    public class ApiResult<T>:IApiResult<T>
    {
        /// <summary>
        /// 代码
        /// </summary>
        public int code { get; set; } = (int)BasicControllerEnums.Status200OK;

        /// <summary>
        /// 代码名称
        /// </summary>
        public string codeName { get; set; } = BasicControllerEnums.Status200OK.ToString();

        /// <summary>
        /// 说明
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// 构造函数
        /// (接口正常执行)
        /// </summary>
        public ApiResult()
        {
        }

        /// <summary>
        /// 构造函数
        /// (接口正常执行 + 返回数据)
        /// </summary>
        /// <param name="_data">数据</param>
        public ApiResult(T _data)
        {
            data = _data;
        }

        /// <summary>
        /// 构造函数
        /// (接口报错返回实体)
        /// </summary>
        /// <param name="_code">代码</param>
        /// <param name="l">本地化服务</param>
        /// <param name="_message">自定义说明（将覆盖默认说明）</param>
        /// <param name="arguments">本地化服务的参数</param>
        public ApiResult(IStringLocalizer l, Enum _code, string _message = "", params object[] arguments)
        {
            // 枚举code
            codeName = _code.ToString();

            var codeType = _code.GetType();

            // 枚举名称
            code = (int)Enum.Parse(codeType, codeName);

            // 为空就从枚举字段去默认说明
            if (string.IsNullOrWhiteSpace(_message))
            {
                var fieldinfo = codeType.GetField(codeName);

                // 获取默认说明
                var description = Attribute.GetCustomAttribute(fieldinfo, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (description != null)
                {
                    message = description.Description;
                }
            }
            // 取传入的说明
            else
            {
                message = _message;
            }

            // 设置全球化
            if (l != null && !string.IsNullOrWhiteSpace(message))
            {
                try
                {
                    var localization_message = l[message, arguments];

                    message = localization_message;
                }
                catch
                {

                }
            }
        }
    }
}