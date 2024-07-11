using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class EID863WithdrawalsCollectionState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sagas.WithdrawalsCollections",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OriginCorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WithdrawalsCollectionsDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuerUid = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmpowermentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnBehalfOf = table.Column<int>(type: "integer", nullable: false),
                    AuthorizerUids = table.Column<string>(type: "text", nullable: false),
                    EmpoweredUids = table.Column<string>(type: "text", nullable: false),
                    ConfirmedUids = table.Column<string>(type: "text", nullable: false),
                    WithdrawalsCollectionTimeoutTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpowermentWithdrawalId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalEntityUid = table.Column<string>(type: "text", nullable: true),
                    LegalEntityName = table.Column<string>(type: "text", nullable: true),
                    IssuerName = table.Column<string>(type: "text", nullable: false),
                    IssuerPosition = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sagas.WithdrawalsCollections", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sagas.WithdrawalsCollections");
        }
    }
}
