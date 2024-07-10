using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Application.Migrations
{
    /// <inheritdoc />
    public partial class EID761Initialsagasstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sagas.EmpowermentActivations",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginCorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivationDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpowermentActivationTimeoutTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpowermentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnBehalfOf = table.Column<int>(type: "integer", nullable: false),
                    AuthorizerUids = table.Column<string>(type: "text", nullable: false),
                    EmpoweredUids = table.Column<string>(type: "text", nullable: false),
                    Uid = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IssuerName = table.Column<string>(type: "text", nullable: false),
                    IssuerPosition = table.Column<string>(type: "text", nullable: true),
                    SuccessfulCompletion = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sagas.EmpowermentActivations", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "Sagas.SignaturesCollections",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginCorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReceivedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SignaturesCollectionDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SignatureCollectionTimeoutTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpowermentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorizerUids = table.Column<string>(type: "text", nullable: false),
                    SignedUids = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sagas.SignaturesCollections", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sagas.EmpowermentActivations");

            migrationBuilder.DropTable(
                name: "Sagas.SignaturesCollections");
        }
    }
}
