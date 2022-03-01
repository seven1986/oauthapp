using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Tenant.Migrations
{
    public partial class _2022012001UpdateTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantOrders",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantID = table.Column<long>(type: "INTEGER", nullable: false),
                    ChannelCode = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelAppID = table.Column<string>(type: "TEXT", nullable: true),
                    Amount = table.Column<long>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantOrders", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantOrders");
        }
    }
}
