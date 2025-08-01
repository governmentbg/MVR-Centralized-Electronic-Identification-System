using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID9084_AddworkingTimewebSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebSiteUrl",
                table: "Providers.Details",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingTimeEnd",
                table: "Providers.Details",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkingTimeStart",
                table: "Providers.Details",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 6, 17));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebSiteUrl",
                table: "Providers.Details");

            migrationBuilder.DropColumn(
                name: "WorkingTimeEnd",
                table: "Providers.Details");

            migrationBuilder.DropColumn(
                name: "WorkingTimeStart",
                table: "Providers.Details");

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 4, 30));
        }
    }
}
