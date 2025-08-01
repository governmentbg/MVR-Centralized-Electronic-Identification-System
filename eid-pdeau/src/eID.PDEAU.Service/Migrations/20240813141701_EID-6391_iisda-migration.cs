using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6391_iisdamigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IssuerName",
                table: "Providers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "IISDABatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UIC = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IISDABatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IISDASections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IISDASections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IISDAServices.Scopes.Defaults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IISDAServices.Scopes.Defaults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IISDAServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceNumber = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    PaymentInfoNormalCost = table.Column<decimal>(type: "numeric", nullable: true),
                    IsEmpowerment = table.Column<bool>(type: "boolean", nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IISDABatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    IISDASectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IISDAServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IISDAServices_IISDABatches_IISDABatchId",
                        column: x => x.IISDABatchId,
                        principalTable: "IISDABatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IISDAServices_IISDASections_IISDASectionId",
                        column: x => x.IISDASectionId,
                        principalTable: "IISDASections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IISDAServices.Scopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IISDAServiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IISDAServices.Scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IISDAServices.Scopes_IISDAServices_IISDAServiceId",
                        column: x => x.IISDAServiceId,
                        principalTable: "IISDAServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IISDAServices.Scopes.Defaults",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("20ab27de-c94b-4b35-b596-265db6e1051c"), "Обжалване на административния акт, резултат от услугата, или на отказа от издаването на такъв" },
                    { new Guid("2f40f241-98f1-4308-8090-b2eac2626049"), "Оттегляне на заявлението" },
                    { new Guid("42cbea41-ba99-4223-820b-6ff03b67d56e"), "Заявяване представянето на информация и документи" },
                    { new Guid("51994550-9e34-4546-9e33-7dbd586b9532"), "Получаване на резултатите от услугата" },
                    { new Guid("c215ecf2-f15c-430d-8061-41f3c0595629"), "Заявяване на услугата" },
                    { new Guid("c73d44f7-3fae-43f3-8413-fd9e18505e75"), "Получаване на съобщения, свързани с електронната административна услуга" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IISDABatches_IdentificationNumber",
                table: "IISDABatches",
                column: "IdentificationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IISDABatches_Name_IsDeleted",
                table: "IISDABatches",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IISDASections_Name",
                table: "IISDASections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IISDASections_Name_IsDeleted",
                table: "IISDASections",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_IISDABatchId_ServiceNumber",
                table: "IISDAServices",
                columns: new[] { "IISDABatchId", "ServiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_IISDASectionId",
                table: "IISDAServices",
                column: "IISDASectionId");

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_ServiceNumber_Name_IsEmpowerment_IsDeleted",
                table: "IISDAServices",
                columns: new[] { "ServiceNumber", "Name", "IsEmpowerment", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices.Scopes_IISDAServiceId_Name",
                table: "IISDAServices.Scopes",
                columns: new[] { "IISDAServiceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices.Scopes.Defaults_Name",
                table: "IISDAServices.Scopes.Defaults",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IISDAServices.Scopes");

            migrationBuilder.DropTable(
                name: "IISDAServices.Scopes.Defaults");

            migrationBuilder.DropTable(
                name: "IISDAServices");

            migrationBuilder.DropTable(
                name: "IISDABatches");

            migrationBuilder.DropTable(
                name: "IISDASections");

            migrationBuilder.AlterColumn<string>(
                name: "IssuerName",
                table: "Providers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
