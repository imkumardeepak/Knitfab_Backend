using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addColTrans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GstNo",
                table: "TransportMasters",
                newName: "DriverNumber");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "TransportMasters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "TransportMasters");

            migrationBuilder.RenameColumn(
                name: "DriverNumber",
                table: "TransportMasters",
                newName: "GstNo");
        }
    }
}
