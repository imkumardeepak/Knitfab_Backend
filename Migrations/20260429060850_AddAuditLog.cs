using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PolybagColor",
                table: "ProductionAllotments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductionStatus",
                table: "ProductionAllotments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "YarnPartyName",
                table: "ProductionAllotments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolybagColor",
                table: "ProductionAllotments");

            migrationBuilder.DropColumn(
                name: "ProductionStatus",
                table: "ProductionAllotments");

            migrationBuilder.DropColumn(
                name: "YarnPartyName",
                table: "ProductionAllotments");
        }
    }
}
