using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSInzinerija1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedGameStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_stats",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "TEXT", nullable: false),
                    game_id = table.Column<int>(type: "INTEGER", nullable: false),
                    recent_score = table.Column<int>(type: "INTEGER", nullable: false),
                    mistakes = table.Column<int>(type: "INTEGER", nullable: true),
                    recent_time = table.Column<TimeSpan>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_stats", x => new { x.user_id, x.game_id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_stats");
        }
    }
}
