using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eID.PAN.Service.Entities;

#nullable disable

namespace eID.PAN.Service.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationsinRegisteredSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<RegisteredSystemTranslation>>(
                name: "Translations",
                table: "RegisteredSystems",
                type: "jsonb",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Translations",
                table: "RegisteredSystems");
        }
    }
}
