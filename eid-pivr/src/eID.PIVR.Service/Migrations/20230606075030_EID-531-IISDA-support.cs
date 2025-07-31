using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID531IISDAsupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IISDABatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                name: "IISDAServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceID = table.Column<int>(type: "integer", nullable: true),
                    ServiceNumber = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    PaymentInfoNormalCost = table.Column<double>(type: "double precision", nullable: true),
                    IsEmpowerment = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "ServiceScopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IISDAServiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceScopes_IISDAServices_IISDAServiceId",
                        column: x => x.IISDAServiceId,
                        principalTable: "IISDAServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "IISDABatches",
                columns: new[] { "Id", "BatchId", "IsDeleted", "Name", "Status" },
                values: new object[] { new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"), 61, false, "Министерство на правосъдието", 0 });

            migrationBuilder.InsertData(
                table: "IISDASections",
                columns: new[] { "Id", "IsDeleted", "IsExternal", "Name" },
                values: new object[] { new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"), false, true, "Услуги, предоставяни от централни администрации" });

            migrationBuilder.InsertData(
                table: "IISDAServices",
                columns: new[] { "Id", "Description", "IISDABatchId", "IISDASectionId", "IsDeleted", "IsEmpowerment", "Name", "PaymentInfoNormalCost", "ServiceID", "ServiceNumber" },
                values: new object[] { new Guid("01951076-86fc-4d81-ba09-12158825a98f"), "Чуждестранни физически лица, които не са граждани на държава член на Европейския съюз и нямат постоянно пребиваване в Република България, имат право да извършват дейност с нестопанска цел на територията на Република България чрез юридическо лице с нестопанска цел в обществена полза", new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"), new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"), false, false, "Издаване на разрешение за извършване на дейност с нестопанска цел от чужденци в Република България", 20.0, 20384, 316 });

            migrationBuilder.CreateIndex(
                name: "IX_IISDABatches_Name",
                table: "IISDABatches",
                column: "Name",
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
                name: "IX_IISDAServices_IISDABatchId",
                table: "IISDAServices",
                column: "IISDABatchId");

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_IISDASectionId",
                table: "IISDAServices",
                column: "IISDASectionId");

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_Name_IsEmpowerment_IsDeleted",
                table: "IISDAServices",
                columns: new[] { "Name", "IsEmpowerment", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_ServiceID_Name",
                table: "IISDAServices",
                columns: new[] { "ServiceID", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopes_IISDAServiceId_Code",
                table: "ServiceScopes",
                columns: new[] { "IISDAServiceId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopes_IISDAServiceId_Name",
                table: "ServiceScopes",
                columns: new[] { "IISDAServiceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopes_Name_IsDeleted",
                table: "ServiceScopes",
                columns: new[] { "Name", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceScopes");

            migrationBuilder.DropTable(
                name: "IISDAServices");

            migrationBuilder.DropTable(
                name: "IISDABatches");

            migrationBuilder.DropTable(
                name: "IISDASections");
        }
    }
}
