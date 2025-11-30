using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class dateformat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "IsActive",
            //    table: "SalesOrdersWeb",
            //    newName: "IsProcess");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "SalesOrdersWeb",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "SalesOrdersWeb",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "SalesOrdersWeb",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "DispatchThrough",
            //    table: "SalesOrdersWeb",
            //    type: "character varying(200)",
            //    maxLength: 200,
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "OrderNo",
            //    table: "SalesOrdersWeb",
            //    type: "character varying(100)",
            //    maxLength: 100,
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ProcessDate",
            //    table: "SalesOrdersWeb",
            //    type: "timestamp without time zone",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "TermsOfDelivery",
            //    table: "SalesOrdersWeb",
            //    type: "character varying(200)",
            //    maxLength: 200,
            //    nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "SalesOrderItemsWeb",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsProcess",
            //    table: "SalesOrderItemsWeb",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessDate",
                table: "SalesOrderItemsWeb",
                type: "timestamp without time zone",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Unit",
            //    table: "SalesOrderItemsWeb",
            //    type: "character varying(50)",
            //    maxLength: 50,
            //    nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispatchThrough",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "ProcessDate",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "TermsOfDelivery",
                table: "SalesOrdersWeb");

            migrationBuilder.DropColumn(
                name: "IsProcess",
                table: "SalesOrderItemsWeb");

            migrationBuilder.DropColumn(
                name: "ProcessDate",
                table: "SalesOrderItemsWeb");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "SalesOrderItemsWeb");

            migrationBuilder.RenameColumn(
                name: "IsProcess",
                table: "SalesOrdersWeb",
                newName: "IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "SalesOrdersWeb",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "SalesOrdersWeb",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "SalesOrdersWeb",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "SalesOrderItemsWeb",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
