using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID6271_Adddefaultservicescopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IISDAServices.Scopes.Defaults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IISDAServices.Scopes.Defaults", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IISDAServices.Scopes.Defaults",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("20ab27de-c94b-4b35-b596-265db6e1051c"), "Обжалване на административния акт, резултат от услугата, или на отказа от издаването на такъв" },
                    { new Guid("2f40f241-98f1-4308-8090-b2eac2626049"), "Оттегляне на заявлението" },
                    { new Guid("42cbea41-ba99-4223-820b-6ff03b67d56e"), "Заявяване представянето на информация и документи" },
                    { new Guid("51994550-9e34-4546-9e33-7dbd586b9532"), "Получаване на резултатите от услугата" },
                    { new Guid("c215ecf2-f15c-430d-8061-41f3c0595629"), "Заявяване на услугата" },
                    { new Guid("c73d44f7-3fae-43f3-8413-fd9e18505e75"), "Получаване на съобщения, свързани с електронната административна услуга" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices.Scopes.Defaults_Name",
                table: "IISDAServices.Scopes.Defaults",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IISDAServices.Scopes.Defaults");
        }
    }
}
