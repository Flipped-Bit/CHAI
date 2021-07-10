using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CHAI.Migrations
{
    public partial class AddDurationPropertiesToTriggerTable : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivateAt",
                table: "Triggers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Triggers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationUnit",
                table: "Triggers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasDeactivationTime",
                table: "Triggers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc/>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivateAt",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "DurationUnit",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "HasDeactivationTime",
                table: "Triggers");
        }
    }
}
