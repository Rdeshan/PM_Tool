# Database Migration Guide - RBAC Implementation

## Overview
This guide explains the database migrations for the Role-Based Access Control (RBAC) system that was added in Phase 2.

## Migration Files Created

### 1. `20250116000001_AddRBACEntities.cs`
The main migration file that creates all RBAC tables and their relationships.

### 2. `AppDbContextModelSnapshot.cs`
The snapshot of the current database model used by EF Core to track changes.

---

## Tables Created

### 1. **Permissions Table**
Stores all available permissions in the system.

```sql
CREATE TABLE [Permissions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [PermissionType] INT NOT NULL UNIQUE,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
);
```

**Permissions Included:**
- User Management: ManageUsers, ManageRoles, ViewUsers
- Project Management: CreateProject, EditProject, DeleteProject, ViewProject, ManageProjectMembers
- Product Management: CreateProduct, EditProduct, DeleteProduct, ViewProduct
- Sub-Project Management: CreateSubProject, EditSubProject, DeleteSubProject, ViewSubProject
- Sprint Management: CreateSprint, EditSprint, DeleteSprint, ViewSprint
- Milestone Management: CreateMilestone, EditMilestone, DeleteMilestone, ViewMilestone
- Ticket Management: CreateTicket, EditTicket, DeleteTicket, ViewTicket, UpdateTicketStatus, AssignTicket
- Bug Management: CreateBugTicket, EditBugTicket, DeleteBugTicket, LinkTestCases
- User Story Management: CreateUserStory, EditUserStory, DeleteUserStory, ViewUserStory
- BRD Management: CreateBRD, EditBRD, DeleteBRD, ViewBRD
- Time Tracking: LogTime, ViewTimeLog
- Comments: PostComment, EditComment, DeleteComment
- Reporting: ViewReports, ViewDashboards, ExportData
- Organization: ManageOrganization, ManageOrganizationSettings
- System: ViewSystemLogs, ManageSystemSettings

### 2. **Roles Table**
Stores role definitions and their metadata.

```sql
CREATE TABLE [Roles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [RoleType] INT NOT NULL UNIQUE,
    [IsSystemRole] BIT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
);
```

**System Roles Created:**
1. **Administrator** (RoleType=1) - Full system access
2. **ProjectManager** (RoleType=2) - Project and team management
3. **Developer** (RoleType=3) - Create/update tickets, log time
4. **QAEngineer** (RoleType=4) - Bug management, test case linking
5. **BusinessAnalyst** (RoleType=5) - BRD and user story management
6. **Viewer** (RoleType=6) - Read-only access to reports/dashboards
7. **Guest** (RoleType=7) - Limited project access via invite

### 3. **UserRoles Table**
Junction table linking users to roles (supports project-specific roles).

```sql
CREATE TABLE [UserRoles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY (Users),
    [RoleId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY (Roles),
    [ProjectId] UNIQUEIDENTIFIER NULL,  -- NULL = organization-wide role
    [IsActive] BIT NOT NULL DEFAULT 1,
    [AssignedAt] DATETIME2 DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    UNIQUE(UserId, RoleId, ProjectId)
);
```

**Key Features:**
- Supports organization-wide roles (ProjectId = NULL)
- Supports project-specific roles (ProjectId = specific project)
- Users can have different roles on different projects
- Unique constraint prevents duplicate assignments

### 4. **RolePermissions Table**
Junction table mapping roles to permissions.

```sql
CREATE TABLE [RolePermissions] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [RoleId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY (Roles),
    [PermissionId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY (Permissions),
    [IsGranted] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
    UNIQUE(RoleId, PermissionId)
);
```

---

## How to Apply Migrations

### Option 1: Automatic Migration (Recommended)
The app automatically applies migrations on startup:

```csharp
// In Program.cs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();  // Creates all tables
}
```

**Steps:**
1. Run the app (`dotnet run` or F5 in Visual Studio)
2. Migrations apply automatically
3. Database tables are created
4. Default roles are initialized

### Option 2: Manual Migration via Package Manager Console

```powershell
# In Package Manager Console (Tools > NuGet Package Manager)

# Update database
Update-Database

# Specific migration
Update-Database -Migration AddRBACEntities
```

### Option 3: Manual Migration via dotnet CLI

```powershell
# In PowerShell at project root
cd PMTool.Web

# Apply all pending migrations
dotnet ef database update --project ../PMTool.Infrastructure

# Apply specific migration
dotnet ef database update AddRBACEntities --project ../PMTool.Infrastructure
```

### Option 4: SQL Script Generation

Generate SQL script without applying immediately:

```powershell
# Generate SQL script
dotnet ef migrations script AddRBACEntities -o migration.sql --project ../PMTool.Infrastructure

# Or to script from migration to current
dotnet ef migrations script --idempotent -o all_migrations.sql --project ../PMTool.Infrastructure
```

---

## Verification

After migration, verify tables were created:

```sql
-- Check if RBAC tables exist
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Permissions', 'Roles', 'UserRoles', 'RolePermissions');

-- View table structure
EXEC sp_help 'Roles';
EXEC sp_help 'Permissions';
EXEC sp_help 'UserRoles';
EXEC sp_help 'RolePermissions';

-- Count default roles created
SELECT COUNT(*) as RoleCount FROM Roles;
SELECT * FROM Roles;

-- Check permissions
SELECT COUNT(*) as PermissionCount FROM Permissions;
```

