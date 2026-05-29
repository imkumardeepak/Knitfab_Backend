using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AvyyanBackend.Data;

#nullable disable

namespace AvyyanBackend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260528120000_AddInspectionStatus")]
    public partial class AddInspectionStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Inspections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
UPDATE ""Inspections""
SET ""Status"" = CASE
    WHEN COALESCE(""Flag"", true) = true THEN 0
    ELSE 1
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Inspections");
        }
    }
}
