using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubProjectIdToProductBacklog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubProjectId",
                table: "ProductBacklogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_SubProjectId",
                table: "ProductBacklogs",
                column: "SubProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBacklogs_SubProjects_SubProjectId",
                table: "ProductBacklogs",
                column: "SubProjectId",
                principalTable: "SubProjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductBacklogs_SubProjects_SubProjectId",
                table: "ProductBacklogs");

            migrationBuilder.DropIndex(
                name: "IX_ProductBacklogs_SubProjectId",
                table: "ProductBacklogs");

            migrationBuilder.DropColumn(
                name: "SubProjectId",
                table: "ProductBacklogs");
        }
    }
}
