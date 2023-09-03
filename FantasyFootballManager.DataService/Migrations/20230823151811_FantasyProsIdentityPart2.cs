using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    /// <inheritdoc />
    public partial class FantasyProsIdentityPart2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_FantasyProsPlayers",
            //    table: "FantasyProsPlayers");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "FantasyProsPlayers",
                newName: "PlayerIdNew");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FantasyProsPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FantasyProsPlayers",
                table: "FantasyProsPlayers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_FantasyProsPlayers",
            //    table: "FantasyProsPlayers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FantasyProsPlayers");

            migrationBuilder.RenameColumn(
                name: "PlayerIdNew",
                table: "FantasyProsPlayers",
                newName: "PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FantasyProsPlayers",
                table: "FantasyProsPlayers",
                column: "PlayerId");
        }
    }
}
