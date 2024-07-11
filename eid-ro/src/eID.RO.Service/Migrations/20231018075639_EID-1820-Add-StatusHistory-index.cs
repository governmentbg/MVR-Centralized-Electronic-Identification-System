using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID1820AddStatusHistoryindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmpowermentStatements.StatusHistory_Status_DateTime",
                table: "EmpowermentStatements.StatusHistory",
                columns: new[] { "Status", "DateTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmpowermentStatements.StatusHistory_Status_DateTime",
                table: "EmpowermentStatements.StatusHistory");
        }
    }
}
