using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSprintManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SprintId",
                table: "ProductBacklogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintScopeChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SprintId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BacklogItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ChangedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintScopeChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintScopeChanges_ProductBacklogs_BacklogItemId",
                        column: x => x.BacklogItemId,
                        principalTable: "ProductBacklogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SprintScopeChanges_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintScopeChanges_Users_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_SprintId",
                table: "ProductBacklogs",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProductId",
                table: "Sprints",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintScopeChanges_BacklogItemId",
                table: "SprintScopeChanges",
                column: "BacklogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintScopeChanges_ChangedById",
                table: "SprintScopeChanges",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_SprintScopeChanges_SprintId",
                table: "SprintScopeChanges",
                column: "SprintId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBacklogs_Sprints_SprintId",
                table: "ProductBacklogs",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductBacklogs_Sprints_SprintId",
                table: "ProductBacklogs");

            migrationBuilder.DropTable(
                name: "SprintScopeChanges");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropIndex(
                name: "IX_ProductBacklogs_SprintId",
                table: "ProductBacklogs");

            migrationBuilder.DropColumn(
                name: "SprintId",
                table: "ProductBacklogs");
        }
    }
}
