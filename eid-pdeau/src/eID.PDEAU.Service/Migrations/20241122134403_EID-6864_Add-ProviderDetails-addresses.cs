using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6864_AddProviderDetailsaddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Providers.Details",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Headquarters",
                table: "Providers.Details",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Providers.Details");

            migrationBuilder.DropColumn(
                name: "Headquarters",
                table: "Providers.Details");
        }
    }
}
