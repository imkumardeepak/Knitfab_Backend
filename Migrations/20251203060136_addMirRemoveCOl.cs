using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class addMirRemoveCOl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherReference",
                table: "SalesOrdersWeb",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
