using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID5792removeCodefromIISDAServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceScopes_IISDAServiceId_Code",
                table: "ServiceScopes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "ServiceScopes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ServiceScopes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopes_IISDAServiceId_Code",
                table: "ServiceScopes",
                columns: new[] { "IISDAServiceId", "Code" },
                unique: true);
        }
    }
}
