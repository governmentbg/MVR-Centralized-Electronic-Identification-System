using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class Identitynumberuniqueindexupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Providers.Details_IdentificationNumber",
                table: "Providers.Details");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details_IdentificationNumber",
                table: "Providers.Details",
                column: "IdentificationNumber",
                unique: true,
                filter: "\"IdentificationNumber\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Providers.Details_IdentificationNumber",
                table: "Providers.Details");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details_IdentificationNumber",
                table: "Providers.Details",
                column: "IdentificationNumber",
                unique: true);
        }
    }
}
