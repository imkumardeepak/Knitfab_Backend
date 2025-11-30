using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addCOlGrossNEtwt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalGrossWeight",
                table: "DispatchPlannings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalNetWeight",
                table: "DispatchPlannings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 3,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalGrossWeight",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "TotalNetWeight",
                table: "DispatchPlannings");
        }
    }
}
