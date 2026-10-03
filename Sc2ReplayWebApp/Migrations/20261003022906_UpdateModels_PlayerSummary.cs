using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sc2ReplayWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels_PlayerSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ActionsPerMinute",
                table: "Players",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ActionsPerMinute",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }
    }
}
