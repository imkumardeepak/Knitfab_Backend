using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class isPickColAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPick",
                table: "DispatchPlannings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "DispatchedRolls",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPick",
                table: "DispatchedRolls",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NetWeight",
                table: "DispatchedRolls",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPick",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "DispatchedRolls");

            migrationBuilder.DropColumn(
                name: "IsPick",
                table: "DispatchedRolls");

            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "DispatchedRolls");
        }
    }
}
