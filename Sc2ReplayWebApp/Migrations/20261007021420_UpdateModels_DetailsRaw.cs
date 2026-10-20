using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sc2ReplayWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels_DetailsRaw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailsRaw",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsRaw",
                table: "Replays");
        }
    }
}
