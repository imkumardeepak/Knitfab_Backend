using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoCOlAddColl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaGG",
                table: "SalesOrderItemsWeb");

            migrationBuilder.AddColumn<int>(
                name: "Dia",
                table: "SalesOrderItemsWeb",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GG",
                table: "SalesOrderItemsWeb",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dia",
                table: "SalesOrderItemsWeb");

            migrationBuilder.DropColumn(
                name: "GG",
                table: "SalesOrderItemsWeb");

            migrationBuilder.AddColumn<string>(
                name: "DiaGG",
                table: "SalesOrderItemsWeb",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
