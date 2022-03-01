using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Tenant.Migrations
{
    public partial class _2022012003UpdateTenantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "PropertyTemplates",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "PropertyTemplates");
        }
    }
}
