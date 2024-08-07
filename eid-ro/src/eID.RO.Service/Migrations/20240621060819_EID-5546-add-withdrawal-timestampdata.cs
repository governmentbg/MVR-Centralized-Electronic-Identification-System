﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eID.RO.Service.Migrations
{
    /// <inheritdoc />
    public partial class EID5546addwithdrawaltimestampdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimestampData",
                table: "EmpowermentStatements.Withdrawals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimestampData",
                table: "EmpowermentStatements.Withdrawals");
        }
    }
}
