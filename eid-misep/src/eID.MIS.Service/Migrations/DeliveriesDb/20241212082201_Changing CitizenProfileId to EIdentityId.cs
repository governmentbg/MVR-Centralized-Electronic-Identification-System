using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.MIS.Service.Migrations.DeliveriesDb
{
    /// <inheritdoc />
    public partial class ChangingCitizenProfileIdtoEIdentityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CitizenProfileId",
                table: "Deliveries",
                newName: "EIdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_Deliveries_CitizenProfileId",
                table: "Deliveries",
                newName: "IX_Deliveries_EIdentityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EIdentityId",
                table: "Deliveries",
                newName: "CitizenProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Deliveries_EIdentityId",
                table: "Deliveries",
                newName: "IX_Deliveries_CitizenProfileId");
        }
    }
}
