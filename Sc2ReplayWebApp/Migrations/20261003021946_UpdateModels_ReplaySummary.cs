using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sc2ReplayWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels_ReplaySummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaseBuild",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DataBuild",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DataVersion",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GameVersion",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseBuild",
                table: "Replays");

            migrationBuilder.DropColumn(
                name: "DataBuild",
                table: "Replays");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                table: "Replays");

            migrationBuilder.DropColumn(
                name: "GameVersion",
                table: "Replays");
        }
    }
}
