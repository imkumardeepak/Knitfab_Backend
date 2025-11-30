using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddsaleorderWEbtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesOrdersWeb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VoucherType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TermsOfPayment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsJobWork = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyGSTIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BuyerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BuyerGSTIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BuyerState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BuyerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BuyerContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BuyerAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConsigneeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConsigneeGSTIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ConsigneeState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConsigneePhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ConsigneeContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConsigneeAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrdersWeb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderItemsWeb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalesOrderWebId = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ItemDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    YarnCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DiaGG = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FabricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Composition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WtPerRoll = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    NoOfRolls = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    IGST = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    SGST = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CGST = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderItemsWeb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderItemsWeb_SalesOrdersWeb_SalesOrderWebId",
                        column: x => x.SalesOrderWebId,
                        principalTable: "SalesOrdersWeb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItemsWeb_ItemName",
                table: "SalesOrderItemsWeb",
                column: "ItemName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItemsWeb_SalesOrderWebId",
                table: "SalesOrderItemsWeb",
                column: "SalesOrderWebId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrdersWeb_BuyerName",
                table: "SalesOrdersWeb",
                column: "BuyerName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrdersWeb_OrderDate",
                table: "SalesOrdersWeb",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrdersWeb_VoucherNumber",
                table: "SalesOrdersWeb",
                column: "VoucherNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesOrderItemsWeb");

            migrationBuilder.DropTable(
                name: "SalesOrdersWeb");
        }
    }
}
