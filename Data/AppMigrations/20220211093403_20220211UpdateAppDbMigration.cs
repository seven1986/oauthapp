using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Data.AppMigrations
{
    public partial class _20220211UpdateAppDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "PropertySettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "PropertySettings");
        }
    }
}
