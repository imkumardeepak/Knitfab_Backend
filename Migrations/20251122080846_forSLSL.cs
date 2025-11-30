using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class forSLSL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "SalesOrderItemsWeb",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SlitLine",
                table: "SalesOrderItemsWeb",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StitchLength",
                table: "SalesOrderItemsWeb",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "SalesOrderItemsWeb");

            migrationBuilder.DropColumn(
                name: "SlitLine",
                table: "SalesOrderItemsWeb");

            migrationBuilder.DropColumn(
                name: "StitchLength",
                table: "SalesOrderItemsWeb");
        }
    }
}
