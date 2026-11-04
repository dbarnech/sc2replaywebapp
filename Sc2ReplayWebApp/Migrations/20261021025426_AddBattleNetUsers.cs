using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sc2ReplayWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "Replays");

            migrationBuilder.AddColumn<int>(
                name: "BattleNetUserId",
                table: "Replays",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BattleNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BattleNetId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BattleTag = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FirstLogin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleNetUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Replays_BattleNetUserId",
                table: "Replays",
                column: "BattleNetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BattleNetUsers_BattleNetId",
                table: "BattleNetUsers",
                column: "BattleNetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Replays_BattleNetUsers_BattleNetUserId",
                table: "Replays",
                column: "BattleNetUserId",
                principalTable: "BattleNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replays_BattleNetUsers_BattleNetUserId",
                table: "Replays");

            migrationBuilder.DropTable(
                name: "BattleNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Replays_BattleNetUserId",
                table: "Replays");

            migrationBuilder.DropColumn(
                name: "BattleNetUserId",
                table: "Replays");

            migrationBuilder.AddColumn<string>(
                name: "UploadedByUserId",
                table: "Replays",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
