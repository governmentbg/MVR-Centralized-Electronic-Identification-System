using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID5220addingempowermentstatementdenialreasoncomment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DenialReasonComment",
                table: "EmpowermentStatements",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DenialReasonComment",
                table: "EmpowermentStatements");
        }
    }
}
