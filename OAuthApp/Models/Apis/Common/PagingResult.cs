using OAuthApp.Enums;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OAuthApp.Models.Apis.Common
{
    public class PagingResult<T>: IPagingResult where T : class
    {
        public int take { get; set; }

        public int skip { get; set; }

        public int code { get; set; } = (int)BasicControllerEnums.Status200OK;

        public string codeName { get; set; } = BasicControllerEnums.Status200OK.ToString();

        public int total { get; set; }

        public string message { get; set; }

        public IList<T> data { get; set; } = new List<T>();

        public PagingResult() { }

        public PagingResult(IList<T> _data, int _total, int _skip, int _take)
        {
            data = _data;
            skip = _skip;
            take = _take;
            total = _total;
            code = (int)BasicControllerEnums.Status200OK;
        }

        public PagingResult(IStringLocalizer l, Enum _code, string _message = "", params object[] arguments)
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
