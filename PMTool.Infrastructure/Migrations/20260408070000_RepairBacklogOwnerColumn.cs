using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PMTool.Infrastructure.Data;

#nullable disable

namespace PMTool.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260408070000_RepairBacklogOwnerColumn")]
public partial class RepairBacklogOwnerColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('ProjectBacklogs', 'OwnerId') IS NULL
BEGIN
    ALTER TABLE [ProjectBacklogs] ADD [OwnerId] uniqueidentifier NULL;
END
");

        migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ProjectBacklogs_OwnerId'
      AND object_id = OBJECT_ID(N'[ProjectBacklogs]')
)
BEGIN
    CREATE INDEX [IX_ProjectBacklogs_OwnerId] ON [ProjectBacklogs]([OwnerId]);
END
");

        migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_ProjectBacklogs_Users_OwnerId'
)
BEGIN
    ALTER TABLE [ProjectBacklogs] WITH CHECK
    ADD CONSTRAINT [FK_ProjectBacklogs_Users_OwnerId]
    FOREIGN KEY([OwnerId]) REFERENCES [Users]([Id]) ON DELETE SET NULL;
END
");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_ProjectBacklogs_Users_OwnerId'
)
BEGIN
    ALTER TABLE [ProjectBacklogs] DROP CONSTRAINT [FK_ProjectBacklogs_Users_OwnerId];
END
");

        migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_ProjectBacklogs_OwnerId'
      AND object_id = OBJECT_ID(N'[ProjectBacklogs]')
)
BEGIN
    DROP INDEX [IX_ProjectBacklogs_OwnerId] ON [ProjectBacklogs];
END
");

        migrationBuilder.Sql(@"
IF COL_LENGTH('ProjectBacklogs', 'OwnerId') IS NOT NULL
BEGIN
    ALTER TABLE [ProjectBacklogs] DROP COLUMN [OwnerId];
END
");
    }
}
