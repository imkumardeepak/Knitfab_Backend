using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class nullremAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNo",
                table: "SalesOrderItemsWeb");

            migrationBuilder.AddColumn<string>(
                name: "SerialNo",
                table: "SalesOrdersWeb",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNo",
                table: "SalesOrdersWeb");

            migrationBuilder.AddColumn<string>(
                name: "SerialNo",
                table: "SalesOrderItemsWeb",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
