using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.MicroService.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,AppRole,long,AppUserClaim,AppUserRole,AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<AppUser>()
       .HasMany(e => e.Claims)
       .WithOne()
       .HasForeignKey(e => e.UserId)
       .IsRequired()
       .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<AppUser>()
                .HasMany(e => e.Tokens)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

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
    }
}
