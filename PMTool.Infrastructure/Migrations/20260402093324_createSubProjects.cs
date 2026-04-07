using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createSubProjects : Migration
    {
        /// <inheritdoc />
        /// 
        /* testing commit */
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubProjectId",
                table: "ProjectBacklogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ModuleOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProjects_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubProjects_Users_ModuleOwnerId",
                        column: x => x.ModuleOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubProjectDependencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependsOnSubProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProjectDependencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProjectDependencies_SubProjects_DependsOnSubProjectId",
                        column: x => x.DependsOnSubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubProjectDependencies_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubProjectTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProjectTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProjectTeams_SubProjects_SubProjectId",
                        column: x => x.SubProjectId,
                        principalTable: "SubProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubProjectTeams_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBacklogs_SubProjectId",
                table: "ProjectBacklogs",
                column: "SubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProjectDependencies_DependsOnSubProjectId",
                table: "SubProjectDependencies",
                column: "DependsOnSubProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProjectDependencies_SubProjectId_DependsOnSubProjectId",
                table: "SubProjectDependencies",
                columns: new[] { "SubProjectId", "DependsOnSubProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubProjects_ModuleOwnerId",
                table: "SubProjects",
                column: "ModuleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProjects_ProductId",
                table: "SubProjects",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProjectTeams_SubProjectId_TeamId",
                table: "SubProjectTeams",
                columns: new[] { "SubProjectId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubProjectTeams_TeamId",
                table: "SubProjectTeams",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectBacklogs_SubProjects_SubProjectId",
                table: "ProjectBacklogs",
                column: "SubProjectId",
                principalTable: "SubProjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectBacklogs_SubProjects_SubProjectId",
                table: "ProjectBacklogs");

            migrationBuilder.DropTable(
                name: "SubProjectDependencies");

            migrationBuilder.DropTable(
                name: "SubProjectTeams");

            migrationBuilder.DropTable(
                name: "SubProjects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectBacklogs_SubProjectId",
                table: "ProjectBacklogs");

            migrationBuilder.DropColumn(
                name: "SubProjectId",
                table: "ProjectBacklogs");
        }
    }
}
