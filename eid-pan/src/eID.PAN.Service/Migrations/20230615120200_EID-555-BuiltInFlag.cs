using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PAN.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID555BuiltInFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltIn",
                table: "NotificationChannels.Rejected",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltIn",
                table: "NotificationChannels.Pending",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltIn",
                table: "NotificationChannels.Archive",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBuiltIn",
                table: "NotificationChannels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBuiltIn",
                table: "NotificationChannels.Rejected");

            migrationBuilder.DropColumn(
                name: "IsBuiltIn",
                table: "NotificationChannels.Pending");

            migrationBuilder.DropColumn(
                name: "IsBuiltIn",
                table: "NotificationChannels.Archive");

            migrationBuilder.DropColumn(
                name: "IsBuiltIn",
                table: "NotificationChannels");
        }
    }
}
