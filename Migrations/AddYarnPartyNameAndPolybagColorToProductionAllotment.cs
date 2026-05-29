using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AvyyanBackend.Data;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("AddYarnPartyNameAndPolybagColorToProductionAllotment")]
    public partial class AddYarnPartyNameAndPolybagColorToProductionAllotment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER TABLE ""ProductionAllotments""
    ADD COLUMN IF NOT EXISTS ""YarnPartyName"" character varying(200);

ALTER TABLE ""ProductionAllotments""
    ADD COLUMN IF NOT EXISTS ""PolybagColor"" character varying(100);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YarnPartyName",
                table: "ProductionAllotments");

            migrationBuilder.DropColumn(
                name: "PolybagColor",
                table: "ProductionAllotments");
        }
    }
}
