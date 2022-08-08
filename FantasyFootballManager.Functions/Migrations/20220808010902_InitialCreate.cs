using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballManager_Functions.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FootballPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FantasyProsId = table.Column<int>(type: "int", nullable: false),
                    FootballCalculatorId = table.Column<int>(type: "int", nullable: false),
                    SportsDataIOId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bye = table.Column<int>(type: "int", nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    PlayerHeadshotURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FFCRank = table.Column<int>(type: "int", nullable: false),
                    FantasyProsRank = table.Column<int>(type: "int", nullable: false),
                    LineupsRank = table.Column<int>(type: "int", nullable: false),
                    AverageDraftPositionSportsData = table.Column<double>(type: "float", nullable: false),
                    AverageDraftPositionFCalculator = table.Column<double>(type: "float", nullable: false),
                    AverageDraftPositionLineups = table.Column<double>(type: "float", nullable: false),
                    LastSeasonFantasyPoints = table.Column<double>(type: "float", nullable: false),
                    ProjectedFantasyPoints = table.Column<double>(type: "float", nullable: false),
                    ProjectedFantasyPointsLineups = table.Column<double>(type: "float", nullable: false),
                    AuctionValue = table.Column<int>(type: "int", nullable: false),
                    LineupsAuctionValue = table.Column<double>(type: "float", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsOnMyTeam = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdatedSportsDataIO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedFantasyPros = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedLineups = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedFFootballCalculator = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FootballPlayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportStatuses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FootballPlayers");

            migrationBuilder.DropTable(
                name: "ImportStatuses");
        }
    }
}
