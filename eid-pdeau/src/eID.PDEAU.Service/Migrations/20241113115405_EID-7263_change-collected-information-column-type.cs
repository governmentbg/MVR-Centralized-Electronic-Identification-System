using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.PDEAU.Contracts.Enums;

#nullable disable

namespace eID.PDEAU.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID7263_changecollectedinformationcolumntype : Migration
    {
        /// <inheritdoc />
        /// 
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Providers.Details.Sections.Services\" ALTER COLUMN \"RequiredPersonalInformation\" DROP DEFAULT;");

            migrationBuilder.Sql("ALTER TABLE \"Providers.Details.Sections.Services\" ALTER COLUMN \"RequiredPersonalInformation\" TYPE jsonb " +
                "USING ( CASE " +
                    "WHEN \"RequiredPersonalInformation\" IS NULL THEN NULL " +
                    "ELSE to_jsonb(ARRAY[\"RequiredPersonalInformation\"]) " +
                "END)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Providers.Details.Sections.Services\" ALTER COLUMN \"RequiredPersonalInformation\" TYPE integer " +
                "USING ( CASE " +
                    "WHEN \"RequiredPersonalInformation\" IS NULL THEN 0::integer " +
                    "ELSE(\"RequiredPersonalInformation\"->> 0)::integer " +
                "END)");

            migrationBuilder.Sql("ALTER TABLE \"Providers.Details.Sections.Services\" ALTER COLUMN \"RequiredPersonalInformation\" SET DEFAULT 0;");
        }
    }
}
