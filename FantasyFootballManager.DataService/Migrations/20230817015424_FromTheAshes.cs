using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    /// <inheritdoc />
    public partial class FromTheAshes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyProsPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SportsdataId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerTeamId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    PlayerPositionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerPositions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerEligibility = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerYahooPositions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerPageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerFilename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerSquareImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerYahooId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CbsPlayerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerByeWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerOwnedAvg = table.Column<double>(type: "float", nullable: false),
                    PlayerOwnedEspn = table.Column<double>(type: "float", nullable: false),
                    PlayerOwnedYahoo = table.Column<int>(type: "int", nullable: false),
                    RankEcr = table.Column<int>(type: "int", nullable: false),
                    RankMin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankMax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankAve = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RankStd = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosRank = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyProsPlayers", x => x.PlayerId);
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: true),
                    YahooId = table.Column<int>(type: "int", nullable: true),
                    StatsId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InjuryBodyPart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FantasyDataId = table.Column<int>(type: "int", nullable: true),
                    YearsExp = table.Column<int>(type: "int", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RotoworldId = table.Column<int>(type: "int", nullable: true),
                    GsisId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewsUpdated = table.Column<long>(type: "bigint", nullable: true),
                    EspnId = table.Column<int>(type: "int", nullable: true),
                    Hashtag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    DepthChartPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchRank = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HighSchool = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepthChartOrder = table.Column<int>(type: "int", nullable: true),
                    RotowireId = table.Column<int>(type: "int", nullable: true),
                    PlayerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InjuryStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SwishId = table.Column<int>(type: "int", nullable: true),
                    SearchLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InjuryNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SportRadarId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SleeperPlayers", x => x.Id);
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FantasyPlayerKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayerTeamId = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageDraftPosition = table.Column<double>(type: "float", nullable: true),
                    AverageDraftPositionPPR = table.Column<double>(type: "float", nullable: true),
                    ByeWeek = table.Column<int>(type: "int", nullable: true),
                    LastSeasonFantasyPoints = table.Column<double>(type: "float", nullable: true),
                    ProjectedFantasyPoints = table.Column<double>(type: "float", nullable: true),
                    AuctionValue = table.Column<double>(type: "float", nullable: true),
                    AuctionValuePPR = table.Column<double>(type: "float", nullable: true),
                    AverageDraftPositionIDP = table.Column<double>(type: "float", nullable: true),
                    AverageDraftPositionRookie = table.Column<double>(type: "float", nullable: true),
                    AverageDraftPositionDynasty = table.Column<double>(type: "float", nullable: true),
                    AverageDraftPosition2QB = table.Column<double>(type: "float", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
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
