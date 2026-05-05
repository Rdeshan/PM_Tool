using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(5567));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(6658));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(6660));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(6661));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(6662));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(6664));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 10, 7, 28, 853, DateTimeKind.Utc).AddTicks(6665));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(2705));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5083));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5086));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5088));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5089));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5090));

            migrationBuilder.UpdateData(
                table: "WorkTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5091));
        }
    }
}
