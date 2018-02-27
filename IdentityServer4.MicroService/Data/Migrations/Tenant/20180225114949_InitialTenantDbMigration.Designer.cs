﻿// <auto-generated />
using IdentityServer4.MicroService.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace IdentityServer4.MicroService.Data.Migrations.Tenant
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20180225114949_InitialTenantDbMigration")]
    partial class InitialTenantDbMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenant", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdminSite");

                    b.Property<long>("CacheDuration");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<string>("EnterpriseEmail");

                    b.Property<string>("IdentityServerIssuerUri");

                    b.Property<string>("Keywords");

                    b.Property<DateTime>("LastUpdateTime");

                    b.Property<string>("Name");

                    b.Property<long>("OwnerUserId");

                    b.Property<string>("PortalSite");

                    b.Property<int>("Status");

                    b.Property<string>("Summary");

                    b.Property<string>("Theme");

                    b.Property<string>("WebSite");

                    b.HasKey("Id");

                    b.ToTable("AppTenant");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenantClaim", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AppTenantId");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.HasKey("Id");

                    b.HasIndex("AppTenantId");

                    b.ToTable("AppTenantClaims");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenantHost", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AppTenantId");

                    b.Property<string>("HostName");

                    b.HasKey("Id");

                    b.HasIndex("AppTenantId");

                    b.ToTable("AppTenantHosts");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenantProperty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AppTenantId");

                    b.Property<string>("Key");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("AppTenantId");

                    b.ToTable("AppTenantProperty");
                });

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenantClaim", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.Tenant.AppTenant")
                        .WithMany("Claims")
                        .HasForeignKey("AppTenantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenantHost", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.Tenant.AppTenant")
                        .WithMany("Hosts")
                        .HasForeignKey("AppTenantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("IdentityServer4.MicroService.Tenant.AppTenantProperty", b =>
                {
                    b.HasOne("IdentityServer4.MicroService.Tenant.AppTenant")
                        .WithMany("Properties")
                        .HasForeignKey("AppTenantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}