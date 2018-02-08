using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.CommonModels
{
    public class PagingRequest<T> where T : new()
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
        public bool asc { get; set; }

        [Range(0, long.MaxValue)]
        [DefaultValue(0)]
        public int skip { get; set; } = 0;

        [Range(10, 2000)]
        [DefaultValue(10)]
        public int take { get; set; } = 10;
    }
}
