using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.PIVR.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID575IISDAchgfieldstype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IISDAServices_IISDABatchId",
                table: "IISDAServices");

            migrationBuilder.DropIndex(
                name: "IX_IISDAServices_Name_IsEmpowerment_IsDeleted",
                table: "IISDAServices");

            migrationBuilder.DropIndex(
                name: "IX_IISDAServices_ServiceID_Name",
                table: "IISDAServices");

            migrationBuilder.DropIndex(
                name: "IX_IISDABatches_Name",
                table: "IISDABatches");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                table: "IISDAServices");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "IISDABatches");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServiceScopes",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<long>(
                name: "ServiceNumber",
                table: "IISDAServices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PaymentInfoNormalCost",
                table: "IISDAServices",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IISDAServices",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "IISDAServices",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "IISDAServices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IISDABatches",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "IISDABatches",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "IISDABatches",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "IISDABatches",
                keyColumn: "Id",
                keyValue: new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"),
                columns: new[] { "IdentificationNumber", "IsExternal", "Name" },
                values: new object[] { "0000000061", true, "Министерство на правосъдието" });

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
                columns: new[] { "Description", "IsExternal", "Name", "PaymentInfoNormalCost", "ServiceNumber" },
                values: new object[] { "Чуждестранни физически лица, които не са граждани на държава член на Европейския съюз и нямат постоянно пребиваване в Република България, имат право да извършват дейност с нестопанска цел на територията на Република България чрез юридическо лице с нестопанска цел в обществена полза", true, "Издаване на разрешение за извършване на дейност с нестопанска цел от чужденци в Република България", 20m, 316L });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_IISDABatchId_ServiceNumber",
                table: "IISDAServices",
                columns: new[] { "IISDABatchId", "ServiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_ServiceNumber_Name_IsEmpowerment_IsDeleted",
                table: "IISDAServices",
                columns: new[] { "ServiceNumber", "Name", "IsEmpowerment", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IISDABatches_IdentificationNumber",
                table: "IISDABatches",
                column: "IdentificationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IISDAServices_IISDABatchId_ServiceNumber",
                table: "IISDAServices");

            migrationBuilder.DropIndex(
                name: "IX_IISDAServices_ServiceNumber_Name_IsEmpowerment_IsDeleted",
                table: "IISDAServices");

            migrationBuilder.DropIndex(
                name: "IX_IISDABatches_IdentificationNumber",
                table: "IISDABatches");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "IISDAServices");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "IISDABatches");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "IISDABatches");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServiceScopes",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceNumber",
                table: "IISDAServices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "PaymentInfoNormalCost",
                table: "IISDAServices",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IISDAServices",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "IISDAServices",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceID",
                table: "IISDAServices",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IISDABatches",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<int>(
                name: "BatchId",
                table: "IISDABatches",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "IISDABatches",
                keyColumn: "Id",
                keyValue: new Guid("32810f2f-a366-4ff4-abf2-b75cc931e995"),
                columns: new[] { "BatchId", "Name" },
                values: new object[] { 61, "ÐœÐ¸Ð½Ð¸ÑÑ‚ÐµÑ€ÑÑ‚Ð²Ð¾ Ð½Ð° Ð¿Ñ€Ð°Ð²Ð¾ÑÑŠÐ´Ð¸ÐµÑ‚Ð¾" });

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
                columns: new[] { "Description", "Name", "PaymentInfoNormalCost", "ServiceID", "ServiceNumber" },
                values: new object[] { "Ð§ÑƒÐ¶Ð´ÐµÑÑ‚Ñ€Ð°Ð½Ð½Ð¸ Ñ„Ð¸Ð·Ð¸Ñ‡ÐµÑÐºÐ¸ Ð»Ð¸Ñ†Ð°, ÐºÐ¾Ð¸Ñ‚Ð¾ Ð½Ðµ ÑÐ° Ð³Ñ€Ð°Ð¶Ð´Ð°Ð½Ð¸ Ð½Ð° Ð´ÑŠÑ€Ð¶Ð°Ð²Ð° Ñ‡Ð»ÐµÐ½ Ð½Ð° Ð•Ð²Ñ€Ð¾Ð¿ÐµÐ¹ÑÐºÐ¸Ñ ÑÑŠÑŽÐ· Ð¸ Ð½ÑÐ¼Ð°Ñ‚ Ð¿Ð¾ÑÑ‚Ð¾ÑÐ½Ð½Ð¾ Ð¿Ñ€ÐµÐ±Ð¸Ð²Ð°Ð²Ð°Ð½Ðµ Ð² Ð ÐµÐ¿ÑƒÐ±Ð»Ð¸ÐºÐ° Ð‘ÑŠÐ»Ð³Ð°Ñ€Ð¸Ñ, Ð¸Ð¼Ð°Ñ‚ Ð¿Ñ€Ð°Ð²Ð¾ Ð´Ð° Ð¸Ð·Ð²ÑŠÑ€ÑˆÐ²Ð°Ñ‚ Ð´ÐµÐ¹Ð½Ð¾ÑÑ‚ Ñ Ð½ÐµÑÑ‚Ð¾Ð¿Ð°Ð½ÑÐºÐ° Ñ†ÐµÐ» Ð½Ð° Ñ‚ÐµÑ€Ð¸Ñ‚Ð¾Ñ€Ð¸ÑÑ‚Ð° Ð½Ð° Ð ÐµÐ¿ÑƒÐ±Ð»Ð¸ÐºÐ° Ð‘ÑŠÐ»Ð³Ð°Ñ€Ð¸Ñ Ñ‡Ñ€ÐµÐ· ÑŽÑ€Ð¸Ð´Ð¸Ñ‡ÐµÑÐºÐ¾ Ð»Ð¸Ñ†Ðµ Ñ Ð½ÐµÑÑ‚Ð¾Ð¿Ð°Ð½ÑÐºÐ° Ñ†ÐµÐ» Ð² Ð¾Ð±Ñ‰ÐµÑÑ‚Ð²ÐµÐ½Ð° Ð¿Ð¾Ð»Ð·Ð°", "Ð˜Ð·Ð´Ð°Ð²Ð°Ð½Ðµ Ð½Ð° Ñ€Ð°Ð·Ñ€ÐµÑˆÐµÐ½Ð¸Ðµ Ð·Ð° Ð¸Ð·Ð²ÑŠÑ€ÑˆÐ²Ð°Ð½Ðµ Ð½Ð° Ð´ÐµÐ¹Ð½Ð¾ÑÑ‚ Ñ Ð½ÐµÑÑ‚Ð¾Ð¿Ð°Ð½ÑÐºÐ° Ñ†ÐµÐ» Ð¾Ñ‚ Ñ‡ÑƒÐ¶Ð´ÐµÐ½Ñ†Ð¸ Ð² Ð ÐµÐ¿ÑƒÐ±Ð»Ð¸ÐºÐ° Ð‘ÑŠÐ»Ð³Ð°Ñ€Ð¸Ñ", 20.0, 20384, 316 });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_IISDABatchId",
                table: "IISDAServices",
                column: "IISDABatchId");

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_Name_IsEmpowerment_IsDeleted",
                table: "IISDAServices",
                columns: new[] { "Name", "IsEmpowerment", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IISDAServices_ServiceID_Name",
                table: "IISDAServices",
                columns: new[] { "ServiceID", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IISDABatches_Name",
                table: "IISDABatches",
                column: "Name",
                unique: true);
        }
    }
}
