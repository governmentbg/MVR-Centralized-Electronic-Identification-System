using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID3272_AddUidType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssuerUidType",
                table: "EmpowermentStatements.Withdrawals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SignerUidType",
                table: "EmpowermentStatements.Signatures",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UidType",
                table: "EmpowermentStatements.EmpoweredUids",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IssuerUidType",
                table: "EmpowermentStatements.Disagreements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UidType",
                table: "EmpowermentStatements.AuthorizerUids",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UidType",
                table: "EmpowermentStatements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuerUidType",
                table: "EmpowermentStatements.Withdrawals");

            migrationBuilder.DropColumn(
                name: "SignerUidType",
                table: "EmpowermentStatements.Signatures");

            migrationBuilder.DropColumn(
                name: "UidType",
                table: "EmpowermentStatements.EmpoweredUids");

            migrationBuilder.DropColumn(
                name: "IssuerUidType",
                table: "EmpowermentStatements.Disagreements");

            migrationBuilder.DropColumn(
                name: "UidType",
                table: "EmpowermentStatements.AuthorizerUids");

            migrationBuilder.DropColumn(
                name: "UidType",
                table: "EmpowermentStatements");
        }
    }
}
