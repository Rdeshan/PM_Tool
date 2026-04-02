# Quick Migration Commands

## ⚡ Fastest Way - Let App Auto-Apply

Just run the app! Migrations apply automatically on startup:

```powershell
dotnet run --project PMTool.Web
# or press F5 in Visual Studio
```

✅ Database created  
✅ RBAC tables created  
✅ Default roles initialized  

---

## 🔧 Manual Migration (Package Manager Console)

Open **Tools → NuGet Package Manager → Package Manager Console** and run:

```powershell
# Apply all pending migrations
Update-Database

# Verify migration applied
Get-Migrations
```

---

## 🛠️ Manual Migration (PowerShell CLI)

```powershell
# Navigate to web project
cd PMTool.Web

# Apply migrations
dotnet ef database update --project ../PMTool.Infrastructure

# Check status
dotnet ef migrations list --project ../PMTool.Infrastructure
```

---

## 📋 What Gets Created

After migration, your database will have:

```
✅ Permissions table (142 permissions for all features)
✅ Roles table (7 default system roles)
✅ UserRoles table (links users to roles)
✅ RolePermissions table (maps role permissions)
```

### 7 Default Roles Created:
1. **Administrator** - Full system access
2. **ProjectManager** - Create/manage projects
3. **Developer** - Create/update tickets, log time
4. **QAEngineer** - Bug management, test cases
5. **BusinessAnalyst** - User stories, BRDs
6. **Viewer** - Read-only dashboards
7. **Guest** - Limited project access

---

## ✅ Verify Migration Worked

In **SQL Server Management Studio**, run:

```sql
-- Check tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('Permissions', 'Roles', 'UserRoles', 'RolePermissions');

-- Check roles created
SELECT COUNT(*) as TotalRoles FROM Roles;
SELECT * FROM Roles;

-- Check permissions
SELECT COUNT(*) as TotalPermissions FROM Permissions;
```

**Expected Output:**
- 4 tables created ✅
- 7 default roles ✅
- 142 permissions ✅

---

## 🚀 Next: Test RBAC

1. **Register a new user**
2. **Assign role to user:**
   ```csharp
   var authService = serviceProvider.GetRequiredService<IAuthorizationService>();
   await authService.AssignRoleToUserAsync(userId, RoleType.Developer);
   ```
3. **Check permissions:**
   ```csharp
   bool canCreateTicket = await authService.HasPermissionAsync(userId, PermissionType.CreateTicket);
   ```

---

## ❌ If Something Goes Wrong

### Reset Database Completely
```powershell
# Remove all migrations (careful!)
Update-Database -Migration 0

# Or delete database and recreate
# Then run app - it will auto-apply migrations
```

### Check Migration Status
```powershell
# In Package Manager Console
Get-Migrations

# Via CLI
dotnet ef migrations list --project ../PMTool.Infrastructure
```

---

## 📖 Full Documentation

See `DATABASE_MIGRATION_GUIDE.md` for:
- Detailed table schemas
- SQL verification queries
- Permission assignment examples
- Troubleshooting guide
- Rollback procedures

---

**Status**: ✅ Ready to migrate!

Just run the app or use the commands above.
