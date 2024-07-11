using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddDenialReasonfieldtostate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DenialReason",
                table: "Sagas.EmpowermentActivations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DenialReason",
                table: "Sagas.EmpowermentActivations");
        }
    }
}
