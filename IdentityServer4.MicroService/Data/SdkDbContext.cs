using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OAuthApp.Data
{
    public class SdkDbContext: DbContext
    {
        public SdkDbContext(DbContextOptions<SdkDbContext> options)
         : base(options)
        {
        }

        public DbSet<SdkReleaseHistory> ReleaseHistories { get; set; }

        public DbSet<SdkPackage> Packages { get; set; }

        public DbSet<SdkGenerator> Generators { get; set; }

        public DbSet<SdkSubscriber> Subscribers { get; set; }
    }

    [Table("SdkPackags")]
    public class SdkPackage
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 网站地址
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// LOGO
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// 包名称
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// swagger地址
        /// </summary>
        public string SwaggerUri { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 模板列表
        /// </summary>
        public List<SdkGenerator> SdkGenerators { get; set; }

        /// <summary>
        /// 订阅者
        /// </summary>
        public List<SdkSubscriber> SdkSubscribers { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
    }

    [Table("SdkGenerators")]
    public class SdkGenerator
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 软件包ID
        /// </summary>
        public long SdkPackageId { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模板地址
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 编译后的代码
        /// </summary>
        public string CompiledCode { get; set; }
    }

    [Table("SdkSubscribers")]
    public class SdkSubscriber
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 软件包ID
        /// </summary>
        public long SdkPackageId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Acator { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 订阅时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }

    [Table("SdkReleaseHistories")]
    public class SdkReleaseHistory
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 软件包ID
        /// </summary>
        public long SdkPackageId { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 发布日志
        /// </summary>
        public string ReleaseDate { get; set; }
    }
}
