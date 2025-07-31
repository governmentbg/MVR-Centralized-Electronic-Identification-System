using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class EmpowermentVerificationSagaIssuerPositionnotrequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IssuerPosition",
                table: "Sagas.EmpowermentVerifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IssuerPosition",
                table: "Sagas.EmpowermentVerifications",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
