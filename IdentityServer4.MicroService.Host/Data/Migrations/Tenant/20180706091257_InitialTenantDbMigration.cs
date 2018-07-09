using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer4.MicroService.Host.Data.Migrations.Tenant
{
    public partial class InitialTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppTenants",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IdentityServerIssuerUri = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    CacheDuration = table.Column<long>(nullable: false),
                    OwnerUserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTenants", x => x.Id);
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
                        name: "FK_AppTenantClaims_AppTenants_AppTenantId",
                        column: x => x.AppTenantId,
                        principalTable: "AppTenants",
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
                        name: "FK_AppTenantHosts_AppTenants_AppTenantId",
                        column: x => x.AppTenantId,
                        principalTable: "AppTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppTenantProperties",
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
                    table.PrimaryKey("PK_AppTenantProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTenantProperties_AppTenants_AppTenantId",
                        column: x => x.AppTenantId,
                        principalTable: "AppTenants",
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
                name: "IX_AppTenantProperties_AppTenantId",
                table: "AppTenantProperties",
                column: "AppTenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppTenantClaims");

            migrationBuilder.DropTable(
                name: "AppTenantHosts");

            migrationBuilder.DropTable(
                name: "AppTenantProperties");

            migrationBuilder.DropTable(
                name: "AppTenants");
        }
    }
}
