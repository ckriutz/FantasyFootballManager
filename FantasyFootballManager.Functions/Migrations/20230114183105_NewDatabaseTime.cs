using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FantasyFootballManager_Functions.Migrations
{
    public partial class NewDatabaseTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FootballPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FantasyProsId = table.Column<int>(type: "integer", nullable: false),
                    FootballCalculatorId = table.Column<int>(type: "integer", nullable: false),
                    SportsDataIOId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<string>(type: "text", nullable: true),
                    Team = table.Column<string>(type: "text", nullable: true),
                    Bye = table.Column<int>(type: "integer", nullable: false),
                    PlayerHeadshotURL = table.Column<string>(type: "text", nullable: true),
                    FFCRank = table.Column<int>(type: "integer", nullable: false),
                    FantasyProsRank = table.Column<int>(type: "integer", nullable: false),
                    AverageDraftPositionSportsData = table.Column<double>(type: "double precision", nullable: false),
                    AverageDraftPositionFCalculator = table.Column<double>(type: "double precision", nullable: false),
                    LastSeasonFantasyPoints = table.Column<double>(type: "double precision", nullable: false),
                    ProjectedFantasyPoints = table.Column<double>(type: "double precision", nullable: false),
                    AuctionValue = table.Column<int>(type: "integer", nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnMyTeam = table.Column<bool>(type: "boolean", nullable: false),
                    LastUpdatedSportsDataIO = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedFantasyPros = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedLineups = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedFFootballCalculator = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FootballPlayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImportStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Service = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
