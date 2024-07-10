using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.RO.Service.Entities;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID862AddEmpowermentWithdrawReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorizerUids_EmpowermentStatements_EmpowermentStatementId",
                table: "AuthorizerUids");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpoweredUids_EmpowermentStatements_EmpowermentStatementId",
                table: "EmpoweredUids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpoweredUids",
                table: "EmpoweredUids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorizerUids",
                table: "AuthorizerUids");

            migrationBuilder.RenameTable(
                name: "EmpoweredUids",
                newName: "EmpowermentStatements.EmpoweredUids");

            migrationBuilder.RenameTable(
                name: "AuthorizerUids",
                newName: "EmpowermentStatements.AuthorizerUids");

            migrationBuilder.RenameIndex(
                name: "IX_EmpoweredUids_Uid",
                table: "EmpowermentStatements.EmpoweredUids",
                newName: "IX_EmpowermentStatements.EmpoweredUids_Uid");

            migrationBuilder.RenameIndex(
                name: "IX_EmpoweredUids_EmpowermentStatementId",
                table: "EmpowermentStatements.EmpoweredUids",
                newName: "IX_EmpowermentStatements.EmpoweredUids_EmpowermentStatementId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorizerUids_Uid",
                table: "EmpowermentStatements.AuthorizerUids",
                newName: "IX_EmpowermentStatements.AuthorizerUids_Uid");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorizerUids_EmpowermentStatementId",
                table: "EmpowermentStatements.AuthorizerUids",
                newName: "IX_EmpowermentStatements.AuthorizerUids_EmpowermentStatementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpowermentStatements.EmpoweredUids",
                table: "EmpowermentStatements.EmpoweredUids",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpowermentStatements.AuthorizerUids",
                table: "EmpowermentStatements.AuthorizerUids",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_EmpowermentStatements.AuthorizerUids_EmpowermentStatements_~",
                table: "EmpowermentStatements.AuthorizerUids",
                column: "EmpowermentStatementId",
                principalTable: "EmpowermentStatements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpowermentStatements.EmpoweredUids_EmpowermentStatements_E~",
                table: "EmpowermentStatements.EmpoweredUids",
                column: "EmpowermentStatementId",
                principalTable: "EmpowermentStatements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpowermentStatements.AuthorizerUids_EmpowermentStatements_~",
                table: "EmpowermentStatements.AuthorizerUids");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpowermentStatements.EmpoweredUids_EmpowermentStatements_E~",
                table: "EmpowermentStatements.EmpoweredUids");

            migrationBuilder.DropTable(
                name: "EmpowermentStatements.Reasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpowermentStatements.EmpoweredUids",
                table: "EmpowermentStatements.EmpoweredUids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmpowermentStatements.AuthorizerUids",
                table: "EmpowermentStatements.AuthorizerUids");

            migrationBuilder.RenameTable(
                name: "EmpowermentStatements.EmpoweredUids",
                newName: "EmpoweredUids");

            migrationBuilder.RenameTable(
                name: "EmpowermentStatements.AuthorizerUids",
                newName: "AuthorizerUids");

            migrationBuilder.RenameIndex(
                name: "IX_EmpowermentStatements.EmpoweredUids_Uid",
                table: "EmpoweredUids",
                newName: "IX_EmpoweredUids_Uid");

            migrationBuilder.RenameIndex(
                name: "IX_EmpowermentStatements.EmpoweredUids_EmpowermentStatementId",
                table: "EmpoweredUids",
                newName: "IX_EmpoweredUids_EmpowermentStatementId");

            migrationBuilder.RenameIndex(
                name: "IX_EmpowermentStatements.AuthorizerUids_Uid",
                table: "AuthorizerUids",
                newName: "IX_AuthorizerUids_Uid");

            migrationBuilder.RenameIndex(
                name: "IX_EmpowermentStatements.AuthorizerUids_EmpowermentStatementId",
                table: "AuthorizerUids",
                newName: "IX_AuthorizerUids_EmpowermentStatementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmpoweredUids",
                table: "EmpoweredUids",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorizerUids",
                table: "AuthorizerUids",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorizerUids_EmpowermentStatements_EmpowermentStatementId",
                table: "AuthorizerUids",
                column: "EmpowermentStatementId",
                principalTable: "EmpowermentStatements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpoweredUids_EmpowermentStatements_EmpowermentStatementId",
                table: "EmpoweredUids",
                column: "EmpowermentStatementId",
                principalTable: "EmpowermentStatements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
