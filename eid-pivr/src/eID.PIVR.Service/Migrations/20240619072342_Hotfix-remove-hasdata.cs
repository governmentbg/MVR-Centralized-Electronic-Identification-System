using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class Hotfixremovehasdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IISDAServices",
                keyColumn: "Id",
                keyValue: new Guid("01951076-86fc-4d81-ba09-12158825a98f"));

            migrationBuilder.DeleteData(
                table: "IISDABatches",
                keyColumn: "Id",
                keyValue: new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"));

            migrationBuilder.DeleteData(
                table: "IISDASections",
                keyColumn: "Id",
                keyValue: new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "IISDABatches",
                columns: new[] { "Id", "IdentificationNumber", "IsDeleted", "IsExternal", "Name", "Status" },
                values: new object[] { new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"), "0000000061", false, true, "Министерство на правосъдието", 0 });

            migrationBuilder.InsertData(
                table: "IISDASections",
                columns: new[] { "Id", "IsDeleted", "IsExternal", "Name" },
                values: new object[] { new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"), false, true, "Услуги, предоставяни от централни администрации" });

            migrationBuilder.InsertData(
                table: "IISDAServices",
                columns: new[] { "Id", "Description", "IISDABatchId", "IISDASectionId", "IsDeleted", "IsEmpowerment", "IsExternal", "Name", "PaymentInfoNormalCost", "ServiceNumber" },
                values: new object[] { new Guid("01951076-86fc-4d81-ba09-12158825a98f"), "Чуждестранни физически лица, които не са граждани на държава член на Европейския съюз и нямат постоянно пребиваване в Република България, имат право да извършват дейност с нестопанска цел на територията на Република България чрез юридическо лице с нестопанска цел в обществена полза", new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"), new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"), false, false, true, "Издаване на разрешение за извършване на дейност с нестопанска цел от чужденци в Република България", 20m, 316L });
        }
    }
}
