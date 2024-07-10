using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class EID3272_AddUidType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssuerUidType",
                table: "Sagas.WithdrawalsCollections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UidType",
                table: "Sagas.EmpowermentActivations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuerUidType",
                table: "Sagas.WithdrawalsCollections");

            migrationBuilder.DropColumn(
                name: "UidType",
                table: "Sagas.EmpowermentActivations");
        }
    }
}
