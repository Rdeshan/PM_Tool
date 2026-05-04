# Product Backlog Database Migration

## Migration Overview

A new migration file has been created to add the `StoryPoints` column to the ProductBacklogs table.

### File: `PMTool.Infrastructure\Migrations\20250105_AddStoryPointsToProductBacklog.cs`

## Migration SQL (Generated)

```sql
-- Migration Up
ALTER TABLE [ProductBacklogs]
ADD [StoryPoints] int NOT NULL DEFAULT 0;

-- Migration Down
ALTER TABLE [ProductBacklogs]
DROP COLUMN [StoryPoints];
```

## Applying the Migration

### Option 1: Using Package Manager Console (Recommended)

```powershell
# Open Package Manager Console in Visual Studio
# Tools > NuGet Package Manager > Package Manager Console

# Run migration
Update-Database -Project PMTool.Infrastructure
```

### Option 2: Using .NET CLI

```bash
# Navigate to project directory
cd PMTool.Infrastructure

# Apply all pending migrations
dotnet ef database update

# Or specific migration
dotnet ef database update AddStoryPointsToProductBacklog
```

### Option 3: Using Visual Studio

1. Open Package Manager Console
2. Select `PMTool.Infrastructure` as the Default project
3. Run: `Update-Database`

## Verification

After migration runs, verify the column was added:

```sql
-- Check column exists
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ProductBacklogs' AND COLUMN_NAME = 'StoryPoints';

-- Expected result:
-- COLUMN_NAME  | DATA_TYPE | IS_NULLABLE
-- StoryPoints  | int       | NO
```

## Column Details

| Property | Value |
|----------|-------|
| Column Name | StoryPoints |
| Data Type | int |
| Nullable | No |
| Default Value | 0 |
| Max Length | N/A |

## Rollback (If Needed)

To rollback this migration if needed:

### Package Manager Console
```powershell
Update-Database -Project PMTool.Infrastructure -TargetMigration [PreviousMigrationName]
```

### .NET CLI
```bash
dotnet ef database update [PreviousMigrationName]
```

## Database State After Migration

### ProductBacklogs Table Structure

```sql
CREATE TABLE [ProductBacklogs] (
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [ParentBacklogItemId] uniqueidentifier NULL,
    [OwnerId] uniqueidentifier NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Type] int NOT NULL,
    [Priority] int NOT NULL,
    [Status] int NOT NULL,
    [StartDate] datetime2 NULL,
    [DueDate] datetime2 NULL,
    [StoryPoints] int NOT NULL DEFAULT 0,  -- NEW
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ProductBacklogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductBacklogs_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
```

## Data Preservation

- ✅ Existing backlog items will NOT be deleted
- ✅ All existing data preserved
- ✅ StoryPoints default to 0 for existing items
- ✅ No data loss migration

## Indexes

The migration does not create any additional indexes on the StoryPoints column. If you plan to frequently query or sort by story points, consider adding an index:

```sql
CREATE INDEX [IX_ProductBacklogs_StoryPoints] 
ON [ProductBacklogs] ([StoryPoints]);
```

## Performance Impact

- ✅ Minimal: Only adds one int column
- ✅ No table restructuring required
- ✅ No data reorganization needed
- ✅ Estimated time: < 1 second on any size database

## Troubleshooting

### Migration Not Applying?

1. **Check Connection String**
   - Verify `appsettings.json` has correct connection string
   - Ensure database server is accessible

2. **Clear EF Core Cache**
   ```powershell
   Remove-Item -Path bin -Recurse -Force
   Remove-Item -Path obj -Recurse -Force
   ```

3. **Rebuild Solution**
   - Clean > Rebuild in Visual Studio

### Error: "Cannot find migration"

- Ensure you're in correct project directory
- Verify migration file exists in Migrations folder
- Check default project is set to PMTool.Infrastructure

### Error: "An instance already exists"

- Close other Package Manager Console windows
- Wait a few seconds and try again

## Related Files

- Migration: `PMTool.Infrastructure\Migrations\20250105_AddStoryPointsToProductBacklog.cs`
- Entity: `PMTool.Domain\Entities\ProductBacklog.cs`
- DbContext: `PMTool.Infrastructure\Data\AppDbContext.cs`

## Next Steps

1. Apply migration: `Update-Database`
2. Verify column creation
3. Test creating backlog items with story points
4. Deploy to other environments (dev, staging, production)

## Deployment Checklist

- [ ] Migration applied to development database
- [ ] Backend code deployed
- [ ] Frontend updated with story points UI
- [ ] Test create/update operations
- [ ] Test filtering by story points
- [ ] Backup production database before migration
- [ ] Apply migration to production
- [ ] Monitor for errors
- [ ] Update documentation

## Notes

- This migration is **required** for the product backlog functionality to work
- The StoryPoints field is used for planning and progress calculation
- Default value of 0 means items can be created without specifying story points
- Story points must be 0 or positive (validated in service layer)

## Additional Resources

- [EF Core Migrations Documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)
