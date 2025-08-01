using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID8748_AddDeniedOntime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeniedOn",
                table: "Providers.Details.Sections.Services",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 4, 30));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeniedOn",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 3, 11));
        }
    }
}
