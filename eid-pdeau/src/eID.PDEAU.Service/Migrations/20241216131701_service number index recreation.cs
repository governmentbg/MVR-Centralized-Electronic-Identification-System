using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class servicenumberindexrecreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Providers.Details.Sections.Services_ProviderDetailsId_Servi~",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections.Services_ProviderDetailsId_Servi~",
                table: "Providers.Details.Sections.Services",
                columns: new[] { "ProviderDetailsId", "ServiceNumber" },
                unique: true,
                filter: "\"ServiceNumber\" != 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Providers.Details.Sections.Services_ProviderDetailsId_Servi~",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections.Services_ProviderDetailsId_Servi~",
                table: "Providers.Details.Sections.Services",
                columns: new[] { "ProviderDetailsId", "ServiceNumber" },
                unique: true);
        }
    }
}
