using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;

namespace OAuthApp.Tenant
{
    //dotnet tool update -g dotnet-ef
    //dotnet ef migrations add InitialTenantDbMigration -c TenantDbContext -o Tenant/Migrations
    //dotnet ef migrations add {datetime}UpdateTenantDbMigration -c TenantDbContext -o Tenant/Migrations
    //dotnet ef database update -c TenantDbContext
    //回滚 dotnet ef migrations remove -c TenantDbContext
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

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<TenantHost> TenantHosts { get; set; }

        public DbSet<TenantClaim> TenantClaims { get; set; }

        public DbSet<PropertyTemplate> PropertyTemplates { get; set; }

        public DbSet<TenantProperty> TenantProperties { get; set; }

        public DbSet<AuthScheme> AuthSchemes { get; set; }

        public DbSet<TenantBlob> TenantBlobs { get; set; }

        public DbSet<TenantServer> TenantServers { get; set; }

        public DbSet<TenantOrder> TenantOrders { get; set; }

        public DbSet<QRCodeSignIn> QRCodeSignIns { get; set; }

        public async Task<object> ExecuteScalarAsync(string sql, params DbParameter[] sqlParams)
        {
            var con = Database.GetDbConnection();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            using var cmd = con.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddRange(sqlParams);

            return await cmd.ExecuteScalarAsync();
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] sqlParams)
        {
            var con = Database.GetDbConnection();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            using var cmd = con.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddRange(sqlParams);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<DbDataReader> ExecuteReaderAsync(string sql, params DbParameter[] sqlParams)
        {
            var con = Database.GetDbConnection();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            using var cmd = con.CreateCommand();
            cmd.CommandText = sql;

            cmd.Parameters.AddRange(sqlParams);

            return await cmd.ExecuteReaderAsync();
        }

        public List<T> Query<T>(string sql, object param = null)
        {
            using var con = new SqliteConnection(Database.GetConnectionString());
            return con.Query<T>(sql, param).ToList();
        }

        public T QueryFirstOrDefault<T>(string sql, object param = null)
        {
            using var con = new SqliteConnection(Database.GetConnectionString());
            return con.QueryFirstOrDefault<T>(sql, param);
        }
    }

    public class AuthScheme
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }

        public long TenantID { get; set; }

        public string Scheme { get; set; }

        public string DisplayName { get; set; }

        public string Remark { get; set; }

        public string WebHookUrl { get; set; }

        public string WebHookToken { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    
    public class Tenant
    {
        [Key]
        public long ID { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 风格
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// ClaimsIssuer
        /// </summary>
        public string ClaimsIssuer { get; set; }

        /// <summary>
        /// 租户数据缓存时长，单位秒
        /// </summary>
        public long CacheDuration { get; set; } = 5;

        /// <summary>
        /// 管理员 用户ID
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// 管理员 用户主域名
        /// </summary>
        public string OwnerHost { get; set; }

        /// <summary>
        /// Logo
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据库占用空间
        /// </summary>
        public long DatabaseSize { get; set; } = 0;

        public long DatabaseMaxSize { get; set; } = 1 * 1024 * 1024;

        public long AppServerMaxSize { get; set; } = 1 * 1024 * 1024;

        public long BlobServerMaxSize { get; set; } = 1 * 1024 * 1024;

        public long ReleaseServerMaxSize { get; set; } = 1 * 1024 * 1024;

        public string WebHookUri { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    
    public class TenantHost
    {
        [Key]
        public long ID { get; set; }

        public long TenantID { get; set; }

        public string HostName { get; set; }
    }

    public class TenantClaim
    {
        [Key]
        public long ID { get; set; }

        public long TenantID { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public class TenantProperty
    {
        [Key]
        public long ID { get; set; }

        public long TenantID { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class TenantBlob
    {
        [Key]
        public long ID { get; set; }

        public long TenantID { get; set; }

        public string ChannelCode { get; set; }

        public string ChannelAppID { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Tag { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class PropertyTemplate
    {
        public long ID { get; set; }

        public long TenantID { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; } = "text";

        public string ValueDefaultItems { get; set; }

        public string Description { get; set; }

        public string Tag { get; set; }

        public string ChannelCode { get; set; }


        [Column(TypeName = "BOOLEAN")]
        public bool IsSystem { get; set; } = false;

        [Column(TypeName = "BOOLEAN")]
        public bool Show { get; set; } = true;

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class TenantServer
    {
        [Key]
        public long ID { get; set; }

        public long TenantID { get; set; }

        public long UserID { get; set; }

        public string Tag { get; set; }

        public string ServerName { get; set; }

        public string Summary { get; set; }

        public int Sort { get; set; }

        public string ServerUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RootFolder { get; set; }

        public string WebSiteUrl { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class TenantOrder
    {
        [Key]
        public long ID { get; set; }

        public long TenantID { get; set; }

        public string ChannelCode { get; set; }

        public string ChannelAppID { get; set; }

        public long Amount { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class QRCodeSignIn
    {
        [Key]
        public long ID { get; set; }

        public string SignInKey { get; set; } = Guid.NewGuid().ToString("n");

        public long TenantID { get; set; }

        public string ChannelCode { get; set; }

        public string ChannelAppID { get; set; }

        public long AppUserID { get; set; }

        public string Status { get; set; } = QRCodeSignInStatus.PreSignIn;

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }

    public static class QRCodeSignInStatus
    {
        public const string PreSignIn = "PreSignIn";
        public const string Scaned = "Scaned";
        public const string OK = "OK";
    }
}
