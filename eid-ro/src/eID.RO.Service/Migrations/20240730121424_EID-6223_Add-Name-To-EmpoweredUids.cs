using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6223_AddNameToEmpoweredUids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EmpowermentStatements.EmpoweredUids",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "EmpowermentStatements.NumbersRegister",
                keyColumn: "Id",
                keyValue: "EMPSTAT",
                column: "LastChange",
                value: new DateOnly(2024, 7, 30));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "EmpowermentStatements.EmpoweredUids");

            migrationBuilder.UpdateData(
                table: "EmpowermentStatements.NumbersRegister",
                keyColumn: "Id",
                keyValue: "EMPSTAT",
                column: "LastChange",
                value: new DateOnly(2024, 7, 24));
        }
    }
}
