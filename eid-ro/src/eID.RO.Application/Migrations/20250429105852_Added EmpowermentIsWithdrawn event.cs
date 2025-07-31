using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmpowermentIsWithdrawnevent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmpowermentWithdrawn",
                table: "Sagas.SignaturesCollections",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmpowermentWithdrawn",
                table: "Sagas.EmpowermentActivations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmpowermentWithdrawn",
                table: "Sagas.SignaturesCollections");

            migrationBuilder.DropColumn(
                name: "IsEmpowermentWithdrawn",
                table: "Sagas.EmpowermentActivations");
        }
    }
}
