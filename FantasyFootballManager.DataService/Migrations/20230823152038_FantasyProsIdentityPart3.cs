using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    /// <inheritdoc />
    public partial class FantasyProsIdentityPart3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerIdNew",
                table: "FantasyProsPlayers",
                newName: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "FantasyProsPlayers",
                newName: "PlayerIdNew");
        }
    }
}
