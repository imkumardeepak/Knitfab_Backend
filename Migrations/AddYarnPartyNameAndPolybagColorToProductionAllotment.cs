using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddYarnPartyNameAndPolybagColorToProductionAllotment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YarnPartyName",
                table: "ProductionAllotments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolybagColor",
                table: "ProductionAllotments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YarnPartyName",
                table: "ProductionAllotments");

            migrationBuilder.DropColumn(
                name: "PolybagColor",
                table: "ProductionAllotments");
        }
    }
}