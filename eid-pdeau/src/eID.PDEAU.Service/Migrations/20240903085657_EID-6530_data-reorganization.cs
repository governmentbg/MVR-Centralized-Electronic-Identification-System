using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6530_datareorganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IISDAServices.Scopes_IISDAServices_IISDAServiceId",
                table: "IISDAServices.Scopes");

            migrationBuilder.DropTable(
                name: "IISDAServices");

            migrationBuilder.DropTable(
                name: "IISDABatches");

            migrationBuilder.DropTable(
                name: "IISDASections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IISDAServices.Scopes.Defaults",
                table: "IISDAServices.Scopes.Defaults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IISDAServices.Scopes",
                table: "IISDAServices.Scopes");

            migrationBuilder.RenameTable(
                name: "IISDAServices.Scopes.Defaults",
                newName: "Providers.Details.Sections.Services.Scopes.Defaults");

            migrationBuilder.RenameTable(
                name: "IISDAServices.Scopes",
                newName: "Providers.Details.Sections.Services.Scopes");

            migrationBuilder.RenameIndex(
                name: "IX_IISDAServices.Scopes.Defaults_Name",
                table: "Providers.Details.Sections.Services.Scopes.Defaults",
                newName: "IX_Providers.Details.Sections.Services.Scopes.Defaults_Name");

            migrationBuilder.RenameColumn(
                name: "IISDAServiceId",
                table: "Providers.Details.Sections.Services.Scopes",
                newName: "ProviderServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_IISDAServices.Scopes_IISDAServiceId_Name",
                table: "Providers.Details.Sections.Services.Scopes",
                newName: "IX_Providers.Details.Sections.Services.Scopes_ProviderServiceI~");

            migrationBuilder.AddColumn<Guid>(
                name: "DetailsId",
                table: "Providers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Providers.Details.Sections.Services.Scopes.Defaults",
                table: "Providers.Details.Sections.Services.Scopes.Defaults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Providers.Details.Sections.Services.Scopes",
                table: "Providers.Details.Sections.Services.Scopes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Providers.Details",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    SyncedFromOnlineRegistry = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UIC = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers.Details", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers.Details.Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SyncedFromOnlineRegistry = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderDetailsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers.Details.Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Providers.Details.Sections_Providers.Details_ProviderDetail~",
                        column: x => x.ProviderDetailsId,
                        principalTable: "Providers.Details",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Providers.Details.Sections.Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceNumber = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    PaymentInfoNormalCost = table.Column<decimal>(type: "numeric", nullable: true),
                    IsEmpowerment = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedFromOnlineRegistry = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderDetailsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderSectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers.Details.Sections.Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Providers.Details.Sections.Services_Providers.Details.Secti~",
                        column: x => x.ProviderSectionId,
                        principalTable: "Providers.Details.Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Providers.Details.Sections.Services_Providers.Details_Provi~",
                        column: x => x.ProviderDetailsId,
                        principalTable: "Providers.Details",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Providers_DetailsId",
                table: "Providers",
                column: "DetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details_IdentificationNumber",
                table: "Providers.Details",
                column: "IdentificationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details_Name_IsDeleted",
                table: "Providers.Details",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections_Name_IsDeleted",
                table: "Providers.Details.Sections",
                columns: new[] { "Name", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections_Name_ProviderDetailsId",
                table: "Providers.Details.Sections",
                columns: new[] { "Name", "ProviderDetailsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections_ProviderDetailsId",
                table: "Providers.Details.Sections",
                column: "ProviderDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections.Services_ProviderDetailsId_Servi~",
                table: "Providers.Details.Sections.Services",
                columns: new[] { "ProviderDetailsId", "ServiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections.Services_ProviderSectionId",
                table: "Providers.Details.Sections.Services",
                column: "ProviderSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Providers.Details.Sections.Services_ServiceNumber_Name_IsEm~",
                table: "Providers.Details.Sections.Services",
                columns: new[] { "ServiceNumber", "Name", "IsEmpowerment", "IsDeleted" });

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_Providers.Details_DetailsId",
                table: "Providers",
                column: "DetailsId",
                principalTable: "Providers.Details",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Providers.Details.Sections.Services.Scopes_Providers.Detail~",
                table: "Providers.Details.Sections.Services.Scopes",
                column: "ProviderServiceId",
                principalTable: "Providers.Details.Sections.Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Providers_Providers.Details_DetailsId",
                table: "Providers");

            migrationBuilder.DropForeignKey(
                name: "FK_Providers.Details.Sections.Services.Scopes_Providers.Detail~",
                table: "Providers.Details.Sections.Services.Scopes");

            migrationBuilder.DropTable(
                name: "Providers.Details.Sections.Services");

            migrationBuilder.DropTable(
                name: "Providers.Details.Sections");

            migrationBuilder.DropTable(
                name: "Providers.Details");

            migrationBuilder.DropIndex(
                name: "IX_Providers_DetailsId",
                table: "Providers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Providers.Details.Sections.Services.Scopes.Defaults",
                table: "Providers.Details.Sections.Services.Scopes.Defaults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Providers.Details.Sections.Services.Scopes",
                table: "Providers.Details.Sections.Services.Scopes");

            migrationBuilder.DropColumn(
                name: "DetailsId",
                table: "Providers");

            migrationBuilder.RenameTable(
                name: "Providers.Details.Sections.Services.Scopes.Defaults",
                newName: "IISDAServices.Scopes.Defaults");

            migrationBuilder.RenameTable(
                name: "Providers.Details.Sections.Services.Scopes",
                newName: "IISDAServices.Scopes");

            migrationBuilder.RenameIndex(
                name: "IX_Providers.Details.Sections.Services.Scopes.Defaults_Name",
                table: "IISDAServices.Scopes.Defaults",
                newName: "IX_IISDAServices.Scopes.Defaults_Name");

            migrationBuilder.RenameColumn(
                name: "ProviderServiceId",
                table: "IISDAServices.Scopes",
                newName: "IISDAServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Providers.Details.Sections.Services.Scopes_ProviderServiceI~",
                table: "IISDAServices.Scopes",
                newName: "IX_IISDAServices.Scopes_IISDAServiceId_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IISDAServices.Scopes.Defaults",
                table: "IISDAServices.Scopes.Defaults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IISDAServices.Scopes",
                table: "IISDAServices.Scopes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "IISDABatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UIC = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false)
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
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
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
                    IISDABatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    IISDASectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsEmpowerment = table.Column<bool>(type: "boolean", nullable: false),
                    IsExternal = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    PaymentInfoNormalCost = table.Column<decimal>(type: "numeric", nullable: true),
                    ServiceNumber = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_IISDAServices.Scopes_IISDAServices_IISDAServiceId",
                table: "IISDAServices.Scopes",
                column: "IISDAServiceId",
                principalTable: "IISDAServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
