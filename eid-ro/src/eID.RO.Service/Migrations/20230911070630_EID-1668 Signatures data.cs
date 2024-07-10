using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID1668Signaturesdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.Signatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SignerUid = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Signature = table.Column<string>(type: "text", nullable: false),
                    EmpowermentStatementId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.Signatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpowermentStatements.Signatures_EmpowermentStatements_Empo~",
                        column: x => x.EmpowermentStatementId,
                        principalTable: "EmpowermentStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpowermentStatements.Signatures_EmpowermentStatementId_Sig~",
                table: "EmpowermentStatements.Signatures",
                columns: new[] { "EmpowermentStatementId", "SignerUid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpowermentStatements.Signatures");
        }
    }
}
