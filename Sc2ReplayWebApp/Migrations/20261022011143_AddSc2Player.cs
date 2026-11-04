using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sc2ReplayWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSc2Player : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "BattleNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sc2PlayerName",
                table: "BattleNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sc2ProfileId",
                table: "BattleNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sc2RealmId",
                table: "BattleNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sc2RegionId",
                table: "BattleNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "BattleNetUsers");

            migrationBuilder.DropColumn(
                name: "Sc2PlayerName",
                table: "BattleNetUsers");

            migrationBuilder.DropColumn(
                name: "Sc2ProfileId",
                table: "BattleNetUsers");

            migrationBuilder.DropColumn(
                name: "Sc2RealmId",
                table: "BattleNetUsers");

            migrationBuilder.DropColumn(
                name: "Sc2RegionId",
                table: "BattleNetUsers");
        }
    }
}
