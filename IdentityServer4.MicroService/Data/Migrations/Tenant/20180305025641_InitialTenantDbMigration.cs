using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace IdentityServer4.MicroService.Data.Migrations.Tenant
{
    public partial class InitialTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTenant",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CacheDuration = table.Column<long>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    IdentityServerIssuerUri = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerUserId = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Theme = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppTenantClaims",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppTenantId = table.Column<long>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenantClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTenantClaims_AppTenant_AppTenantId",
                        column: x => x.AppTenantId,
                        principalTable: "AppTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppTenantHosts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppTenantId = table.Column<long>(nullable: false),
                    HostName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenantHosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTenantHosts_AppTenant_AppTenantId",
                        column: x => x.AppTenantId,
                        principalTable: "AppTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppTenantProperty",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppTenantId = table.Column<long>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenantProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTenantProperty_AppTenant_AppTenantId",
                        column: x => x.AppTenantId,
                        principalTable: "AppTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantClaims_AppTenantId",
                table: "AppTenantClaims",
                column: "AppTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantHosts_AppTenantId",
                table: "AppTenantHosts",
                column: "AppTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTenantProperty_AppTenantId",
                table: "AppTenantProperty",
                column: "AppTenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTenantClaims");

            migrationBuilder.DropTable(
                name: "AppTenantHosts");

            migrationBuilder.DropTable(
                name: "AppTenantProperty");

            migrationBuilder.DropTable(
                name: "AppTenant");
        }
    }
}
