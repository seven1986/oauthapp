using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAuthApp.Data.AppMigrations
{
    public partial class InitialAppDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppChatMessages",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppID = table.Column<long>(type: "INTEGER", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", nullable: true),
                    OpenID = table.Column<string>(type: "TEXT", nullable: true),
                    NickName = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    Platform = table.Column<string>(type: "TEXT", nullable: true),
                    MessageType = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    Remark = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    ShowIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChatMessages", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AppRanks",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppID = table.Column<long>(type: "INTEGER", nullable: false),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    RankKey = table.Column<string>(type: "TEXT", nullable: true),
                    UnionID = table.Column<string>(type: "TEXT", nullable: true),
                    Platform = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    NickName = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<long>(type: "INTEGER", nullable: false),
                    MaximumScore = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRanks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    ProjectID = table.Column<long>(type: "INTEGER", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: true),
                    ServerPath = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                    AppKey = table.Column<string>(type: "TEXT", nullable: true),
                    Share = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppID = table.Column<long>(type: "INTEGER", nullable: false),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    Platform = table.Column<string>(type: "TEXT", nullable: true),
                    UnionID = table.Column<string>(type: "TEXT", nullable: true),
                    NickName = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EmailIsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneIsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AppVersions",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppID = table.Column<long>(type: "INTEGER", nullable: false),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    AppServerID = table.Column<long>(type: "INTEGER", nullable: false),
                    Ver = table.Column<string>(type: "TEXT", nullable: true),
                    Tag = table.Column<string>(type: "TEXT", nullable: true),
                    PackageBackupUri = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MarketTags",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    ChannelCode = table.Column<string>(type: "TEXT", nullable: true),
                    ShowFlag = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketTags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Logo = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PropertySettings",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelCode = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelAppId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertySettings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserAppServers",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    AppServerID = table.Column<long>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppServers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<long>(type: "INTEGER", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    NickName = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    EmailIsValid = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneIsValid = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    IsDelete = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppChatMessages");

            migrationBuilder.DropTable(
                name: "AppRanks");

            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "AppVersions");

            migrationBuilder.DropTable(
                name: "MarketTags");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "PropertySettings");

            migrationBuilder.DropTable(
                name: "UserAppServers");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
