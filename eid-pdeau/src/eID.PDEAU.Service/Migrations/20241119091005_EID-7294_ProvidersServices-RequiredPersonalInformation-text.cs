using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.PDEAU.Contracts.Enums;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID7294_ProvidersServicesRequiredPersonalInformationtext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RequiredPersonalInformation",
                table: "Providers.Details.Sections.Services",
                type: "text",
                nullable: true,
                oldClrType: typeof(ICollection<CollectablePersonalInformation>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.Sql("ALTER TABLE IF EXISTS public.\"Providers.Details.Sections.Services\" ALTER COLUMN \"RequiredPersonalInformation\" DROP NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE IF EXISTS public.\"Providers.Details.Sections.Services\" ALTER COLUMN \"RequiredPersonalInformation\" SET NOT NULL;");

            migrationBuilder.AlterColumn<ICollection<CollectablePersonalInformation>>(
                name: "RequiredPersonalInformation",
                table: "Providers.Details.Sections.Services",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
