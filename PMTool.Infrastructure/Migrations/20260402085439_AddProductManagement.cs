using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBacklogs_Projects_ProjectId",
                table: "ProjectBacklogs");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "ProjectBacklogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PlannedReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReleaseType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseNotes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseNotes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBacklogs_ProductId",
                table: "ProjectBacklogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProjectId_VersionName",
                table: "Products",
                columns: new[] { "ProjectId", "VersionName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseNotes_CreatedByUserId",
                table: "ReleaseNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseNotes_ProductId",
                table: "ReleaseNotes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBacklogs_Products_ProductId",
                table: "ProjectBacklogs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBacklogs_Projects_ProjectId",
                table: "ProjectBacklogs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBacklogs_Products_ProductId",
                table: "ProjectBacklogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBacklogs_Projects_ProjectId",
                table: "ProjectBacklogs");

            migrationBuilder.DropTable(
                name: "ReleaseNotes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBacklogs_ProductId",
                table: "ProjectBacklogs");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProjectBacklogs");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBacklogs_Projects_ProjectId",
                table: "ProjectBacklogs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
