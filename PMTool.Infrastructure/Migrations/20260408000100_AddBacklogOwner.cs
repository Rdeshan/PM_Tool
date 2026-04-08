using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations;

public partial class AddBacklogOwner : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "OwnerId",
            table: "ProjectBacklogs",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_ProjectBacklogs_OwnerId",
            table: "ProjectBacklogs",
            column: "OwnerId");

        migrationBuilder.AddForeignKey(
            name: "FK_ProjectBacklogs_Users_OwnerId",
            table: "ProjectBacklogs",
            column: "OwnerId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ProjectBacklogs_Users_OwnerId",
            table: "ProjectBacklogs");

        migrationBuilder.DropIndex(
            name: "IX_ProjectBacklogs_OwnerId",
            table: "ProjectBacklogs");

        migrationBuilder.DropColumn(
            name: "OwnerId",
            table: "ProjectBacklogs");
    }
}
