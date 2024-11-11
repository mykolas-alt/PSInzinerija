using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSInzinerija1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateToHighScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "record_date",
                table: "high_scores",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "record_date",
                table: "high_scores");
        }
    }
}
