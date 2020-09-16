using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.Common
{
    public class PagingRequest<T> : IPagingRequest where T : new()
    {
        /// <summary>
        /// 查询实体
        /// </summary>
        public T q { get; set; } = new T();

        /// <summary>
        /// 排序字段
        /// </summary>
        public virtual string orderby { get; set; }

        /// <summary>
        /// 正序或倒序
        /// </summary>
        [DefaultValue(false)]
        public bool? asc { get; set; } = false;

        /// <summary>
        /// 跳过的数据条数
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int? skip { get; set; } = 0;

        /// <summary>
        /// 将获取的数据条数
        /// </summary>
        [Range(0, int.MaxValue)]
        [DefaultValue(10)]
        public int? take { get; set; } = 10;

        /// <summary>
        /// 开始时间。yyyy-MM-dd
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// 结束时间。yyyy-MM-dd
        /// </summary>
        public DateTime? endTime { get; set; }
    }
}
