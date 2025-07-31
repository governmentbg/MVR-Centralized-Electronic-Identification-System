using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PJS.Services.Migrations
{
    /// <inheritdoc />
    public partial class Reasonforexclusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReasonForExclusion",
                table: "Exclusions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PathCheckResults",
                columns: table => new
                {
                    IsExcluded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PathCheckResults");

            migrationBuilder.DropColumn(
                name: "ReasonForExclusion",
                table: "Exclusions");
        }
    }
}
