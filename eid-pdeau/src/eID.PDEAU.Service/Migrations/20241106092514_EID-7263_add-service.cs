using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID7263_addservice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinimumLevelOfAssurance",
                table: "Providers.Details.Sections.Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequiredPersonalInformation",
                table: "Providers.Details.Sections.Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumLevelOfAssurance",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.DropColumn(
                name: "RequiredPersonalInformation",
                table: "Providers.Details.Sections.Services");
        }
    }
}
