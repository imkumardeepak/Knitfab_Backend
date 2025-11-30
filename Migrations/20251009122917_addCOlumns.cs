using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addCOlumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFGStickerGenerated",
                table: "RollConfirmations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NetWeight",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TareWeight",
                table: "RollConfirmations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "IsFGStickerGenerated",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "RollConfirmations");

            migrationBuilder.DropColumn(
                name: "TareWeight",
                table: "RollConfirmations");
        }
    }
}
