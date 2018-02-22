using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer4.MicroService.Data
{
    public class IdentityDbContext : IdentityDbContext<AppUser,AppRole,long,AppUserClaim,AppUserRole,AppUserLogin, AppRoleClaim, AppUserToken>
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

            #region Distributions
            builder.Entity<AppUser>()
                    .HasMany(e => e.Distributions)
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

        public DbSet<AspNetUserDistribution> UserDistributions { get; set; }

        public DbSet<AspNetUserTenant> UserTenants { get; set; }

        public object ExecuteScalar(string sql)
        {
            var con = Database.GetDbConnection();

            if (con.State != System.Data.ConnectionState.Open)
            {
                con.Open();
            }

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;

                return cmd.ExecuteScalar();
            }
        }
    }

    public class MD5PasswordHasher : IPasswordHasher<AppUser>
    {
        public string HashPassword(AppUser user, string password) => 
            md5String(password);

        public PasswordVerificationResult VerifyHashedPassword(AppUser user, string hashedPassword, string providedPassword)
        {
            if (md5String(providedPassword).Equals(hashedPassword))
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// 生成MD5
        /// </summary>
        /// <returns></returns>
        private string md5String(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(str);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        }
    }
}
