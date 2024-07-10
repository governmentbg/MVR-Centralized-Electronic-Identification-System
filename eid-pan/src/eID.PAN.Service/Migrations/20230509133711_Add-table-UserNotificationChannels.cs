using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PAN.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddtableUserNotificationChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserNotificationChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationChannels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationChannels_NotificationChannelId",
                table: "UserNotificationChannels",
                column: "NotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationChannels_UserId_NotificationChannelId",
                table: "UserNotificationChannels",
                columns: new[] { "UserId", "NotificationChannelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserNotificationChannels");
        }
    }
}
