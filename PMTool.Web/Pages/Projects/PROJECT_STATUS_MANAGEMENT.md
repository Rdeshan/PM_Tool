# Project Status Management Implementation

## Overview
Implemented project status management where:
- All new projects are created with **"Active"** status by default
- Only **Administrators** and **Project Managers** can change project status
- Regular users can only view project status (read-only)
- Project status options: Active (1), On Hold (2), Completed (3)

## Changes Made

### 1. **New DTO: UpdateProjectRequest**
**File:** `PMTool.Application\DTOs\Project\UpdateProjectRequest.cs`

Created a dedicated DTO for project updates that includes the Status field:

```csharp
public class UpdateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public string? ColourCode { get; set; }
    public int Status { get; set; }
}
```

### 2. **ProjectService Updates**
**File:** `PMTool.Application\Services\Project\ProjectService.cs`

**Updated Interface:**
- Changed `UpdateProjectAsync` signature to accept `UpdateProjectRequest` instead of `CreateProjectRequest`

**Updated Implementation:**
- `UpdateProjectAsync` now updates the project status along with other fields
- Status is persisted to the database

```csharp
public async Task<bool> UpdateProjectAsync(Guid id, UpdateProjectRequest request)
{
    var project = await _projectRepository.GetByIdAsync(id);
    if (project == null)
        return false;

    // ... code validation ...

    project.Name = request.Name;
    project.Description = request.Description;
    project.ClientName = request.ClientName;
    project.ProjectCode = request.ProjectCode;
    project.StartDate = request.StartDate;
    project.ExpectedEndDate = request.ExpectedEndDate;
    project.ColourCode = request.ColourCode;
    project.Status = request.Status;  // ← New: Status field

    return await _projectRepository.UpdateAsync(project);
}
```

**CreateProjectAsync Already Sets Active Status:**
```csharp
public async Task<bool> CreateProjectAsync(CreateProjectRequest request, Guid createdByUserId)
{
    var project = new Domain.Entities.Project
    {
        // ... other fields ...
        Status = 1 // Active - Default status for new projects
    };

    return await _projectRepository.CreateAsync(project);
}
```

### 3. **Edit Page Code-Behind Updates**
**File:** `PMTool.Web\Pages\Projects\Edit.cshtml.cs`

**Key Changes:**
- Changed `Input` property type from `CreateProjectRequest` to `UpdateProjectRequest`
- Load current project status on GET
- Pass status to service on POST
- Validate using `CreateProjectRequestValidator` with converted data
- Authorization: Only Admins and Project Managers via `[Authorize(Roles = "Administrator,Project Manager")]`

```csharp
[Authorize(Roles = "Administrator,Project Manager")]
public class EditModel : PageModel
{
    public UpdateProjectRequest Input { get; set; } = new();
    
    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Project = await _projectService.GetProjectByIdAsync(id);
        Input = new UpdateProjectRequest
        {
            // ... other fields ...
            Status = Project.Status  // Load current status
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        // ... validation ...
        
        var result = await _projectService.UpdateProjectAsync(id, Input);
        
        // ... handle result ...
    }
}
```

### 4. **Edit Page View Updates**
**File:** `PMTool.Web\Pages\Projects\Edit.cshtml`

**Added Project Status Dropdown:**
```html
<!-- Project Status -->
<div class="mb-3">
    <label asp-for="Input.Status" class="form-label">Project Status <span class="text-danger">*</span></label>
    <select asp-for="Input.Status" class="form-select form-select-lg">
        <option value="1">Active</option>
        <option value="2">On Hold</option>
        <option value="3">Completed</option>
    </select>
    <span asp-validation-for="Input.Status" class="text-danger small"></span>
    <small class="text-muted d-block mt-2">Only Administrators and Project Managers can change project status</small>
</div>
```

**Features:**
- Dropdown with three status options
- Clear labeling and validation messages
- Helper text explaining access restrictions
- Positioned after dates and before colour selector

## Project Status Enum
**File:** `PMTool.Domain\Enums\ProjectStatus.cs`

```csharp
public enum ProjectStatus
{
    Active = 1,
    OnHold = 2,
    Completed = 3
}
```

## User Access Control

### Administrators and Project Managers
✅ Can create projects (automatically set to Active)
✅ Can edit project details
✅ Can change project status (Active ↔ On Hold ↔ Completed)
✅ Can delete projects
✅ See status dropdown in edit form

### Regular Users
✅ Can view projects and their status
✅ Can see project details
❌ Cannot access edit page (401 Unauthorized)
❌ Cannot create projects
❌ Cannot change project status
❌ Cannot delete projects

## Authorization Flow

```
User Login
    ↓
If Admin/PM → Can access Edit page (Shows status dropdown)
If Regular User → Cannot access Edit page (401 Unauthorized)
```

## Default Project Creation Flow

```
Create New Project
    ↓
Set all required fields (Name, Code, Dates, etc.)
    ↓
Status = 1 (Active) [Automatic - Not shown in create form]
    ↓
Project saved with Active status
    ↓
Only Admins/PMs can change to On Hold or Completed later
```

## Status Change Flow

```
Admin/PM Opens Edit Page
    ↓
Current status displayed in dropdown
    ↓
Select new status (Active/On Hold/Completed)
    ↓
Update Project
    ↓
Status persisted to database
    ↓
User redirected to project details
```

## Database Persistence

The `Project` entity already has a `Status` property (int):
```csharp
public int Status { get; set; }
```

No database migration needed - the structure already supports status tracking.

## Benefits

✅ **Clean Access Control** - Role-based status changes
✅ **Default Active State** - No need to set status on creation
✅ **User-Friendly** - Clear dropdown with three readable options
✅ **Audit Trail Ready** - Status changes can be logged
✅ **Scalable** - Easy to add more statuses in the future
✅ **Consistent** - All projects start in the same state

## Testing Checklist

- [ ] Create a project as Admin → Verify status is Active
- [ ] Create a project as PM → Verify status is Active
- [ ] Edit project as Admin → Change status to On Hold → Verify saved
- [ ] Edit project as PM → Change status to Completed → Verify saved
- [ ] Try to access edit page as Regular User → Verify 401 Unauthorized
- [ ] View project details as Regular User → Verify status displayed (read-only)
- [ ] Verify status options in dropdown: Active, On Hold, Completed
