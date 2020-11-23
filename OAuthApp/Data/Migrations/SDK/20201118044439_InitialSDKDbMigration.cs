using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Data.Migrations.SDK
{
    public partial class InitialSDKDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SdkPackags",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    WebSite = table.Column<string>(nullable: true),
                    LogoUri = table.Column<string>(nullable: true),
                    PackageName = table.Column<string>(nullable: true),
                    SwaggerUri = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UserID = table.Column<long>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Enable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SdkPackags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SdkReleaseHistories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<long>(nullable: false),
                    SdkPackageId = table.Column<long>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SdkReleaseHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SdkGenerators",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SdkPackageId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Enable = table.Column<bool>(nullable: false),
                    CompiledCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SdkGenerators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SdkGenerators_SdkPackags_SdkPackageId",
                        column: x => x.SdkPackageId,
                        principalTable: "SdkPackags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SdkSubscribers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SdkPackageId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Acator = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Enable = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SdkSubscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SdkSubscribers_SdkPackags_SdkPackageId",
                        column: x => x.SdkPackageId,
                        principalTable: "SdkPackags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SdkGenerators_SdkPackageId",
                table: "SdkGenerators",
                column: "SdkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SdkSubscribers_SdkPackageId",
                table: "SdkSubscribers",
                column: "SdkPackageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SdkGenerators");

            migrationBuilder.DropTable(
                name: "SdkReleaseHistories");

            migrationBuilder.DropTable(
                name: "SdkSubscribers");

            migrationBuilder.DropTable(
                name: "SdkPackags");
        }
    }
}
