using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuthApp.Data
{
    /// <summary>
    /// 数据缓存表
    /// </summary>
    [Table("AppCache")]
    public class AppCache
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [Column(TypeName = "nvarchar(449)")]
        public string Id { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [Column(TypeName = "varbinary(max)")]
        public string Value { get; set; }

        /// <summary>
        /// ExpiresAtTime
        /// </summary>
        [Column(TypeName = "datetimeoffset(7)")]
        public DateTimeOffset ExpiresAtTime { get; set; }

        /// <summary>
        /// SlidingExpirationInSeconds
        /// </summary>
        public long SlidingExpirationInSeconds { get; set; }

        /// <summary>
        /// AbsoluteExpiration
        /// </summary>
        [Column(TypeName = "datetimeoffset(7)")]
        public DateTimeOffset AbsoluteExpiration { get; set; }
    }
}
