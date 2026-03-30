# RBAC (Role-Based Access Control) Implementation - Phase 2 Complete

## 🎯 What's Been Implemented

### ✅ Database Layer
- **Permissions table** - 142 fine-grained permissions
- **Roles table** - 7 system roles with permission mappings
- **UserRoles table** - Links users to roles (supports project-specific roles)
- **RolePermissions table** - Maps roles to permissions
- **Migrations** - Automated database schema creation

### ✅ Domain Layer
- `Role` entity - Role definitions
- `Permission` entity - Permission definitions
- `UserRole` entity - User-role assignments
- `RolePermission` entity - Role-permission mappings
- Enums: `RoleType`, `PermissionType`

### ✅ Infrastructure Layer
- `IRoleRepository` & `RoleRepository` - Role CRUD
- `IUserRoleRepository` & `UserRoleRepository` - User role assignments
- `IPermissionRepository` & `PermissionRepository` - Permission access

### ✅ Application Layer
- `IRoleService` & `RoleService` - Role business logic
- `IAuthorizationService` & `AuthorizationService` - Permission checking
- DTOs: `RoleDTO`, `UserRoleDTO`, `AssignRoleRequest`

### ✅ Dependency Injection
- All RBAC services registered in `Program.cs`
- Ready to use in Razor Pages and APIs

---

## 🗂️ File Structure

```
PMTool/
├── PMTool.Domain/
│   ├── Entities/
│   │   ├── Role.cs ✅
│   │   ├── Permission.cs ✅
│   │   ├── UserRole.cs ✅
│   │   ├── RolePermission.cs ✅
│   │   └── User.cs (updated with navigation)
│   └── Enums/
│       └── RolePermission.cs (RoleType, PermissionType) ✅
│
├── PMTool.Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs (updated) ✅
│   │   └── Migrations/
│   │       ├── 20250116000001_AddRBACEntities.cs ✅
│   │       └── AppDbContextModelSnapshot.cs ✅
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   │   ├── IRoleRepository.cs ✅
│   │   │   ├── IUserRoleRepository.cs ✅
│   │   │   └── IPermissionRepository.cs ✅
│   │   ├── RoleRepository.cs ✅
│   │   ├── UserRoleRepository.cs ✅
│   │   └── PermissionRepository.cs ✅
│
├── PMTool.Application/
│   ├── Services/RBAC/
│   │   ├── IRoleService.cs ✅
│   │   ├── RoleService.cs ✅
│   │   ├── IAuthorizationService.cs ✅
│   │   └── AuthorizationService.cs ✅
│   └── DTOs/RBAC/
│       ├── RoleDTO.cs ✅
│       ├── UserRoleDTO.cs ✅
│       └── AssignRoleRequest.cs ✅
│
└── Documentation/
    ├── DATABASE_MIGRATION_GUIDE.md ✅
    ├── MIGRATION_QUICKSTART.md ✅
    └── This file
```

---

## 🚀 Getting Started

### Step 1: Apply Database Migration

**Option A: Automatic (Easiest)**
```powershell
# Just run the app - migrations apply automatically
dotnet run --project PMTool.Web
# or press F5 in Visual Studio
```

**Option B: Manual (Package Manager Console)**
```powershell
Update-Database
```

**Option C: Manual (CLI)**
```powershell
cd PMTool.Web
dotnet ef database update --project ../PMTool.Infrastructure
```

### Step 2: Verify Database

```sql
-- Check tables
SELECT * FROM Roles;
SELECT * FROM Permissions;

-- Should show 7 roles and 142 permissions
SELECT COUNT(*) FROM Roles;
SELECT COUNT(*) FROM Permissions;
```

### Step 3: Use in Your Code

```csharp
// Inject into Razor Page
public class MyPageModel : PageModel
{
    private readonly IAuthorizationService _authService;
    
    public MyPageModel(IAuthorizationService authService)
    {
        _authService = authService;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Redirect("/Auth/Login");
        
        // Check permission
        bool canCreateProject = await _authService.HasPermissionAsync(
            Guid.Parse(userId), 
            PermissionType.CreateProject
        );
        
        if (!canCreateProject)
            return Forbid();
            
        return Page();
    }
}
```

