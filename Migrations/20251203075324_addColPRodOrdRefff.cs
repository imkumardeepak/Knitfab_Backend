using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addColPRodOrdRefff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherReference",
                table: "ProductionAllotments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherReference",
                table: "ProductionAllotments");
        }
    }
}
