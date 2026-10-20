using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sc2ReplayWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels_DetailsHex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailsHex",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsHex",
                table: "Replays");
        }
    }
}
