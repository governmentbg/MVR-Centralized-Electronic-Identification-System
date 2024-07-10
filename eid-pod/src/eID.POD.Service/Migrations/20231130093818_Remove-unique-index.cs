using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.POD.Service.Migrations
{
    /// <inheritdoc />
    public partial class Removeuniqueindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Datasets_DatasetName",
                table: "Datasets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Datasets_DatasetName",
                table: "Datasets",
                column: "DatasetName",
                unique: true);
        }
    }
}