---

## Default Roles & Permissions

After migration, these roles are automatically created with their permissions:

### Administrator
- All permissions (full system access)
- Can manage users, roles, organization settings

### Project Manager
- Project CRUD operations
- Product management
- Sprint and milestone management
- Team member assignment
- Can view reports and dashboards

### Developer
- Create/edit tickets in assigned sub-projects
- Update ticket status
- Log time entries
- Post comments
- View reports and dashboards

### QA Engineer
- Create/edit bug tickets
- Link test cases to tickets
- Update ticket status
- View project/sprint/ticket info
- Post comments

### Business Analyst
- Create/edit user stories and BRDs
- Manage backlog items
- View project/ticket information
- Post comments
- Link requirements to tickets

### Viewer / Stakeholder
- Read-only access to projects, sprints, tickets
- Can view reports and dashboards
- Cannot create or modify any content

### Guest
- Limited read-only access (only via invite link)
- Can view specific project and its tickets
- Can view reports
- No system account required

---

## Assigning Roles to Users

### In Code
```csharp
// Get authorization service
var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

// Assign organization-wide role
await authService.AssignRoleToUserAsync(userId, RoleType.Developer, projectId: null);

// Assign project-specific role
await authService.AssignRoleToUserAsync(userId, RoleType.ProjectManager, projectId);
```

### Via Database
```sql
-- Assign Developer role organization-wide to user
INSERT INTO UserRoles (Id, UserId, RoleId, ProjectId, IsActive, AssignedAt, UpdatedAt)
SELECT NEWID(), 
       @UserId,
       Id,
       NULL,
       1,
       GETUTCDATE(),
       GETUTCDATE()
FROM Roles WHERE RoleType = 3;

-- Assign ProjectManager role for specific project
INSERT INTO UserRoles (Id, UserId, RoleId, ProjectId, IsActive, AssignedAt, UpdatedAt)
SELECT NEWID(),
       @UserId,
       Id,
       @ProjectId,
       1,
       GETUTCDATE(),
       GETUTCDATE()
FROM Roles WHERE RoleType = 2;
```

---

## Checking Permissions

### In Code
```csharp
var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

// Check if user has specific permission
bool hasPermission = await authService.HasPermissionAsync(userId, PermissionType.CreateProject);

// Check if user has specific role
bool isAdmin = await authService.HasRoleAsync(userId, RoleType.Administrator);

// Get all user permissions
var permissions = await authService.GetUserPermissionsAsync(userId);

// Get user roles for specific project
var projectRoles = await authService.GetUserProjectRolesAsync(userId, projectId);
```

### Via Database
```sql
-- Get all permissions for a user
SELECT DISTINCT p.*
FROM Permissions p
INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
INNER JOIN Roles r ON rp.RoleId = r.Id
INNER JOIN UserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId AND ur.IsActive = 1;

-- Get user's roles
SELECT r.*
FROM Roles r
INNER JOIN UserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId AND ur.IsActive = 1;

-- Get roles for a specific project
SELECT r.*
FROM Roles r
INNER JOIN UserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId AND ur.ProjectId = @ProjectId AND ur.IsActive = 1;
```

---

## Rollback (If Needed)

### Via Package Manager Console
```powershell
# Rollback last migration
Update-Database -Migration 0  # or specify previous migration

# Rollback specific migration
Update-Database -Migration PreviousMigrationName
```

### Via dotnet CLI
```powershell
# Remove last migration (if not applied yet)
dotnet ef migrations remove --project ../PMTool.Infrastructure

# Rollback database to previous migration
dotnet ef database update PreviousMigrationName --project ../PMTool.Infrastructure
```

---

## Database Schema Diagram

```
Users (1) ──────── (Many) UserRoles (Many) ──────── (1) Roles (Many) ──────── (Many) Permissions
                                                            │
                                                            │
                                                       RolePermissions
                                                            │
                                                            └─────────────────────┘
```

---

## Best Practices

1. **Always backup database** before applying migrations in production
2. **Test migrations** in development/staging first
3. **Review migration files** before applying
4. **Keep migration history** - don't delete old migration files
5. **Use meaningful migration names** - e.g., `AddRBACEntities`
6. **Document custom migrations** with comments

---

## Troubleshooting

### Migration fails: "Cannot find migration"
**Solution:** Ensure migration files are in correct folder and named correctly

### "The target database already exists"
**Solution:** Delete the database and let EF recreate it, or use `Update-Database -Force`

### Foreign key constraint errors
**Solution:** Ensure related records exist before inserting (roles before user roles, etc.)

### Permission denied errors
**Solution:** Ensure SQL Server user has necessary permissions on the database

---

## Next Steps

1. ✅ Apply migration to database
2. ✅ Verify tables created successfully
3. ✅ Test role assignment functionality
4. ✅ Implement permission checking in Razor Pages
5. ✅ Create role management UI pages

See `RBAC_IMPLEMENTATION.md` for frontend implementation details.
