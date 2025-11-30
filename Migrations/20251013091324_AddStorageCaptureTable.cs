using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageCaptureTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageCaptures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FGRollNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tape = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageCaptures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_CustomerName",
                table: "StorageCaptures",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_FGRollNo",
                table: "StorageCaptures",
                column: "FGRollNo");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_LocationCode",
                table: "StorageCaptures",
                column: "LocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_LotNo",
                table: "StorageCaptures",
                column: "LotNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageCaptures");
        }
    }
}
