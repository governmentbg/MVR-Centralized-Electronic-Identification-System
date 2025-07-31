using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID5792_ScopesTableNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceScopes_IISDAServices_IISDAServiceId",
                table: "ServiceScopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceScopes",
                table: "ServiceScopes");

            migrationBuilder.RenameTable(
                name: "ServiceScopes",
                newName: "IISDAServices.Scopes");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceScopes_IISDAServiceId_Name",
                table: "IISDAServices.Scopes",
                newName: "IX_IISDAServices.Scopes_IISDAServiceId_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IISDAServices.Scopes",
                table: "IISDAServices.Scopes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IISDAServices.Scopes_IISDAServices_IISDAServiceId",
                table: "IISDAServices.Scopes",
                column: "IISDAServiceId",
                principalTable: "IISDAServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IISDAServices.Scopes_IISDAServices_IISDAServiceId",
                table: "IISDAServices.Scopes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IISDAServices.Scopes",
                table: "IISDAServices.Scopes");

            migrationBuilder.RenameTable(
                name: "IISDAServices.Scopes",
                newName: "ServiceScopes");

            migrationBuilder.RenameIndex(
                name: "IX_IISDAServices.Scopes_IISDAServiceId_Name",
                table: "ServiceScopes",
                newName: "IX_ServiceScopes_IISDAServiceId_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceScopes",
                table: "ServiceScopes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceScopes_IISDAServices_IISDAServiceId",
                table: "ServiceScopes",
                column: "IISDAServiceId",
                principalTable: "IISDAServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
