using Dapper;
using OAuthApp.Tenant;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OAuthApp.Data
{
    //dotnet ef migrations add InitialApiMigration -c ApiDbContext -o Data/ApiMigrations
    //dotnet ef migrations add {datetime}UpdateApiMigration -c ApiDbContext -o Data/ApiMigrations
    //更新 dotnet ef database update -c ApiDbContext
    //回滚 dotnet ef migrations remove -c ApiDbContext
    public class ApiDbContext : DbContext
    {
        private readonly TenantDbCreator _dbCreator;

        public ApiDbContext(
            DbContextOptions<ApiDbContext> options,
            TenantDbCreator dbCreator)
         : base(options)
        {
            _dbCreator = dbCreator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_dbCreator.DBConnection);

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Api> Apis { get; set; }

        public DbSet<ApiSubscriber> ApiSubscribers { get; set; }

        public DbSet<ApiCodeGenerator> ApiCodeGenerators { get; set; }

        public DbSet<CodeGenerator> CodeGenerators { get; set; }

        public int Execute(string sql, object param = null)
        {
            using var con = new SqliteConnection(Database.GetConnectionString());

            return con.Execute(sql, param);
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

    public class Api
    {
        [Key]
        public long ID { get; set; }
        public long UserID { get; set; }
        public string ApiKey { get; set; } = Guid.NewGuid().ToString("n");
        public string Name { get; set; }
        public string Tags { get; set; }
        public string Summary { get; set; }
        public string WebSite { get; set; }
        public string LogoUri { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "BOOLEAN")]
        public bool Show { get; set; }
        public int Sort { get; set; }
        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class ApiSubscriber
    {
        [Key]
        public long ID { get; set; }
        public long ApiID { get; set; }
        public long ApiCodeGeneratorID { get; set; }
        public long UserID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }

    public class ApiCodeGenerator
    {
        [Key]
        public long ID { get; set; }
        public long ApiID { get; set; }
        public long CodeGeneratorID { get; set; }
    }

    public class CodeGenerator
    {
        [Key]
        public long ID { get; set; }
        public long UserID { get; set; }
        public string LogoUri { get; set; }
        public string Name { get; set; }
        public string SourceUri { get; set; }
        public string SourceCode { get; set; }
        public string Tags { get; set; }
        public string Version { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "BOOLEAN")]
        public bool Share { get; set; } = true;

        [Column(TypeName = "BOOLEAN")]
        public bool Show { get; set; } = true;

        [Column(TypeName = "DATETIME")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Column(TypeName = "DATETIME")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        [Column(TypeName = "BOOLEAN")]
        public bool IsDelete { get; set; } = false;
    }
}