using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6530_datareorganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupplierName",
                table: "EmpowermentStatements",
                newName: "ProviderName");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "EmpowermentStatements",
                newName: "ProviderId");

            migrationBuilder.UpdateData(
                table: "EmpowermentStatements.NumbersRegister",
                keyColumn: "Id",
                keyValue: "EMPSTAT",
                column: "LastChange",
                value: new DateOnly(2024, 9, 9));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderName",
                table: "EmpowermentStatements",
                newName: "SupplierName");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "EmpowermentStatements",
                newName: "SupplierId");

            migrationBuilder.UpdateData(
                table: "EmpowermentStatements.NumbersRegister",
                keyColumn: "Id",
                keyValue: "EMPSTAT",
                column: "LastChange",
                value: new DateOnly(2024, 7, 30));
        }
    }
}
