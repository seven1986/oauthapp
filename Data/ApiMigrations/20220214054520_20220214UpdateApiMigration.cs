using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Data.ApiMigrations
{
    public partial class _20220214UpdateApiMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SwaggerDocument",
                table: "Apis");

            migrationBuilder.DropColumn(
                name: "SwaggerUri",
                table: "Apis");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SwaggerDocument",
                table: "Apis",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwaggerUri",
                table: "Apis",
                type: "TEXT",
                nullable: true);
        }
    }
}
