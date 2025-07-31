using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PAN.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6283_AddNotificationChannelrejectreason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "NotificationChannels.Rejected",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "NotificationChannels.Rejected");
        }
    }
}
