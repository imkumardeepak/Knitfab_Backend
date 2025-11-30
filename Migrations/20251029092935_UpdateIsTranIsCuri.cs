using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIsTranIsCuri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourierId",
                table: "DispatchPlannings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCourier",
                table: "DispatchPlannings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTransport",
                table: "DispatchPlannings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TransportId",
                table: "DispatchPlannings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_CourierId",
                table: "DispatchPlannings",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_TransportId",
                table: "DispatchPlannings",
                column: "TransportId");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchPlannings_CourierMasters_CourierId",
                table: "DispatchPlannings",
                column: "CourierId",
                principalTable: "CourierMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DispatchPlannings_TransportMasters_TransportId",
                table: "DispatchPlannings",
                column: "TransportId",
                principalTable: "TransportMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DispatchPlannings_CourierMasters_CourierId",
                table: "DispatchPlannings");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatchPlannings_TransportMasters_TransportId",
                table: "DispatchPlannings");

            migrationBuilder.DropIndex(
                name: "IX_DispatchPlannings_CourierId",
                table: "DispatchPlannings");

            migrationBuilder.DropIndex(
                name: "IX_DispatchPlannings_TransportId",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "CourierId",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "IsCourier",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "IsTransport",
                table: "DispatchPlannings");

            migrationBuilder.DropColumn(
                name: "TransportId",
                table: "DispatchPlannings");
        }
    }
}
