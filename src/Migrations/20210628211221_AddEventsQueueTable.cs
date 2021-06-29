using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CHAI.Migrations
{
    public partial class AddEventsQueueTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventQueue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TriggeredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TriggerId = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventQueue", x => x.Id);
                });
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventQueue");
        }
    }
}
