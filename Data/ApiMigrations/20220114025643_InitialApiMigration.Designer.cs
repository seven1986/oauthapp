﻿// <auto-generated />
using System;
using OAuthApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OAuthApp.Data.ApiMigrations
{
    [DbContext(typeof(ApiDbContext))]
    [Migration("20220114025643_InitialApiMigration")]
    partial class InitialApiMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("H5App.Data.Api", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApiKey")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

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

                    b.Property<bool>("Show")
                        .HasColumnType("BOOLEAN");

                    b.Property<int>("Sort")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Summary")
                        .HasColumnType("TEXT");

                    b.Property<string>("SwaggerDocument")
                        .HasColumnType("TEXT");

                    b.Property<string>("SwaggerUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tags")
                        .HasColumnType("TEXT");

                    b.Property<long>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("WebSite")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Apis");
                });

            modelBuilder.Entity("H5App.Data.ApiCodeGenerator", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ApiID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CodeGeneratorID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("ApiCodeGenerators");
                });

            modelBuilder.Entity("H5App.Data.ApiSubscriber", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ApiCodeGeneratorID")
                        .HasColumnType("INTEGER");

                    b.Property<long>("ApiID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Avatar")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("BOOLEAN");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("DATETIME");

                    b.Property<string>("Mobile")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("ApiSubscribers");
                });

            modelBuilder.Entity("H5App.Data.CodeGenerator", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("DATETIME");

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

                    b.Property<bool>("Share")
                        .HasColumnType("BOOLEAN");

                    b.Property<bool>("Show")
                        .HasColumnType("BOOLEAN");

                    b.Property<string>("SourceCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("Summary")
                        .HasColumnType("TEXT");

                    b.Property<string>("Tags")
                        .HasColumnType("TEXT");

                    b.Property<long>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("CodeGenerators");
                });
#pragma warning restore 612, 618
        }
    }
}