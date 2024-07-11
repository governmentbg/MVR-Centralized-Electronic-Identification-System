using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.RO.Service.Entities;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID863Withdrawalscollectionchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpowermentStatements.Reasons");

            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.WithdrawalReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Translations = table.Column<ICollection<EmpowermentWithdrawalReasonTranslation>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.WithdrawalReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.Withdrawals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActiveDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IssuerUid = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EmpowermentStatementId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.Withdrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpowermentStatements.Withdrawals_EmpowermentStatements_Emp~",
                        column: x => x.EmpowermentStatementId,
                        principalTable: "EmpowermentStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpowermentStatements.Withdrawals_EmpowermentStatementId",
                table: "EmpowermentStatements.Withdrawals",
                column: "EmpowermentStatementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpowermentStatements.WithdrawalReasons");

            migrationBuilder.DropTable(
                name: "EmpowermentStatements.Withdrawals");

            migrationBuilder.CreateTable(
                name: "EmpowermentStatements.Reasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Translations = table.Column<ICollection<EmpowermentWithdrawalReasonTranslation>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements.Reasons", x => x.Id);
                });
        }
    }
}
