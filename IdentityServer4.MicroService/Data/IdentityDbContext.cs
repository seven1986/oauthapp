using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Data
{
    public class IdentityDbContext : IdentityDbContext<AppUser, AppRole, long, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            #region Claims
            builder.Entity<AppUser>()
                    .HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Logins
            builder.Entity<AppUser>()
                    .HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Roles
            builder.Entity<AppUser>()
                    .HasMany(e => e.Roles)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Tokens
            builder.Entity<AppUser>()
                    .HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Tenants
            builder.Entity<AppUser>()
                    .HasMany(e => e.Tenants)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Files
            builder.Entity<AppUser>()
                    .HasMany(e => e.Files)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Clients
            builder.Entity<AppUser>()
                    .HasMany(e => e.Clients)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region ApiResources
            builder.Entity<AppUser>()
                    .HasMany(e => e.ApiResources)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Distributors
            builder.Entity<AppUser>()
                    .HasMany(e => e.Distributors)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Properties
            builder.Entity<AppUser>()
                    .HasMany(e => e.Properties)
                    .WithOne()
                    .HasForeignKey(e => e.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            #endregion

            builder.Entity<AppRole>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<AspNetUserClient> UserClients { get; set; }

        public DbSet<AspNetUserFile> UserFiles { get; set; }

        public DbSet<AspNetUserApiResource> UserApiResources { get; set; }

        public DbSet<AspNetUserDistributor> AspNetUserDistributor { get; set; }

        public DbSet<AspNetUserTenant> UserTenants { get; set; }

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

            //if (con.State != ConnectionState.Open)
            //{
            //    con.Open();
            //}

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
}
