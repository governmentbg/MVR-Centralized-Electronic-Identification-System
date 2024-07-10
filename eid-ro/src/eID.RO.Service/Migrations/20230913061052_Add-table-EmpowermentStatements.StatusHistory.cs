using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddtableEmpowermentStatementsStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.StatusHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpowermentStatementId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.StatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpowermentStatements.StatusHistory_EmpowermentStatements_E~",
                        column: x => x.EmpowermentStatementId,
                        principalTable: "EmpowermentStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpowermentStatements.StatusHistory_EmpowermentStatementId",
                table: "EmpowermentStatements.StatusHistory",
                column: "EmpowermentStatementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpowermentStatements.StatusHistory");
        }
    }
}
