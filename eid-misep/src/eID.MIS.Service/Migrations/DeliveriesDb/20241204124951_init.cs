using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.MIS.Service.Migrations.DeliveriesDb
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CitizenProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    SentOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SystemName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Subject = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ReferencedOrn = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CitizenProfileId",
                table: "Deliveries",
                column: "CitizenProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deliveries");
        }
    }
}
