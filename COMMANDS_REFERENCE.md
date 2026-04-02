# 🛠️ Database Migration & RBAC Commands Reference

## 📋 Quick Commands

### 🚀 Apply Migrations (Choose ONE)

**Option 1: Automatic (EASIEST)**
```powershell
dotnet run --project PMTool.Web
# or press F5 in Visual Studio
```

**Option 2: Package Manager Console**
```powershell
# Tools > NuGet Package Manager > Package Manager Console
Update-Database
```

**Option 3: CLI**
```powershell
cd PMTool.Web
dotnet ef database update --project ../PMTool.Infrastructure
```

---

## ✅ Verify Installation

```sql
-- Check tables created
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Permissions', 'Roles', 'UserRoles', 'RolePermissions');

-- Check roles (should be 7)
SELECT COUNT(*) as RoleCount FROM Roles;
SELECT * FROM Roles;

-- Check permissions (should be 142)
SELECT COUNT(*) as PermissionCount FROM Permissions;

-- Show all system roles with permission counts
SELECT r.Name, COUNT(rp.PermissionId) as PermissionCount
FROM Roles r
LEFT JOIN RolePermissions rp ON r.Id = rp.RoleId
GROUP BY r.Name;
```

---

## 🔍 Migration Status

### View All Migrations
```powershell
# Package Manager Console
Get-Migrations

# CLI
dotnet ef migrations list --project ../PMTool.Infrastructure
```

### Check Current Database Migration
```powershell
# Package Manager Console (SQL Server)
SELECT * FROM __EFMigrationsHistory;

# Or from CLI
dotnet ef migrations script --idempotent --project ../PMTool.Infrastructure
```

---

## 🔧 Common Tasks

### Rollback Last Migration
```powershell
# Package Manager Console
Update-Database -Migration 0

# CLI - list migration before this one
dotnet ef migrations list --project ../PMTool.Infrastructure
# Then
dotnet ef database update <PreviousMigrationName> --project ../PMTool.Infrastructure
```

### Remove Last Migration (Not Applied)
```powershell
# Package Manager Console
Remove-Migration

# CLI
dotnet ef migrations remove --project ../PMTool.Infrastructure
```

### View SQL for Migration
```powershell
# Package Manager Console
Script-Migration -From <FromMigration> -To <ToMigration>

# CLI - Generate SQL script
dotnet ef migrations script -o migration.sql --project ../PMTool.Infrastructure
```

---

## 👤 User Role Management (SQL)

### Assign Role to User

**Developer Role (Organization-wide)**
```sql
INSERT INTO UserRoles (Id, UserId, RoleId, ProjectId, IsActive, AssignedAt, UpdatedAt)
SELECT NEWID(), 
       @UserId,
       r.Id,
       NULL,
       1,
       GETUTCDATE(),
       GETUTCDATE()
FROM Roles r WHERE r.RoleType = 3;  -- Developer
```

**Project Manager (Project-Specific)**
```sql
INSERT INTO UserRoles (Id, UserId, RoleId, ProjectId, IsActive, AssignedAt, UpdatedAt)
SELECT NEWID(),
       @UserId,
       r.Id,
       @ProjectId,
       1,
       GETUTCDATE(),
       GETUTCDATE()
FROM Roles r WHERE r.RoleType = 2;  -- ProjectManager
```

### View User Roles
```sql
-- All roles for a user
SELECT u.Email, r.Name, ur.ProjectId
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE u.Email = '@user@example.com';

-- User permissions
SELECT DISTINCT p.Name, p.Description
FROM Permissions p
INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
INNER JOIN Roles r ON rp.RoleId = r.Id
INNER JOIN UserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId AND ur.IsActive = 1;
```

### Remove Role from User
```sql
DELETE FROM UserRoles
WHERE UserId = @UserId AND RoleId = @RoleId;
```

---

## 📊 Role & Permission Reference

### All 7 System Roles

```sql
SELECT * FROM Roles ORDER BY RoleType;

-- Results:
-- 1. Administrator
-- 2. ProjectManager
-- 3. Developer
-- 4. QAEngineer
-- 5. BusinessAnalyst
-- 6. Viewer
-- 7. Guest
```

### All 142 Permissions

```sql
SELECT PermissionType, Name, Description FROM Permissions ORDER BY PermissionType;

-- Categories:
-- 1-9: User Management
-- 10-19: Project Management
-- 20-29: Product Management
-- 30-39: Sub-Project Management
-- 40-49: Sprint Management
-- 50-59: Milestone Management
-- 60-69: Ticket Management
-- 70-79: Bug Management
-- 80-89: User Story Management
-- 90-99: BRD Management
-- 100-109: Time Tracking
-- 110-119: Comments
-- 120-129: Reporting & Analytics
-- 130-139: Organization Management
-- 140-149: System Administration
```

---

## 💻 C# Usage Examples

### Check Permission
```csharp
var authService = app.Services.GetRequiredService<IAuthorizationService>();
var userId = Guid.Parse("user-id");

bool canCreateProject = await authService.HasPermissionAsync(userId, PermissionType.CreateProject);
if (!canCreateProject)
{
    // User doesn't have permission
}
```

### Check Role
```csharp
bool isAdmin = await authService.HasRoleAsync(userId, RoleType.Administrator);

// For project-specific role
bool isProjectManager = await authService.HasRoleAsync(
    userId, 
    RoleType.ProjectManager, 
    projectId
);
```

