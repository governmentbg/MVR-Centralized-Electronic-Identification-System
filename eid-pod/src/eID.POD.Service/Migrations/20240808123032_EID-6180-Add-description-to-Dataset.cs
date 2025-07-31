using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.POD.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6180AdddescriptiontoDataset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Datasets",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Datasets");
        }
    }
}
