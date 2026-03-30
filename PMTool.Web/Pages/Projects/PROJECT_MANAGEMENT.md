# Project Creation & Settings - Complete Implementation

## Overview
This document describes the complete implementation of Project Creation & Settings feature as per Section 3.1 of the PM Tool specification.

## Features Implemented

### 1. **Project Creation**
- **Create Project**: Any Project Manager or Administrator can create a project with:
  - Project Name (required, max 200 chars)
  - Description (optional, max 1000 chars)
  - Client Name (required, max 200 chars)
  - Project Code (required, 3-20 uppercase letters/numbers, unique)
  - Start Date (required)
  - Expected End Date (required, must be after start date)
  - Project Colour (hex colour code for visual identification)

### 2. **Project Code System**
- Each project gets a unique short code (e.g., 'UMS')
- Used as prefix for all ticket IDs (e.g., UMS-001, UMS-002)
- Validated to be 3-20 uppercase alphanumeric characters
- Unique constraint enforced at database level

### 3. **Project Status Management**
- **Active** - Project is currently running
- **On Hold** - Project is temporarily paused
- **Completed** - Project is finished
- Status displayed with colour-coded badges

### 4. **Project Avatar & Colour**
- Each project has a custom colour (hex code) for quick visual identification
- Displayed in project cards and details page
- Colour picker integrated in create/edit forms

### 5. **Project Archival**
- Completed projects can be archived
- Archived projects are read-only
- Archived projects remain fully searchable
- Separate "Archived" tab in Projects page

### 6. **Team Assignment**
- Teams and individual users can be assigned to projects
- Controls who can access the project
- View team members on project details page
- Add/remove team members functionality

### 7. **Project-Level Backlog**
- Each project has a top-level backlog
- Can contain BRDs, user stories, and high-level requirements
- Accessible from project details page
- Prepared for future implementation

## Database Schema

### Projects Table
```sql
CREATE TABLE Projects (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    ClientName NVARCHAR(200) NOT NULL,
    ProjectCode NVARCHAR(20) NOT NULL UNIQUE,
    Status INT DEFAULT 1, -- 1=Active, 2=OnHold, 3=Completed
    AvatarUrl NVARCHAR(MAX),
    ColourCode NVARCHAR(7), -- Hex colour code
    IsArchived BIT DEFAULT 0,
    StartDate DATETIME2 NOT NULL,
    ExpectedEndDate DATETIME2 NOT NULL,
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
)

CREATE INDEX IX_Projects_ProjectCode ON Projects(ProjectCode) UNIQUE
```

### ProjectBacklogs Table
```sql
CREATE TABLE ProjectBacklogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProjectId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(500) NOT NULL,
    Description NVARCHAR(2000),
    Type INT, -- BRD=1, UserStory=2, Requirement=3, etc.
    Priority INT, -- High=1, Medium=2, Low=3
    Status INT, -- Draft=1, Active=2, Completed=3, Archived=4
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE(),
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE
)
```

## API Endpoints / Pages

### Projects Management
| Page | Route | Method | Description |
|------|-------|--------|-------------|
| Index | `/Projects` | GET | List all projects with filter tabs |
| Create | `/Projects/Create` | GET/POST | Create new project form |
| Details | `/Projects/Details/{id}` | GET | View project details |
| Edit | `/Projects/Edit/{id}` | GET/POST | Edit project information |
| Archive | `/Projects/Index` | POST | Archive a project |

## Key Services

### IProjectService
```csharp
public interface IProjectService
{
    Task<ProjectDTO?> GetProjectByIdAsync(Guid id);
    Task<ProjectDTO?> GetProjectByCodeAsync(string projectCode);
    Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
    Task<IEnumerable<ProjectDTO>> GetActiveProjectsAsync();
    Task<IEnumerable<ProjectDTO>> GetUserProjectsAsync(Guid userId);
    Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync();
    Task<bool> CreateProjectAsync(CreateProjectRequest request, Guid createdByUserId);
    Task<bool> UpdateProjectAsync(Guid id, CreateProjectRequest request);
    Task<bool> DeleteProjectAsync(Guid id);
    Task<bool> ArchiveProjectAsync(Guid id);
    Task<bool> ProjectCodeExistsAsync(string projectCode);
    Task<IEnumerable<User>> GetProjectTeamAsync(Guid projectId);
    Task<bool> AddTeamMemberAsync(Guid projectId, Guid userId, Guid roleId);
    Task<bool> RemoveTeamMemberAsync(Guid projectId, Guid userId);
}
```

## DTOs

### CreateProjectRequest
```csharp
public class CreateProjectRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ClientName { get; set; }
    public string ProjectCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public string? ColourCode { get; set; }
}
```

