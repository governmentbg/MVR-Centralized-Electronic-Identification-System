using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID7688_providerservicestatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Providers.Details.Sections.Services",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW() AT TIME ZONE 'UTC'");

            migrationBuilder.AddColumn<string>(
                name: "DenialReason",
                table: "Providers.Details.Sections.Services",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Providers.Details.Sections.Services",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Providers.Details.Sections.Services",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerFullName",
                table: "Providers.Details.Sections.Services",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Providers.Details.Sections.Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.DropColumn(
                name: "DenialReason",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.DropColumn(
                name: "ReviewerFullName",
                table: "Providers.Details.Sections.Services");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Providers.Details.Sections.Services");
        }
    }
}
