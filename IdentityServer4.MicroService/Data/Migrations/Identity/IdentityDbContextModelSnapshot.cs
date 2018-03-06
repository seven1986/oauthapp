﻿// <auto-generated />
using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace IdentityServer4.MicroService.Data.Migrations.Identity
{
    [DbContext(typeof(IdentityDbContext))]
    partial class IdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer4.MicroService.AppRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<DateTime>("Birthday");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("Gender");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime>("LastUpdateTime");

                    b.Property<string>("Lineage")
                        .HasColumnType("sys.hierarchyid");

                    b.Property<string>("LineageIDs");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NickName");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<long>("ParentUserID");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("Remark");

                    b.Property<string>("SecurityStamp");

                    b.Property<decimal>("Stature");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("TypeIDs");

                    b.Property<Guid>("UserKey");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<decimal>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserLogin", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<long>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserRole", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<long>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserToken", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserApiResource", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ApiResourceId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserApiResources");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserClient", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ClientId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClients");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserDistribution", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Commission");

                    b.Property<DateTime>("CommissionLastUpdate");

                    b.Property<decimal>("CommissionLv1");

                    b.Property<DateTime>("CommissionLv1LastUpdate");

                    b.Property<decimal>("CommissionLv2");

                    b.Property<DateTime>("CommissionLv2LastUpdate");

                    b.Property<decimal>("CommissionLv3");

                    b.Property<DateTime>("CommissionLv3LastUpdate");

                    b.Property<decimal>("Earned");

                    b.Property<decimal>("EarnedDiff");

                    b.Property<DateTime>("EarnedDiffLastUpdate");

                    b.Property<long>("Members");

                    b.Property<DateTime>("MembersLastUpdate");

                    b.Property<decimal>("Sales");

                    b.Property<DateTime>("SalesLastUpdate");

                    b.Property<long>("TenantId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserDistribution");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserFile", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FileType");

                    b.Property<string>("Files");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserFiles");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserTenant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("TenantId");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserTenants");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppRoleClaim", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserClaim", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserLogin", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserRole", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AppUserToken", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserApiResource", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("ApiResources")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserClient", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Clients")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserDistribution", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Distributions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserFile", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Files")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.AspNetUserTenant", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.AppUser")
                        .WithMany("Tenants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}