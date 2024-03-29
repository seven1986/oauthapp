﻿// <auto-generated />
using System;
using OAuthApp.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OAuthApp.Tenant.Migrations
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20220120040043_2022012003UpdateTenantDbMigration")]
    partial class _2022012003UpdateTenantDbMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("H5App.Tenant.AuthScheme", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Remark")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scheme")
                        .HasColumnType("TEXT");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WebHookToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("WebHookUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("AuthSchemes");
                });

            modelBuilder.Entity("H5App.Tenant.PropertyTemplate", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChannelCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<bool>("IsSystem")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Show")
                        .HasColumnType("BOOLEAN");

                    b.Property<string>("Tag")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.Property<string>("ValueDefaultItems")
                        .HasColumnType("TEXT");

                    b.Property<string>("ValueType")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("PropertyTemplates");
                });

            modelBuilder.Entity("H5App.Tenant.Tenant", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("AppServerMaxSize")
                        .HasColumnType("INTEGER");

                    b.Property<long>("BlobServerMaxSize")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CacheDuration")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimsIssuer")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<long>("DatabaseMaxSize")
                        .HasColumnType("INTEGER");

                    b.Property<long>("DatabaseSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("LogoUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("OwnerHost")
                        .HasColumnType("TEXT");

                    b.Property<long>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ReleaseServerMaxSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Theme")
                        .HasColumnType("TEXT");

                    b.Property<string>("WebHookUri")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("H5App.Tenant.TenantBlob", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChannelAppID")
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tag")
                        .HasColumnType("TEXT");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("TenantBlobs");
                });

            modelBuilder.Entity("H5App.Tenant.TenantClaim", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("TenantClaims");
                });

            modelBuilder.Entity("H5App.Tenant.TenantHost", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("HostName")
                        .HasColumnType("TEXT");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("TenantHosts");
                });

            modelBuilder.Entity("H5App.Tenant.TenantOrder", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Amount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChannelAppID")
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("TenantOrders");
                });

            modelBuilder.Entity("H5App.Tenant.TenantProperty", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("TenantProperties");
                });

            modelBuilder.Entity("H5App.Tenant.TenantServer", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("RootFolder")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerUrl")
                        .HasColumnType("TEXT");

                    b.Property<int>("Sort")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Summary")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tag")
                        .HasColumnType("TEXT");

                    b.Property<long>("TenantID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("WebSiteUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("TenantServers");
                });
#pragma warning restore 612, 618
        }
    }
}
