using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID9161_AddUserssoftdelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Providers.Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 6, 30));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Providers.Users");

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 6, 17));
        }
    }
}
