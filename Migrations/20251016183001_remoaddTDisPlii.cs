using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class remoaddTDisPlii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispatchPlannings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    SalesOrderItemId = table.Column<int>(type: "integer", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tape = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalRequiredRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalReadyRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalDispatchedRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    IsFullyDispatched = table.Column<bool>(type: "boolean", nullable: false),
                    DispatchStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DispatchEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VehicleNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    License = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    LoadingNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchPlannings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DispatchedRolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DispatchPlanningId = table.Column<int>(type: "integer", nullable: false),
                    LotNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FGRollNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NetWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RollNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsLoaded = table.Column<bool>(type: "boolean", nullable: false),
                    LoadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchedRolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchedRolls_DispatchPlannings_DispatchPlanningId",
                        column: x => x.DispatchPlanningId,
                        principalTable: "DispatchPlannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchedRolls_DispatchPlanningId",
                table: "DispatchedRolls",
                column: "DispatchPlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchedRolls_FGRollNo",
                table: "DispatchedRolls",
                column: "FGRollNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchedRolls_LotNo",
                table: "DispatchedRolls",
                column: "LotNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_CustomerName",
                table: "DispatchPlannings",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_LoadingNo",
                table: "DispatchPlannings",
                column: "LoadingNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_LotNo",
                table: "DispatchPlannings",
                column: "LotNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_SalesOrderId",
                table: "DispatchPlannings",
                column: "SalesOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchedRolls");

            migrationBuilder.DropTable(
                name: "DispatchPlannings");
        }
    }
}
