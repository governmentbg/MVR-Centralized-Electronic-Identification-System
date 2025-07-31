using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PAN.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6280_notificationchannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "NotificationChannels.Rejected");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "NotificationChannels.Pending");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "NotificationChannels.Archive");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "NotificationChannels");

            migrationBuilder.AddColumn<string>(
                name: "SystemName",
                table: "NotificationChannels.Rejected",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SystemName",
                table: "NotificationChannels.Pending",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SystemName",
                table: "NotificationChannels.Archive",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SystemName",
                table: "NotificationChannels",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemName",
                table: "NotificationChannels.Rejected");

            migrationBuilder.DropColumn(
                name: "SystemName",
                table: "NotificationChannels.Pending");

            migrationBuilder.DropColumn(
                name: "SystemName",
                table: "NotificationChannels.Archive");

            migrationBuilder.DropColumn(
                name: "SystemName",
                table: "NotificationChannels");

            migrationBuilder.AddColumn<Guid>(
                name: "SystemId",
                table: "NotificationChannels.Rejected",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SystemId",
                table: "NotificationChannels.Pending",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SystemId",
                table: "NotificationChannels.Archive",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SystemId",
                table: "NotificationChannels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
