using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.RO.Service.Entities;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID572AddIndividualEmpowermentStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpowermentStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uid = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OnBehalfOf = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    SupplierName = table.Column<string>(type: "text", nullable: false),
                    ServiceId = table.Column<int>(type: "integer", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    VolumeOfRepresentation = table.Column<ICollection<VolumeOfRepresentation>>(type: "jsonb", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    XMLRepresentation = table.Column<string>(type: "text", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpowermentStatements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizerUids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uid = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    EmpowermentStatementId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizerUids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizerUids_EmpowermentStatements_EmpowermentStatementId",
                        column: x => x.EmpowermentStatementId,
                        principalTable: "EmpowermentStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpoweredUids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uid = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    EmpowermentStatementId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpoweredUids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpoweredUids_EmpowermentStatements_EmpowermentStatementId",
                        column: x => x.EmpowermentStatementId,
                        principalTable: "EmpowermentStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizerUids_EmpowermentStatementId",
                table: "AuthorizerUids",
                column: "EmpowermentStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizerUids_Uid",
                table: "AuthorizerUids",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpoweredUids_EmpowermentStatementId",
                table: "EmpoweredUids",
                column: "EmpowermentStatementId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpoweredUids_Uid",
                table: "EmpoweredUids",
                column: "Uid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpowermentStatements_Uid",
                table: "EmpowermentStatements",
                column: "Uid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizerUids");

            migrationBuilder.DropTable(
                name: "EmpoweredUids");

            migrationBuilder.DropTable(
                name: "EmpowermentStatements");
        }
    }
}
