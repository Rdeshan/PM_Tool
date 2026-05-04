using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductBacklogCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductBacklogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentBacklogItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBacklogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBacklogs_ProductBacklogs_ParentBacklogItemId",
                        column: x => x.ParentBacklogItemId,
                        principalTable: "ProductBacklogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductBacklogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductBacklogs_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_OwnerId",
                table: "ProductBacklogs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_ParentBacklogItemId",
                table: "ProductBacklogs",
                column: "ParentBacklogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBacklogs_ProductId",
                table: "ProductBacklogs",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBacklogs");
        }
    }
}
