using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID965Propertynamechanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedOn",
                table: "EmpowermentStatements",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "EmpowermentStatements",
                newName: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "EmpowermentStatements",
                newName: "ModifiedOn");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "EmpowermentStatements",
                newName: "ModifiedBy");
        }
    }
}
