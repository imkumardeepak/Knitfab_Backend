using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class ADDCOlumnDisPlanningTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "DispatchPlannings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumCapacityKgs",
                table: "DispatchPlannings",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "DispatchPlannings",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransportName",
                table: "DispatchPlannings",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_ContactPerson",
                table: "DispatchPlannings",
                column: "ContactPerson");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_TransportName",
                table: "DispatchPlannings",
                column: "TransportName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DispatchPlannings_ContactPerson",
                table: "DispatchPlannings");

            migrationBuilder.DropIndex(
                name: "IX_DispatchPlannings_TransportName",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "MaximumCapacityKgs",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "TransportName",
                table: "DispatchPlannings");
        }
    }
}
