using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class EID4622RemovedConfirmedUids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmedUids",
                table: "Sagas.WithdrawalsCollections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmedUids",
                table: "Sagas.WithdrawalsCollections",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
