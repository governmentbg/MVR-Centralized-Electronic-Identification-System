using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.POD.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID1846 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datasets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DatasetName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CronPeriod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataSource = table.Column<string>(type: "text", nullable: false),
                    DatasetUri = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastRun = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datasets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_DatasetName",
                table: "Datasets",
                column: "DatasetName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Datasets");
        }
    }
}
