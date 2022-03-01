using Dapper;
using OAuthApp.Tenant;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace OAuthApp.Data
{
    //dotnet tool update --global dotnet-ef

    //dotnet ef migrations add InitialAppDbMigration -c AppDbContext -o Data/AppMigrations
    //dotnet ef migrations add {datetime}UpdateAppDbMigration -c AppDbContext -o Data/AppMigrations
    //更新 dotnet ef database update -c AppDbContext
    //回滚 dotnet ef migrations remove -c AppDbContext

    public class AppDbContext : DbContext
    {
        private readonly TenantDbCreator _dbCreator;

        public AppDbContext(DbContextOptions<AppDbContext> options, TenantDbCreator dbCreator)
         : base(options)
        {
            _dbCreator = dbCreator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_dbCreator.DBConnection);

            base.OnConfiguring(optionsBuilder);
        }

        public void ExecuteReader(string sql,Action<DbDataReader> readerAction, params DbParameter[] sqlParams)
        {
            using var con = new SqliteConnection(Database.GetConnectionString());
            using var cmd = con.CreateCommand();
            cmd.CommandText = sql;

            if (sqlParams.Length > 0)
            {
                cmd.Parameters.AddRange(sqlParams);
            }

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            else if (con.State == ConnectionState.Broken)
            {
                con.Close();
                con.Open();
            }

            if (readerAction != null)
            {
                using var reader = cmd.ExecuteReader();
                readerAction.Invoke(reader);
            }
        }

        public int Execute(string sql,object param=null)
        {
            using var con = new SqliteConnection(Database.GetConnectionString());
            return con.Execute(sql, param);
        }

        public T ExecuteScalar<T>(string sql, object param)
        {
            using var con = new SqliteConnection(Database.GetConnectionString());
            return con.ExecuteScalar<T>(sql, param);
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

        #region 项目
        public DbSet<Project> Projects { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<AppRank> AppRanks { get; set; }
        public DbSet<AppChatMessage> AppChatMessages { get; set; }
        public DbSet<AppVersion> AppVersions { get; set; }
        public DbSet<AppUser> AppUsers { get; set; } 
        #endregion

        #region 用户
        public DbSet<User> Users { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }

        public DbSet<UserAppServer> UserAppServers { get; set; }
        #endregion

        #region 公共
        public DbSet<PropertySetting> PropertySettings { get; set; }

        public DbSet<MarketTag> MarketTags { get; set; }

        public DbSet<Team> Teams { get; set; }
        #endregion
    }

    #region 项目
    public class Project
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }

        public string Name { get; set; }

        public string Logo { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    public class App
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }

        public long ProjectID { get; set; }

        public string Website { get; set; }

        public string ServerPath { get; set; } = Guid.NewGuid().ToString("n");

        public string Name { get; set; }

        public string Logo { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        public string AppKey { get; set; } = Guid.NewGuid().ToString("D");

        [Column(TypeName = "BOOLEAN")]
        public bool Share { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;

        //public string PlayerAvator { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }

        //#region 微信公众号
        //public string WechatRedirectUri { get; set; }
        //public string WechatScope { get; set; }
        //public string WechatClientID { get; set; }
        //public string WechatClientSecret { get; set; }
        //#endregion

        //#region 微信开放平台
        //public string OpenWechatRedirectUri { get; set; }
        //public string OpenWechatScope { get; set; }
        //public string OpenWechatClientID { get; set; }
        //public string OpenWechatClientSecret { get; set; }
        //#endregion

        //#region 微博
        //public string WeiboRedirectUri { get; set; }
        //public string WeiboClientID { get; set; }
        //public string WeiboClientSecret { get; set; }
        //#endregion

        //#region 吉秀
        //public string JixiuClientID { get; set; }
        //public string JixiuServer { get; set; }
        //#endregion

        //#region QQ
        //public string QQRedirectUri { get; set; }
        //public string QQClientID { get; set; }
        //public string QQClientSecret { get; set; }
        //#endregion

        //#region 微信分享
        //public string ShareIcon { get; set; }
        //public string ShareTitle { get; set; }
        //public string ShareDescription { get; set; }
        //public string ShareLink { get; set; }
        //#endregion

        //#region Git存储库
        //public string RepositoryID { get; set; }
        //public string RepositoryUrl { get; set; }
        //public string RepositoryUserName { get; set; }
        //public string RepositoryPassword { get; set; }
        //#endregion

        //#region 微信小程序
        //public string WechatMiniPClientID { get; set; }
        //public string WechatMiniPClientSecret { get; set; }
        //#endregion

        //#region 微信支付
        //public string WechatPayAppID { get; set; }
        //#endregion

        //#region 小i机器人
        //public string BotAppKey { get; set; }
        //public string BotAppSecret { get; set; }
        //#endregion

        //#region 微信JSSDK默认启用的API集合
        //public string WechatJSApiList { get; set; }
        //#endregion

        //#region AppMarket
        //public bool AppMarketShow { get; set; }
        //public int AppMarketPriority { get; set; } = 1;
        //public string AppMarketTags { get; set; }
        //public string AppMarketWebSite { get; set; }
        //#endregion
        //public virtual List<AppProperty> Properties { get; } = new List<AppProperty>();
        //public virtual List<AppBlob> Blobs { get; } = new List<AppBlob>();
        //public virtual List<AppChatMessage> ChatMessages { get; } = new List<AppChatMessage>();
        //public virtual List<AppRank> Ranks { get; } = new List<AppRank>();
        //public virtual List<AppVersion> Versions { get; } = new List<AppVersion>();
    }
    public class AppChatMessage
    {
        [Key]
        public long ID { get; set; }

        public long AppID { get; set; }

        public string GroupName { get; set; }

        public string OpenID { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public string Platform { get; set; }

        public string MessageType { get; set; }

        public string Content { get; set; }

        public string Remark { get; set; }

        public string Tags { get; set; }

        public int ShowIndex { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    public class AppRank
    {
        [Key]
        public long ID { get; set; }

        public long AppID { get; set; }

        public long UserID { get; set; }

        public string RankKey { get; set; }

        public string UnionID { get; set; }

        public string Platform { get; set; }

        public string Avatar { get; set; }

        public string NickName { get; set; }

        public string Data { get; set; }

        public long Score { get; set; }

        public long MaximumScore { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    public class AppVersion
    {
        [Key]
        public long ID { get; set; }

        public long AppID { get; set; }

        public long UserID { get; set; }

        public long AppServerID { get; set; }

        public string Ver { get; set; }

        public string Tag { get; set; }

        public string PackageBackupUri { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    public class AppUser
    {
        [Key]
        public long ID { get; set; }

        public long AppID { get; set; }

        public long UserID { get; set; }

        public string Platform { get; set; }

        public string UnionID { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public string Data { get; set; }

        public string UserName { get; set; }

        public string Pwd { get; set; }

        public string Email { get; set; }
        public bool EmailIsValid { get; set; } = false;

        public string Phone { get; set; }
        public bool PhoneIsValid { get; set; } = false;

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    #endregion

    #region 公共
    public class PropertySetting
    {
        [Key]
        public long ID { get; set; }

        public string ChannelCode { get; set; }

        public long ChannelAppId { get; set; }

        public string Tag { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
    public class MarketTag
    {
        [Key]
        public long ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long UserID { get; set; }

        public int Priority { get; set; } = 1;

        public string ChannelCode { get; set; }


        [Column(TypeName = "BOOLEAN")]
        public bool ShowFlag { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; }
    }
    #endregion

    #region 用户
    public class User
    {
        [Key]
        public long ID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public string Email { get; set; }
        [Column(TypeName = "BOOLEAN")]
        public bool EmailIsValid { get; set; } = false;

        public string Phone { get; set; }
        [Column(TypeName = "BOOLEAN")]
        public bool PhoneIsValid { get; set; } = false;

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class UserAppServer
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }

        public long AppServerID { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class UserClaim
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public class Team
    {
        [Key]
        public long ID { get; set; }

        public long UserID { get; set; }

        public string ChannelCode { get; set; }

        public string ChannelAppID { get; set; }

        public string Role { get; set; }

        public string Permission { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
    #endregion
}