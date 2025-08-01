using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Auth0Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    YahooUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    YahooLeagueId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EspnUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EspnLeagueId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SleeperUsername = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SleeperLeagueId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
