using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class EID4877_empowermentverifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sagas.EmpowermentVerifications",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginCorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CurrentRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmpowermentValidationCheckExpirationTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpowermentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnBehalfOf = table.Column<int>(type: "integer", nullable: false),
                    Uid = table.Column<string>(type: "text", nullable: false),
                    UidType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IssuerPosition = table.Column<string>(type: "text", nullable: false),
                    AuthorizerUids = table.Column<string>(type: "text", nullable: false),
                    EmpoweredUids = table.Column<string>(type: "text", nullable: false),
                    DenialReason = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sagas.EmpowermentVerifications", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sagas.EmpowermentVerifications");
        }
    }
}