---

## 🛡️ Permission Checking Examples

### Check Single Permission
```csharp
bool hasPermission = await _authService.HasPermissionAsync(
    userId, 
    PermissionType.EditProject
);
```

### Check User Role
```csharp
bool isAdmin = await _authService.HasRoleAsync(
    userId, 
    RoleType.Administrator
);

// For project-specific role
bool isProjectManager = await _authService.HasRoleAsync(
    userId, 
    RoleType.ProjectManager, 
    projectId
);
```

### Get All User Permissions
```csharp
var permissions = await _authService.GetUserPermissionsAsync(userId);
foreach (var perm in permissions)
{
    Console.WriteLine($"{perm.Name}: {perm.Description}");
}
```

### Get User Roles
```csharp
// Organization-wide roles
var roles = await _authService.GetUserRolesAsync(userId);

// Project-specific roles
var projectRoles = await _authService.GetUserProjectRolesAsync(userId, projectId);
```

---

## 👥 7 System Roles

### 1. Administrator
- **Permissions:** Full system access
- **Can:** Manage users, roles, organization settings, view system logs
- **Use Case:** System admins

### 2. Project Manager
- **Permissions:** Create/edit/delete projects, manage team members, create sprints/milestones
- **Use Case:** Project leads, product owners

### 3. Developer
- **Permissions:** Create/edit tickets, log time, post comments
- **Use Case:** Software developers

### 4. QA Engineer
- **Permissions:** Create/edit bugs, link test cases, update ticket status
- **Use Case:** Quality assurance team

### 5. Business Analyst
- **Permissions:** Create/edit user stories and BRDs, manage backlog
- **Use Case:** Business analysts, product managers

### 6. Viewer / Stakeholder
- **Permissions:** Read-only access to projects, tickets, reports, dashboards
- **Use Case:** Stakeholders, executives

### 7. Guest
- **Permissions:** Limited read-only access to specific project via invite link
- **Use Case:** External collaborators, clients

---

## 🎓 Usage in Razor Pages

### Example: Project Management Page

```csharp
// Pages/Projects/ManageProjects.cshtml.cs
[Authorize]
public class ManageProjectsModel : PageModel
{
    private readonly IAuthorizationService _authService;
    private readonly IProjectService _projectService;
    
    public ManageProjectsModel(
        IAuthorizationService authService, 
        IProjectService projectService)
    {
        _authService = authService;
        _projectService = projectService;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Check if user can view projects
        bool canView = await _authService.HasPermissionAsync(
            Guid.Parse(userId),
            PermissionType.ViewProject
        );
        
        if (!canView)
            return Forbid();
        
        // Load projects
        var projects = await _projectService.GetUserProjectsAsync(Guid.Parse(userId));
        return Page();
    }
    
    public async Task<IActionResult> OnPostCreateAsync(ProjectCreateRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Check if user can create projects
        bool canCreate = await _authService.HasPermissionAsync(
            Guid.Parse(userId),
            PermissionType.CreateProject
        );
        
        if (!canCreate)
            return Forbid();
        
        await _projectService.CreateProjectAsync(request);
        return RedirectToPage();
    }
}
```

### Example: HTML with Conditional UI

```html
<!-- Pages/Projects/ManageProjects.cshtml -->
@page
@model ManageProjectsModel
@using System.Security.Claims
@{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var canCreate = await Model.AuthService.HasPermissionAsync(
        Guid.Parse(userId),
        PermissionType.CreateProject
    );
}

<h1>Projects</h1>

@if (canCreate)
{
    <a class="btn btn-primary" asp-page="./Create">Create Project</a>
}

<table class="table">
    @foreach (var project in Model.Projects)
    {
        <tr>
            <td>@project.Name</td>
            @if (canCreate)
            {
                <td>
                    <a asp-page="./Edit" asp-route-id="@project.Id">Edit</a>
                    <a asp-page="./Delete" asp-route-id="@project.Id">Delete</a>
                </td>
            }
        </tr>
    }
</table>
```

---

