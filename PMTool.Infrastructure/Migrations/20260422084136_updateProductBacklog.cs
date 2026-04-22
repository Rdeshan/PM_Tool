using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateProductBacklog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentBacklogItemId",
                table: "ProjectBacklogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBacklogs_ParentBacklogItemId",
                table: "ProjectBacklogs",
                column: "ParentBacklogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBacklogs_ProjectBacklogs_ParentBacklogItemId",
                table: "ProjectBacklogs",
                column: "ParentBacklogItemId",
                principalTable: "ProjectBacklogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBacklogs_ProjectBacklogs_ParentBacklogItemId",
                table: "ProjectBacklogs");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBacklogs_ParentBacklogItemId",
                table: "ProjectBacklogs");

            migrationBuilder.DropColumn(
                name: "ParentBacklogItemId",
                table: "ProjectBacklogs");
        }
    }
}
