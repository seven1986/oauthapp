using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Tenant.Migrations
{
    public partial class InitialTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthSchemes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    Scheme = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Remark = table.Column<string>(type: "TEXT", nullable: true),
                    WebHookUrl = table.Column<string>(type: "TEXT", nullable: true),
                    WebHookToken = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthSchemes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTemplates",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    ValueType = table.Column<string>(type: "TEXT", nullable: true),
                    ValueDefaultItems = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Tag = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelCode = table.Column<string>(type: "TEXT", nullable: true),
                    Show = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTemplates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenantBlobs",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    ChannelCode = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelAppID = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    Tag = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantBlobs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenantClaims",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantClaims", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenantHosts",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    HostName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantHosts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenantProperties",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProperties", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Theme = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimsIssuer = table.Column<string>(type: "TEXT", nullable: true),
                    CacheDuration = table.Column<long>(type: "INTEGER", nullable: false),
                    OwnerId = table.Column<long>(type: "INTEGER", nullable: false),
                    OwnerHost = table.Column<string>(type: "TEXT", nullable: true),
                    LogoUri = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    DatabaseSize = table.Column<long>(type: "INTEGER", nullable: false),
                    DatabaseMaxSize = table.Column<long>(type: "INTEGER", nullable: false),
                    WebHookUri = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TenantServers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    Tag = table.Column<string>(type: "TEXT", nullable: true),
                    ServerName = table.Column<string>(type: "TEXT", nullable: true),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    Sort = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerUrl = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    RootFolder = table.Column<string>(type: "TEXT", nullable: true),
                    WebSiteUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantServers", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthSchemes");

            migrationBuilder.DropTable(
                name: "PropertyTemplates");

            migrationBuilder.DropTable(
                name: "TenantBlobs");

            migrationBuilder.DropTable(
                name: "TenantClaims");

            migrationBuilder.DropTable(
                name: "TenantHosts");

            migrationBuilder.DropTable(
                name: "TenantProperties");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "TenantServers");
        }
    }
}
