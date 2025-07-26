using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSQLMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    User = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    IsThumbsUp = table.Column<bool>(type: "boolean", nullable: false),
                    IsThumbsDown = table.Column<bool>(type: "boolean", nullable: false),
                    IsDraftedOnMyTeam = table.Column<bool>(type: "boolean", nullable: false),
                    IsDraftedOnOtherTeam = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyProsPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    PlayerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SportsdataId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayerTeamId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    PlayerPositionId = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    PlayerPositions = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlayerShortName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PlayerEligibility = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayerYahooPositions = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PlayerPageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PlayerFilename = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PlayerSquareImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PlayerImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PlayerYahooId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CbsPlayerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayerByeWeek = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    PlayerOwnedAvg = table.Column<double>(type: "double precision", nullable: false),
                    PlayerOwnedEspn = table.Column<double>(type: "double precision", nullable: false),
                    PlayerOwnedYahoo = table.Column<int>(type: "integer", nullable: false),
                    RankEcr = table.Column<int>(type: "integer", nullable: false),
                    RankMin = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RankMax = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RankAve = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RankStd = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PosRank = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Tier = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyProsPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FantasyProsPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SleeperPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: true),
                    YahooId = table.Column<int>(type: "integer", nullable: true),
                    StatsId = table.Column<int>(type: "integer", nullable: true),
                    Position = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    SearchFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Weight = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SearchFirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InjuryBodyPart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FantasyDataId = table.Column<int>(type: "integer", nullable: true),
                    YearsExp = table.Column<int>(type: "integer", nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RotoworldId = table.Column<int>(type: "integer", nullable: true),
                    GsisId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NewsUpdated = table.Column<long>(type: "bigint", nullable: true),
                    EspnId = table.Column<int>(type: "integer", nullable: true),
                    Hashtag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TeamId = table.Column<int>(type: "integer", nullable: true),
                    DepthChartPosition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    SearchRank = table.Column<int>(type: "integer", nullable: true),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HighSchool = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DepthChartOrder = table.Column<int>(type: "integer", nullable: true),
                    RotowireId = table.Column<int>(type: "integer", nullable: true),
                    College = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InjuryStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SwishId = table.Column<int>(type: "integer", nullable: true),
                    SearchLastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InjuryNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BirthCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SportRadarId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleeperPlayers", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_SleeperPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SportsDataIoPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FantasyPlayerKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlayerID = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PlayerTeamId = table.Column<int>(type: "integer", nullable: true),
                    Position = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    AverageDraftPosition = table.Column<double>(type: "double precision", nullable: true),
                    AverageDraftPositionPPR = table.Column<double>(type: "double precision", nullable: true),
                    ByeWeek = table.Column<int>(type: "integer", nullable: true),
                    LastSeasonFantasyPoints = table.Column<double>(type: "double precision", nullable: true),
                    ProjectedFantasyPoints = table.Column<double>(type: "double precision", nullable: true),
                    AuctionValue = table.Column<double>(type: "double precision", nullable: true),
                    AuctionValuePPR = table.Column<double>(type: "double precision", nullable: true),
                    AverageDraftPositionIDP = table.Column<double>(type: "double precision", nullable: true),
                    AverageDraftPositionRookie = table.Column<double>(type: "double precision", nullable: true),
                    AverageDraftPositionDynasty = table.Column<double>(type: "double precision", nullable: true),
                    AverageDraftPosition2QB = table.Column<double>(type: "double precision", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsDataIoPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportsDataIoPlayers_Teams_PlayerTeamId",
                        column: x => x.PlayerTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FantasyProsPlayers_TeamId",
                table: "FantasyProsPlayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SleeperPlayers_TeamId",
                table: "SleeperPlayers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsDataIoPlayers_PlayerTeamId",
                table: "SportsDataIoPlayers",
                column: "PlayerTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataStatus");

            migrationBuilder.DropTable(
                name: "FantasyActivities");

            migrationBuilder.DropTable(
                name: "FantasyProsPlayers");

            migrationBuilder.DropTable(
                name: "SleeperPlayers");

            migrationBuilder.DropTable(
                name: "SportsDataIoPlayers");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
