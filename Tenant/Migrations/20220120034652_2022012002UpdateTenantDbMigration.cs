using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Tenant.Migrations
{
    public partial class _2022012002UpdateTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AppServerMaxSize",
                table: "Tenants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "BlobServerMaxSize",
                table: "Tenants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ReleaseServerMaxSize",
                table: "Tenants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                table: "TenantOrders",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppServerMaxSize",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "BlobServerMaxSize",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ReleaseServerMaxSize",
                table: "Tenants");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "TenantOrders",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");
        }
    }
}
