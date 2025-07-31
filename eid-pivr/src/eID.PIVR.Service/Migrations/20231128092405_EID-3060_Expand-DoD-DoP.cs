using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID3060_ExpandDoDDoP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionOfProhibition",
                table: "DatesOfProhibition",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TypeOfProhibition",
                table: "DatesOfProhibition",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "UidType",
                table: "DatesOfProhibition",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "UidType",
                table: "DatesOfDeath",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionOfProhibition",
                table: "DatesOfProhibition");

            migrationBuilder.DropColumn(
                name: "TypeOfProhibition",
                table: "DatesOfProhibition");

            migrationBuilder.DropColumn(
                name: "UidType",
                table: "DatesOfProhibition");

            migrationBuilder.DropColumn(
                name: "UidType",
                table: "DatesOfDeath");
        }
    }
}