### ProjectDTO
```csharp
public class ProjectDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ClientName { get; set; }
    public string ProjectCode { get; set; }
    public int Status { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ColourCode { get; set; }
    public bool IsArchived { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Validation Rules

### CreateProjectRequestValidator
- **Name**: Required, max 200 characters
- **ProjectCode**: Required, 3-20 uppercase alphanumeric characters (validated with regex)
- **ClientName**: Required, max 200 characters
- **Description**: Optional, max 1000 characters
- **StartDate**: Required, must be before ExpectedEndDate
- **ExpectedEndDate**: Required, must be after StartDate
- **ColourCode**: Optional, must be valid hex colour if provided (#RRGGBB format)

## User Permissions (Future Implementation)
- **Administrator**: Can create, edit, delete, and archive any project
- **Project Manager**: Can create, edit, and manage projects they own
- **Developer**: Can view projects assigned to them
- **Viewer**: Can view only assigned projects (read-only)

## Usage Examples

### Create a New Project
1. Navigate to **Projects** in main navigation
2. Click **New Project** button
3. Fill in project details:
   - Name: "User Management System"
   - Code: "UMS"
   - Client: "ABC Corporation"
   - Dates: Select start and end dates
   - Colour: Choose a colour for visual identification
4. Click **Create Project**

### View Project Details
1. Navigate to **Projects**
2. Click **View** on any project card
3. See project information, team members, and backlog

### Edit Project
1. On project details page, click **Edit**
2. Modify project details
3. Click **Update Project**

### Archive Project
1. On projects list, click **Archive** button on the project card
2. Confirm archival
3. Project moves to "Archived" tab

### Add Team Member (Future)
1. On project details page, click **+ Add** in Team Members section
2. Select user and role
3. Click to assign

## Future Enhancements
- [ ] Project avatar image upload
- [ ] Team member bulk assignment
- [ ] Project templates
- [ ] Project cloning
- [ ] Project milestones
- [ ] Project budget tracking
- [ ] Project health indicators
- [ ] Automated backlog creation from templates
- [ ] Project activity timeline
- [ ] Export project data

## Files Created

### Domain
- `PMTool.Domain/Enums/ProjectStatus.cs` - Project status enum
- `PMTool.Domain/Entities/Project.cs` - Project entity
- `PMTool.Domain/Entities/ProjectBacklog.cs` - ProjectBacklog entity

### Infrastructure
- `PMTool.Infrastructure/Repositories/Interfaces/IProjectRepository.cs` - Repository interface
- `PMTool.Infrastructure/Repositories/ProjectRepository.cs` - Repository implementation
- `PMTool.Infrastructure/Migrations/20260330042450_AddProjects.cs` - Database migration

### Application
- `PMTool.Application/DTOs/Project/CreateProjectRequest.cs` - DTO
- `PMTool.Application/DTOs/Project/ProjectDTO.cs` - DTO
- `PMTool.Application/Validators/Project/CreateProjectRequestValidator.cs` - Validator
- `PMTool.Application/Services/Project/ProjectService.cs` - Service

### Web (Razor Pages)
- `PMTool.Web/Pages/Projects/Index.cshtml` - Projects list
- `PMTool.Web/Pages/Projects/Index.cshtml.cs` - Index page model
- `PMTool.Web/Pages/Projects/Create.cshtml` - Create project form
- `PMTool.Web/Pages/Projects/Create.cshtml.cs` - Create page model
- `PMTool.Web/Pages/Projects/Edit.cshtml` - Edit project form
- `PMTool.Web/Pages/Projects/Edit.cshtml.cs` - Edit page model
- `PMTool.Web/Pages/Projects/Details.cshtml` - Project details page
- `PMTool.Web/Pages/Projects/Details.cshtml.cs` - Details page model

## Testing Checklist

- [ ] Create a new project with all fields
- [ ] Create a project with minimal fields (only required)
- [ ] Verify project code uniqueness validation
- [ ] Verify date validation (end date after start date)
- [ ] View projects list with active/archived tabs
- [ ] View project details page
- [ ] Edit project information
- [ ] Archive a project
- [ ] View archived projects
- [ ] Delete a project
- [ ] Colour picker functionality
- [ ] Project code uniqueness check

## Database Migration

To apply the Project schema to your database:

```powershell
# In Package Manager Console
Update-Database
```

Or manually:
```sql
-- Run the migration script from 20260330042450_AddProjects.cs
```

## Security Notes

- Project creation restricted to authenticated users
- Permission checks should be enforced at service level (future)
- Archive operation prevents deletion of project data
- Project code is unique and immutable after creation

## Performance Notes

- Project list queries include filtering and pagination (add later)
- Project details page includes team members and backlog items
- Index on ProjectCode for faster lookups
- Foreign keys maintain referential integrity
