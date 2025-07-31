using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6134_Addempowermentnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "EmpowermentStatements",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.NumbersRegister",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Current = table.Column<int>(type: "integer", nullable: false),
                    LastChange = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.NumbersRegister", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "EmpowermentStatements.NumbersRegister",
                columns: new[] { "Id", "Current", "LastChange" },
                values: new object[] { "EMPSTAT", 0, new DateOnly(2024, 7, 24) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpowermentStatements.NumbersRegister");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "EmpowermentStatements");
        }
    }
}
