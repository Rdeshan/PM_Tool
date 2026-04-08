using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PMTool.Infrastructure.Data;

#nullable disable

namespace PMTool.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260408103000_AddProjectDocuments")]
public partial class AddProjectDocuments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProjectDocuments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DocumentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                FileSize = table.Column<long>(type: "bigint", nullable: false),
                SubmittedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProjectDocuments", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProjectDocuments_Projects_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "Projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ProjectDocuments_Users_SubmittedByUserId",
                    column: x => x.SubmittedByUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProjectDocuments_ProjectId",
            table: "ProjectDocuments",
            column: "ProjectId");

        migrationBuilder.CreateIndex(
            name: "IX_ProjectDocuments_SubmittedByUserId",
            table: "ProjectDocuments",
            column: "SubmittedByUserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProjectDocuments");
    }
}
