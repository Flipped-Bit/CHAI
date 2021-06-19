using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CHAI.Migrations
{
    public partial class Init : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Application = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    UserID = table.Column<string>(type: "TEXT", nullable: true),
                    OAuthToken = table.Column<string>(type: "TEXT", maxLength: 24, nullable: true),
                    GlobalCooldown = table.Column<int>(type: "INTEGER", nullable: false),
                    GlobalCooldownUnit = table.Column<int>(type: "INTEGER", nullable: false),
                    GlobalLastTriggered = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LoggingEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Triggers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BitsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    BitsCondition = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumBits = table.Column<int>(type: "INTEGER", nullable: false),
                    MaximumBits = table.Column<int>(type: "INTEGER", nullable: false),
                    UserLevelEveryone = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserLevelSubs = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserLevelVips = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserLevelMods = table.Column<bool>(type: "INTEGER", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false),
                    CharAnimTriggerKeyChar = table.Column<string>(type: "TEXT", nullable: false),
                    CharAnimTriggerKeyValue = table.Column<int>(type: "INTEGER", nullable: false),
                    Cooldown = table.Column<int>(type: "INTEGER", nullable: false),
                    CooldownUnit = table.Column<int>(type: "INTEGER", nullable: false),
                    LastTriggered = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RewardName = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Triggers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Application", "GlobalCooldown", "GlobalCooldownUnit", "GlobalLastTriggered", "LoggingEnabled", "OAuthToken", "UserID", "Username" },
                values: new object[] { 1, null, 0, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, null });
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Triggers");
        }
    }
}
