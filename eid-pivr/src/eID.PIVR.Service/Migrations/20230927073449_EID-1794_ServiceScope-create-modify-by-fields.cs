using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID1794_ServiceScopecreatemodifybyfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceScopes_Name_IsDeleted",
                table: "ServiceScopes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceScopes");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ServiceScopes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ServiceScopes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "ServiceScopes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ServiceScopes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "IISDABatches",
                keyColumn: "Id",
                keyValue: new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"),
                column: "Name",
                value: "Министерство на правосъдието");

            migrationBuilder.UpdateData(
                table: "IISDASections",
                keyColumn: "Id",
                keyValue: new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"),
                column: "Name",
                value: "Услуги, предоставяни от централни администрации");

            migrationBuilder.UpdateData(
                table: "IISDAServices",
                keyColumn: "Id",
                keyValue: new Guid("01951076-86fc-4d81-ba09-12158825a98f"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Чуждестранни физически лица, които не са граждани на държава член на Европейския съюз и нямат постоянно пребиваване в Република България, имат право да извършват дейност с нестопанска цел на територията на Република България чрез юридическо лице с нестопанска цел в обществена полза", "Издаване на разрешение за извършване на дейност с нестопанска цел от чужденци в Република България" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ServiceScopes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ServiceScopes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ServiceScopes");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ServiceScopes");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceScopes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "IISDABatches",
                keyColumn: "Id",
                keyValue: new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"),
                column: "Name",
                value: "ÐœÐ¸Ð½Ð¸ÑÑ‚ÐµÑ€ÑÑ‚Ð²Ð¾ Ð½Ð° Ð¿Ñ€Ð°Ð²Ð¾ÑÑŠÐ´Ð¸ÐµÑ‚Ð¾");

            migrationBuilder.UpdateData(
                table: "IISDASections",
                keyColumn: "Id",
                keyValue: new Guid("7e9df27f-8e06-4384-bfc5-6b884d158245"),
                column: "Name",
                value: "Ð£ÑÐ»ÑƒÐ³Ð¸, Ð¿Ñ€ÐµÐ´Ð¾ÑÑ‚Ð°Ð²ÑÐ½Ð¸ Ð¾Ñ‚ Ñ†ÐµÐ½Ñ‚Ñ€Ð°Ð»Ð½Ð¸ Ð°Ð´Ð¼Ð¸Ð½Ð¸ÑÑ‚Ñ€Ð°Ñ†Ð¸Ð¸");

            migrationBuilder.UpdateData(
                table: "IISDAServices",
                keyColumn: "Id",
                keyValue: new Guid("01951076-86fc-4d81-ba09-12158825a98f"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Ð§ÑƒÐ¶Ð´ÐµÑÑ‚Ñ€Ð°Ð½Ð½Ð¸ Ñ„Ð¸Ð·Ð¸Ñ‡ÐµÑÐºÐ¸ Ð»Ð¸Ñ†Ð°, ÐºÐ¾Ð¸Ñ‚Ð¾ Ð½Ðµ ÑÐ° Ð³Ñ€Ð°Ð¶Ð´Ð°Ð½Ð¸ Ð½Ð° Ð´ÑŠÑ€Ð¶Ð°Ð²Ð° Ñ‡Ð»ÐµÐ½ Ð½Ð° Ð•Ð²Ñ€Ð¾Ð¿ÐµÐ¹ÑÐºÐ¸Ñ ÑÑŠÑŽÐ· Ð¸ Ð½ÑÐ¼Ð°Ñ‚ Ð¿Ð¾ÑÑ‚Ð¾ÑÐ½Ð½Ð¾ Ð¿Ñ€ÐµÐ±Ð¸Ð²Ð°Ð²Ð°Ð½Ðµ Ð² Ð ÐµÐ¿ÑƒÐ±Ð»Ð¸ÐºÐ° Ð‘ÑŠÐ»Ð³Ð°Ñ€Ð¸Ñ, Ð¸Ð¼Ð°Ñ‚ Ð¿Ñ€Ð°Ð²Ð¾ Ð´Ð° Ð¸Ð·Ð²ÑŠÑ€ÑˆÐ²Ð°Ñ‚ Ð´ÐµÐ¹Ð½Ð¾ÑÑ‚ Ñ Ð½ÐµÑÑ‚Ð¾Ð¿Ð°Ð½ÑÐºÐ° Ñ†ÐµÐ» Ð½Ð° Ñ‚ÐµÑ€Ð¸Ñ‚Ð¾Ñ€Ð¸ÑÑ‚Ð° Ð½Ð° Ð ÐµÐ¿ÑƒÐ±Ð»Ð¸ÐºÐ° Ð‘ÑŠÐ»Ð³Ð°Ñ€Ð¸Ñ Ñ‡Ñ€ÐµÐ· ÑŽÑ€Ð¸Ð´Ð¸Ñ‡ÐµÑÐºÐ¾ Ð»Ð¸Ñ†Ðµ Ñ Ð½ÐµÑÑ‚Ð¾Ð¿Ð°Ð½ÑÐºÐ° Ñ†ÐµÐ» Ð² Ð¾Ð±Ñ‰ÐµÑÑ‚Ð²ÐµÐ½Ð° Ð¿Ð¾Ð»Ð·Ð°", "Ð˜Ð·Ð´Ð°Ð²Ð°Ð½Ðµ Ð½Ð° Ñ€Ð°Ð·Ñ€ÐµÑˆÐµÐ½Ð¸Ðµ Ð·Ð° Ð¸Ð·Ð²ÑŠÑ€ÑˆÐ²Ð°Ð½Ðµ Ð½Ð° Ð´ÐµÐ¹Ð½Ð¾ÑÑ‚ Ñ Ð½ÐµÑÑ‚Ð¾Ð¿Ð°Ð½ÑÐºÐ° Ñ†ÐµÐ» Ð¾Ñ‚ Ñ‡ÑƒÐ¶Ð´ÐµÐ½Ñ†Ð¸ Ð² Ð ÐµÐ¿ÑƒÐ±Ð»Ð¸ÐºÐ° Ð‘ÑŠÐ»Ð³Ð°Ñ€Ð¸Ñ" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceScopes_Name_IsDeleted",
                table: "ServiceScopes",
                columns: new[] { "Name", "IsDeleted" });
        }
    }
}
