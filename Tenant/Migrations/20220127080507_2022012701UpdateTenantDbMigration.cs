using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Tenant.Migrations
{
    public partial class _2022012701UpdateTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QRCodeSignIns",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SignInKey = table.Column<string>(type: "TEXT", nullable: true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    ChannelCode = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelAppID = table.Column<string>(type: "TEXT", nullable: true),
                    AppUserID = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodeSignIns", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QRCodeSignIns");
        }
    }
}
