using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID9088_Registerdoneservices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Providers.DoneServises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers.DoneServises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Providers.DoneServises_Providers.Details.Sections.Services_~",
                        column: x => x.ServiceId,
                        principalTable: "Providers.Details.Sections.Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Providers.DoneServises_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 7, 15));

            migrationBuilder.CreateIndex(
                name: "IX_Providers.DoneServises_ProviderId_CreatedOn",
                table: "Providers.DoneServises",
                columns: new[] { "ProviderId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Providers.DoneServises_ServiceId",
                table: "Providers.DoneServises",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Providers.DoneServises");

            migrationBuilder.UpdateData(
                table: "Providers.NumbersRegister",
                keyColumn: "Id",
                keyValue: "REGNOID",
                column: "LastChange",
                value: new DateOnly(2025, 6, 30));
        }
    }
}