## 🔄 Assigning Roles to Users

### Programmatically
```csharp
var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

// Assign organization-wide role
await authService.AssignRoleToUserAsync(
    userId: new Guid("..."),
    roleType: RoleType.Developer,
    projectId: null
);

// Assign project-specific role
await authService.AssignRoleToUserAsync(
    userId: new Guid("..."),
    roleType: RoleType.ProjectManager,
    projectId: new Guid("...")
);
```

### Via SQL
```sql
-- Assign Developer role to user
INSERT INTO UserRoles (Id, UserId, RoleId, ProjectId, IsActive, AssignedAt, UpdatedAt)
SELECT NEWID(), 
       @UserId,
       Id,
       NULL,
       1,
       GETUTCDATE(),
       GETUTCDATE()
FROM Roles WHERE RoleType = 3;  -- Developer
```

---

## 📊 Permission Matrix

| Permission | Admin | PM | Dev | QA | BA | Viewer | Guest |
|-----------|-------|----|----|----|----|--------|-------|
| CreateProject | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| EditProject | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| ViewProject | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| CreateTicket | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| CreateBugTicket | ✅ | ✅ | ❌ | ✅ | ❌ | ❌ | ❌ |
| CreateUserStory | ✅ | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ |
| CreateBRD | ✅ | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ |
| ViewReports | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| ViewDashboards | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| LogTime | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| ManageUsers | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| ManageOrganization | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

---

## 🎯 Next Steps (Phase 3+)

### Immediate
- [ ] Create role management UI page (Admin only)
- [ ] Create user management UI with role assignment
- [ ] Add authorization checks to all Razor Pages
- [ ] Add permission checks to API endpoints

### Short-term
- [ ] Implement project-specific role UI
- [ ] Create permission audit log
- [ ] Add role templates for custom roles
- [ ] Implement delegation (e.g., PM can assign roles to their team)

### Long-term
- [ ] Add dynamic permission creation UI
- [ ] Implement permission inheritance
- [ ] Add time-limited role assignments
- [ ] Create role analytics dashboard

---

## ✅ Checklist

- [x] Database migration created
- [x] RBAC entities modeled
- [x] Repositories implemented
- [x] Services implemented
- [x] Dependency injection configured
- [x] 7 default roles created
- [x] 142 permissions defined
- [x] Documentation complete
- [ ] **Next: Apply migration to your database**
- [ ] Next: Test role assignment
- [ ] Next: Add authorization checks to pages
- [ ] Next: Create role management UI

---

## 🔗 Documentation Files

- **DATABASE_MIGRATION_GUIDE.md** - Complete migration reference
- **MIGRATION_QUICKSTART.md** - Quick commands to apply migrations
- **AUTHENTICATION_ARCHITECTURE.md** - Authentication system (Phase 1)
- **FRONTEND_IMPLEMENTATION.md** - Razor Pages frontend
- **EMAIL_SETUP_GUIDE.md** - Email service configuration

---

## 💡 Key Concepts

### Organization-wide vs Project-specific Roles
```csharp
// Organization-wide role (applies everywhere)
await authService.AssignRoleToUserAsync(userId, RoleType.Developer, projectId: null);

// Project-specific role (only for this project)
await authService.AssignRoleToUserAsync(userId, RoleType.ProjectManager, projectId);
```

### Permission Inheritance
- Permissions are defined at the database level
- Roles have multiple permissions
- Users have roles, so inherit all permissions from those roles
- Project-specific roles can override organization-wide roles

### User Can Have Multiple Roles
```sql
-- User is both QA Engineer and Developer
INSERT INTO UserRoles VALUES (NewId, @UserId, @QAEngineerId, NULL, 1, GETUTCDATE(), GETUTCDATE());
INSERT INTO UserRoles VALUES (NewId, @UserId, @DeveloperRoleId, NULL, 1, GETUTCDATE(), GETUTCDATE());
```

---

## 🚀 Ready to Deploy?

See **DEPLOYMENT_GUIDE.md** (coming in Phase 3) for production setup.

**Status**: ✅ Phase 2 Complete - RBAC Foundation Ready!
