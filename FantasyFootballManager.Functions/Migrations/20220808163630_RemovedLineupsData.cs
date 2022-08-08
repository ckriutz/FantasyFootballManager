using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballManager_Functions.Migrations
{
    public partial class RemovedLineupsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageDraftPositionLineups",
                table: "FootballPlayers");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "FootballPlayers");

            migrationBuilder.DropColumn(
                name: "LineupsAuctionValue",
                table: "FootballPlayers");

            migrationBuilder.DropColumn(
                name: "LineupsRank",
                table: "FootballPlayers");

            migrationBuilder.DropColumn(
                name: "ProjectedFantasyPointsLineups",
                table: "FootballPlayers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageDraftPositionLineups",
                table: "FootballPlayers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Depth",
                table: "FootballPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "LineupsAuctionValue",
                table: "FootballPlayers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "LineupsRank",
                table: "FootballPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ProjectedFantasyPointsLineups",
                table: "FootballPlayers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
