using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    /// <inheritdoc />
    public partial class FantasyPlayerInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FantasyPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsThumbsUp = table.Column<bool>(type: "bit", nullable: false),
                    IsThumbsDown = table.Column<bool>(type: "bit", nullable: false),
                    IsTaken = table.Column<bool>(type: "bit", nullable: false),
                    IsOnMyTeam = table.Column<bool>(type: "bit", nullable: false),
                    PickedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickNumber = table.Column<int>(type: "int", nullable: true),
                    PickRound = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyPlayers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FantasyPlayers");
        }
    }
}
