using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID2861_GetDeceasedByPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DatesOfDeath_PersonalId_CreatedOn",
                table: "DatesOfDeath",
                columns: new[] { "PersonalId", "CreatedOn" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatesOfDeath_PersonalId_CreatedOn",
                table: "DatesOfDeath");
        }
    }
}
