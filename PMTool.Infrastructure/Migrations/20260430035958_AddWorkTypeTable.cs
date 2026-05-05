using System;
using Microsoft.EntityFrameworkCore.Migrations;


#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WorkTypes",
                columns: new[] { "Id", "CreatedDate", "Description", "IsDefault", "Key", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(2705), null, true, "feature", "Feature" },
                    { 2, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5083), null, true, "improvement", "Improvement" },
                    { 3, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5086), null, true, "changerequest", "Change Request" },
                    { 4, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5088), null, true, "testcase", "Test Case" },
                    { 5, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5089), null, true, "story", "User Story" },
                    { 6, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5090), null, true, "bug", "Bug" },
                    { 7, new DateTime(2026, 4, 30, 3, 59, 56, 895, DateTimeKind.Utc).AddTicks(5091), null, true, "task", "Task" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkTypes");
        }
    }
}
