# Project Management Authorization Guide

## Overview
Project creation and management features are restricted to **Administrators** and **Project Managers** only.

## Authorization Rules

### Project Creation
- **Allowed Roles**: Administrator, Project Manager
- **Restriction**: Only users with these roles can access `/Projects/Create`
- **Behavior**: Users without permission will see a 403 Forbidden error

### Project Editing
- **Allowed Roles**: Administrator, Project Manager
- **Restriction**: Edit button is disabled for other roles
- **Behavior**: Unauthorized users cannot modify project details

### Project Deletion
- **Allowed Roles**: Administrator, Project Manager
- **Restriction**: Delete button is disabled for other roles
- **Behavior**: POST request will return 403 Forbidden if attempted

### Project Archival
- **Allowed Roles**: Administrator, Project Manager
- **Restriction**: Archive button is disabled for other roles
- **Behavior**: POST request will return 403 Forbidden if attempted

### Project Viewing
- **Allowed Roles**: All authenticated users
- **Restriction**: All users can view projects (future: implement per-project permissions)
- **Behavior**: View button is always available

## Implementation Details

### Page-Level Authorization
Pages use the `[Authorize(Roles = "Administrator,Project Manager")]` attribute:

```csharp
[Authorize(Roles = "Administrator,Project Manager")]
public class CreateModel : PageModel
{
    // Page logic
}
```

### Action-Level Authorization
POST actions check roles at runtime:

```csharp
public async Task<IActionResult> OnPostArchiveAsync(Guid projectId)
{
    if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
    {
        return Forbid();
    }
    
    // Archive logic
}
```

### UI-Level Authorization
Razor pages check roles before rendering buttons:

```razor
@{
    var canCreateProject = User.IsInRole("Administrator") || 
                          User.IsInRole("Project Manager");
}

@if (canCreateProject)
{
    <a asp-page="./Create" class="btn btn-primary">New Project</a>
}
else
{
    <button class="btn btn-primary" disabled>New Project</button>
}
```

## Test Scenarios

### Admin User (Admin@pmtool.com)
✅ Can create projects
✅ Can edit projects
✅ Can delete projects
✅ Can archive projects
✅ Can view all projects

### Project Manager User (PM@pmtool.com)
✅ Can create projects
✅ Can edit projects
✅ Can delete projects
✅ Can archive projects
✅ Can view all projects

### Developer User (Dev@pmtool.com)
❌ Cannot create projects (403 Forbidden)
✅ Can view projects
❌ Edit button disabled
❌ Delete button disabled
❌ Archive button disabled

### QA Engineer (QA@pmtool.com)
❌ Cannot create projects (403 Forbidden)
✅ Can view projects
❌ Edit button disabled
❌ Delete button disabled
❌ Archive button disabled

### Business Analyst (BA@pmtool.com)
❌ Cannot create projects (403 Forbidden)
✅ Can view projects
❌ Edit button disabled
❌ Delete button disabled
❌ Archive button disabled

### Viewer (Viewer@pmtool.com)
❌ Cannot create projects (403 Forbidden)
✅ Can view projects
❌ Edit button disabled
❌ Delete button disabled
❌ Archive button disabled

### Guest (Guest@pmtool.com)
❌ Cannot create projects (403 Forbidden)
✅ Can view projects
❌ Edit button disabled
❌ Delete button disabled
❌ Archive button disabled

## Error Handling

### Access Denied (403)
When unauthorized user attempts to:
1. Access `/Projects/Create` directly
2. POST to create/edit/delete/archive endpoints

Result: 
```
403 Forbidden
Access to the resource is denied. This action is restricted to Administrators and Project Managers.
```

### Global Error Handling
Recommended to configure a 403 error page at `Pages/Error403.cshtml`:

```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePages(context =>
    {
        if (context.HttpContext.Response.StatusCode == 403)
        {
            context.HttpContext.Response.Redirect("/Error403");
        }
        return Task.CompletedTask;
    });
}
```

## Future Enhancements

1. **Project-Level Role Assignment**
   - Allow assigning users to projects with specific roles
   - Different permissions per project

2. **Role Hierarchy**
   - Admin inherits all Project Manager permissions
   - Custom role creation

3. **Audit Logging**
   - Log who created/modified/archived projects
   - Track authorization denials

4. **Time-Based Permissions**
   - Restrict editing after project completion
   - Restrict deletion after specific date

5. **Permission Management UI**
   - Admin interface to manage project permissions
   - Bulk role assignment

## Quick Testing

### Test with Admin Account
1. Login as `admin@pmtool.com` / `Admin@123`
2. Navigate to `/Projects`
3. Click "New Project" - ✅ Should work
4. Create a project
5. Click "Edit" - ✅ Should work
6. Click "Archive" - ✅ Should work

### Test with Developer Account
1. Login as `dev@pmtool.com` / `Dev@123`
2. Navigate to `/Projects`
3. Verify "New Project" button is disabled
4. Try accessing `/Projects/Create` directly - ✅ Should see 403
5. View project details
6. Verify "Edit" and "Delete" buttons are disabled

## Security Best Practices

✅ **Implemented:**
- Authorization on page load
- Authorization on POST actions
- UI control hiding for unauthorized users
- Role-based access control

⚠️ **To Be Implemented:**
- Resource-level authorization (checking if user owns the project)
- Audit logging of authorization failures
- Rate limiting for failed authorization attempts
- CORS authorization headers