### Get User Roles
```csharp
var roles = await authService.GetUserRolesAsync(userId);

foreach (var role in roles)
{
    Console.WriteLine($"Role: {role.Name}");
    Console.WriteLine($"Permissions: {role.Permissions.Count()}");
}
```

### Assign Role
```csharp
var authService = app.Services.GetRequiredService<IAuthorizationService>();

await authService.AssignRoleToUserAsync(
    userId: new Guid("..."),
    roleType: RoleType.Developer,
    projectId: null  // Organization-wide
);
```

---

## 🧬 Database Diagram

```
┌────────────────────┐
│      Users         │
├────────────────────┤
│ Id (PK)            │
│ Email (Unique)     │
│ PasswordHash       │
│ FirstName          │
│ LastName           │
│ IsActive           │
│ ...auth fields...  │
└────────┬───────────┘
         │ (1:Many)
         │
         ▼
┌────────────────────┐
│    UserRoles       │
├────────────────────┤
│ Id (PK)            │
│ UserId (FK)        │
│ RoleId (FK)        │
│ ProjectId (FK)     │
│ IsActive           │
│ AssignedAt         │
└────────┬───────────┘
         │ (Many:1)
         │
         ▼
┌────────────────────┐         ┌─────────────────────┐
│      Roles         │─────────│ RolePermissions     │
├────────────────────┤         ├─────────────────────┤
│ Id (PK)            │         │ Id (PK)             │
│ Name (Unique)      │         │ RoleId (FK)         │
│ RoleType (Unique)  │         │ PermissionId (FK)   │
│ IsSystemRole       │         │ IsGranted           │
│ IsActive           │         └─────────────────────┘
│ CreatedAt          │                  │
│ UpdatedAt          │                  │
└────────────────────┘                  │
                                        │ (Many:Many)
                                        │
                                        ▼
                            ┌──────────────────────┐
                            │   Permissions       │
                            ├──────────────────────┤
                            │ Id (PK)              │
                            │ Name (Unique)        │
                            │ PermissionType (Unique)
                            │ IsActive             │
                            │ CreatedAt            │
                            │ UpdatedAt            │
                            └──────────────────────┘
```

---

## 🐛 Troubleshooting

### Migration Not Found
```powershell
# Check migrations folder exists
dir PMTool.Infrastructure/Migrations

# List all migrations
dotnet ef migrations list --project ../PMTool.Infrastructure
```

### Foreign Key Constraint Error
```powershell
# Check roles exist before assigning to users
SELECT COUNT(*) FROM Roles;
# Should return 7

# If 0, run migration again
Update-Database
```

### Tables Not Visible
```powershell
# Refresh database connection
# Or restart Visual Studio
# Or reconnect to database

# Verify tables via SQL
SELECT * FROM INFORMATION_SCHEMA.TABLES;
```

### Permission Denied
```powershell
# Check SQL Server login permissions
# Ensure user has db_owner or appropriate permissions on database
```

---

## 📝 Common SQL Queries

### Get All Roles with Permissions
```sql
SELECT 
    r.Id,
    r.Name,
    r.RoleType,
    COUNT(rp.PermissionId) as PermissionCount,
    STRING_AGG(p.Name, ', ') as Permissions
FROM Roles r
LEFT JOIN RolePermissions rp ON r.Id = rp.RoleId
LEFT JOIN Permissions p ON rp.PermissionId = p.Id
GROUP BY r.Id, r.Name, r.RoleType;
```

### Get Users by Role
```sql
SELECT 
    u.Id,
    u.Email,
    r.Name as RoleName,
    ur.ProjectId,
    ur.IsActive
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
INNER JOIN Roles r ON ur.RoleId = r.Id
WHERE r.RoleType = 3  -- Developer
ORDER BY u.Email;
```

### Get Role's Permissions
```sql
SELECT 
    p.PermissionType,
    p.Name,
    p.Description
FROM Permissions p
INNER JOIN RolePermissions rp ON p.Id = rp.PermissionId
WHERE rp.RoleId = @RoleId
ORDER BY p.PermissionType;
```

### User Has Permission Check
```sql
SELECT COUNT(*) as HasPermission
FROM Users u
INNER JOIN UserRoles ur ON u.Id = ur.UserId
INNER JOIN Roles r ON ur.RoleId = r.Id
INNER JOIN RolePermissions rp ON r.Id = rp.RoleId
INNER JOIN Permissions p ON rp.PermissionId = p.Id
WHERE u.Id = @UserId 
AND p.PermissionType = @PermissionType
AND ur.IsActive = 1
AND r.IsActive = 1
AND p.IsActive = 1;
```

---

## 📚 Documentation Links

- **Quick Start**: QUICKSTART.md
- **Migration Guide**: DATABASE_MIGRATION_GUIDE.md
- **RBAC Guide**: RBAC_IMPLEMENTATION.md
- **Phase 2 Summary**: PHASE2_COMPLETE_SUMMARY.md
- **Authentication**: AUTHENTICATION_ARCHITECTURE.md
- **Email Setup**: EMAIL_SETUP_GUIDE.md

---

**Last Updated**: January 16, 2025  
**Status**: ✅ Ready for Migration
