using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID7793_Addregistrationnumbers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalNumber",
                table: "Providers",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Providers",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Providers.NumbersRegister",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Current = table.Column<int>(type: "integer", nullable: false),
                    LastChange = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers.NumbersRegister", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Providers.NumbersRegister",
                columns: new[] { "Id", "Current", "LastChange" },
                values: new object[] { "REGNOID", 0, new DateOnly(2025, 1, 22) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Providers.NumbersRegister");

            migrationBuilder.DropColumn(
                name: "ExternalNumber",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Providers");
        }
    }
}
