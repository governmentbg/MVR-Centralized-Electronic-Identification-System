using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID8259_APIaddchangeuserfunc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Providers.Users.AdministratorActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdministratorUid = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    AdministratorUidType = table.Column<int>(type: "integer", nullable: false),
                    AdministratorFullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers.Users.AdministratorActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Providers.Users.AdministratorActions_Providers.Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Providers.Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 3, 11));

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Users.AdministratorActions_UserId",
                table: "Providers.Users.AdministratorActions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Providers.Users.AdministratorActions");

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 1, 22));
        }
    }
}
