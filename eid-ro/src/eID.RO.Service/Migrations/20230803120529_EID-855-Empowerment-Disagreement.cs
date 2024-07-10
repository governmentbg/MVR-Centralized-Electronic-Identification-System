using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.RO.Service.Entities;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID855EmpowermentDisagreement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.DisagreementReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Translations = table.Column<ICollection<EmpowermentDisagreementReasonTranslation>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.DisagreementReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.Disagreements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActiveDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuerUid = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmpowermentStatementId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.Disagreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpowermentStatements.Disagreements_EmpowermentStatements_E~",
                        column: x => x.EmpowermentStatementId,
                        principalTable: "EmpowermentStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpowermentStatements.Disagreements_EmpowermentStatementId",
                table: "EmpowermentStatements.Disagreements",
                column: "EmpowermentStatementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpowermentStatements.DisagreementReasons");

            migrationBuilder.DropTable(
                name: "EmpowermentStatements.Disagreements");
        }
    }
}
