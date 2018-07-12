using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Tenant
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
           : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AppTenant> Tenants { get; set; }

        public DbSet<AppTenantHost> TenantHosts { get; set; }

        public async Task<object> ExecuteScalarAsync(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] sqlParams)
        {
            var con = Database.GetDbConnection();

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == System.Data.ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;

                cmd.Parameters.AddRange(sqlParams);

                return await cmd.ExecuteScalarAsync();
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] sqlParams)
        {
            var con = Database.GetDbConnection();

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == System.Data.ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;

                cmd.Parameters.AddRange(sqlParams);

                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<DbDataReader> ExecuteReaderAsync(string sql, CommandType cmdType = CommandType.Text, params SqlParameter[] sqlParams)
        {
            var con = Database.GetDbConnection();

            if (con.State == System.Data.ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == System.Data.ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;

                cmd.Parameters.AddRange(sqlParams);

                return await cmd.ExecuteReaderAsync();
            }
        }
    }

    /// <summary>
    /// 租户默认属性集合
    /// </summary>
    public class TenantDefaultProperty
    {
        /// <summary>
        /// 官方网站
        /// </summary>
        public const string WebSite = "WebSite";

        /// <summary>
        /// 开放平台
        /// </summary>
        public const string PortalSite = "PortalSite";

        /// <summary>
        /// 运营中心
        /// </summary>
        public const string AdminSite = "AdminSite";

        /// <summary>
        /// 关键字
        /// </summary>
        public const string Keywords = "Keywords";

        /// <summary>
        /// 描述
        /// </summary>
        public const string Summary = "Summary";

        /// <summary>
        /// 介绍
        /// </summary>
        public const string Description = "Description";

        /// <summary>
        /// 企业邮箱
        /// </summary>
        public const string EnterpriseEmail = "EnterpriseEmail";

        /// <summary>
        /// 站点统计代码
        /// </summary>
        public const string Tracking = "Tracking";

        /// <summary>
        /// 网站图标
        /// </summary>
        public const string Favicon = "Favicon";
    }

    /// <summary>
    /// 租户实体
    /// </summary>
    [Table("AppTenants")]
    public class AppTenant
    {
        public long Id { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 风格
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TenantStatus Status { get; set; }

        /// <summary>
        /// IdentityServer IssuerUri
        /// </summary>
        public string IdentityServerIssuerUri { get; set; }

        /// <summary>
        /// 域名集合
        /// </summary>
        public virtual List<AppTenantHost> Hosts { get; } = new List<AppTenantHost>();

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 声明
        /// </summary>
        public virtual List<AppTenantClaim> Claims { get; } = new List<AppTenantClaim>();

        /// <summary>
        /// 租户数据缓存时长，单位秒
        /// </summary>
        public long CacheDuration { get; set; } = 3600L;

        /// <summary>
        /// 所有者用户Id
        /// </summary>
        public long OwnerUserId { get; set; }

        /// <summary>
        /// 属性集合
        /// </summary>
        public virtual List<AppTenantProperty> Properties { get; } = new List<AppTenantProperty>();
    }

    [Table("AppTenantHosts")]
    public class AppTenantHost
    {
        public long Id { get; set; }

        public long AppTenantId { get; set; }

        public string HostName { get; set; }
    }

    [Table("AppTenantClaims")]
    public class AppTenantClaim
    {
        public long Id { get; set; }

        public long AppTenantId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    [Table("AppTenantProperties")]
    public class AppTenantProperty
    {
        public long Id { get; set; }

        public long AppTenantId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public enum TenantStatus
    {
        Created = 0,
        Enable = 1,
        Disable = 2
    }
}
