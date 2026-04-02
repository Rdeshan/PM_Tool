# User Administration Feature Implementation

## Overview
Complete user administration system with the following capabilities:
1. **Invite by Email** - Administrators invite users via email with account setup links
2. **Deactivate Accounts** - Administrators can deactivate users while preserving historical data
3. **Role Reassignment** - Change user roles on projects at any time
4. **Profile Settings** - Users can update display name, avatar, and notification preferences
5. **Team Grouping** - Users organized into named teams (Dev, QA, BA, DevOps, Design) for sub-project assignment

---

## Architecture Overview

### Domain Entities

#### Extended User Entity
```csharp
public class User
{
    // New Fields
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public string? InvitationToken { get; set; }
    public DateTime? InvitationTokenExpiry { get; set; }
    public bool AccountSetupCompleted { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    
    // Navigation
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
```

#### Team Entity
```csharp
public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
```

#### TeamMember Entity
```csharp
public class TeamMember
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    
    public Team? Team { get; set; }
    public User? User { get; set; }
}
```

---

## Data Transfer Objects (DTOs)

### UserDTO
Comprehensive user representation with roles and teams:
```csharp
public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public bool NotificationsEnabled { get; set; }
    public List<string> Roles { get; set; }
    public List<string> Teams { get; set; }
}
```

### InviteUserRequest
```csharp
public class InviteUserRequest
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Guid> RoleIds { get; set; }
    public List<Guid> TeamIds { get; set; }
}
```

### UpdateProfileRequest
```csharp
public class UpdateProfileRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool NotificationsEnabled { get; set; }
}
```

### TeamDTO & CreateTeamRequest
```csharp
public class TeamDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public int MemberCount { get; set; }
}

public class CreateTeamRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```

---

## Services

### IUserAdminService
Handles all user administration operations:

```csharp
public interface IUserAdminService
{
    // Retrieval
    Task<UserDTO?> GetUserByIdAsync(Guid id);
    Task<UserDTO?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();
    Task<IEnumerable<UserDTO>> GetActiveUsersAsync();
    Task<IEnumerable<UserDTO>> GetInactiveUsersAsync();
    
    // Invitation & Setup
    Task<bool> InviteUserAsync(InviteUserRequest request, Guid invitedByUserId);
    Task<bool> CompleteAccountSetupAsync(string token, string password, string firstName, string lastName);
    
    // Activation/Deactivation
    Task<bool> DeactivateUserAsync(Guid userId);
    Task<bool> ReactivateUserAsync(Guid userId);
    
    // Profile Management
    Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    
    // Role Management
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    
    // Team Management
    Task<bool> AddUserToTeamAsync(Guid userId, Guid teamId);
    Task<bool> RemoveUserFromTeamAsync(Guid userId, Guid teamId);
    Task<IEnumerable<UserDTO>> GetUsersByTeamAsync(Guid teamId);
}
```

### ITeamService
Team management operations:

```csharp
public interface ITeamService
{
    Task<TeamDTO?> GetTeamByIdAsync(Guid id);
    Task<TeamDTO?> GetTeamByNameAsync(string name);
    Task<IEnumerable<TeamDTO>> GetAllTeamsAsync();
    Task<IEnumerable<TeamDTO>> GetActiveTeamsAsync();
    Task<bool> CreateTeamAsync(CreateTeamRequest request);
    Task<bool> UpdateTeamAsync(Guid id, CreateTeamRequest request);
    Task<bool> DeleteTeamAsync(Guid id);
    Task<bool> AddMemberAsync(Guid teamId, Guid userId);
    Task<bool> RemoveMemberAsync(Guid teamId, Guid userId);
}
```

---

## Repositories

### IUserAdminRepository
Extended user data access:

```csharp
public interface IUserAdminRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetActiveAsync();
    Task<IEnumerable<User>> GetInactiveAsync();
    Task<bool> CreateAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeactivateAsync(Guid userId);
    Task<bool> ReactivateAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersByTeamAsync(Guid teamId);
    Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId);
}
```

### ITeamRepository
Team data access:

```csharp
public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<Team?> GetByNameAsync(string name);
    Task<IEnumerable<Team>> GetAllAsync();
    Task<IEnumerable<Team>> GetActiveAsync();
    Task<bool> CreateAsync(Team team);
    Task<bool> UpdateAsync(Team team);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<User>> GetTeamMembersAsync(Guid teamId);
    Task<bool> AddMemberAsync(Guid teamId, Guid userId);
    Task<bool> RemoveMemberAsync(Guid teamId, Guid userId);
    Task<bool> IsMemberAsync(Guid teamId, Guid userId);
}
```

---

## Email Service Extension

Added support for account invitations:

```csharp
public interface IEmailService
{
    Task<bool> SendAccountInvitationAsync(string email, string setupLink);
    // ... existing methods
}
```

### Invitation Email
- Sent to invited users with 7-day valid setup link
- Includes welcome message and account setup instructions
- Pre-populates email with invitation token

---

## Features & Workflows

### 1. Invite User by Email

**Workflow:**
```
Admin initiates invite
    ↓
System creates inactive user with invitation token
    ↓
Roles and teams assigned
    ↓
Invitation email sent with setup link
    ↓
User clicks link within 7 days
    ↓
User completes account setup (set password, confirm details)
    ↓
Account activated and user can login
```

**Service Call:**
```csharp
await _userAdminService.InviteUserAsync(
    new InviteUserRequest 
    { 
        Email = "user@example.com",
        FirstName = "John",
        LastName = "Doe",
        RoleIds = new List<Guid> { adminRoleId },
        TeamIds = new List<Guid> { devTeamId }
    }, 
    adminUserId
);
```

### 2. Deactivate User

**Features:**
- Marks user as inactive (`IsActive = false`)
- Records deactivation timestamp
- All historical data preserved
- User cannot login but data remains accessible
- Can be reactivated anytime

**Service Call:**
```csharp
await _userAdminService.DeactivateUserAsync(userId);
```

### 3. Role Reassignment

**Features:**
- Assign/remove roles to users
- Can be done at any time
- Multiple roles supported
- Project-specific roles supported

**Service Calls:**
```csharp
// Assign role
await _userAdminService.AssignRoleToUserAsync(userId, roleId);

// Remove role
await _userAdminService.RemoveRoleFromUserAsync(userId, roleId);
```

### 4. Profile Settings

**Editable Fields:**
- Display Name (optional, used in UI instead of First/Last Name)
- Avatar URL
- First Name
- Last Name
- Notification Preferences (Email notifications on/off)

**Service Call:**
```csharp
await _userAdminService.UpdateProfileAsync(userId, 
    new UpdateProfileRequest 
    { 
        FirstName = "John",
        LastName = "Doe",
        DisplayName = "Johnny",
        AvatarUrl = "https://...",
        NotificationsEnabled = true
    }
);
```

### 5. Team Grouping

**Team Types (Examples):**
- Development
- Quality Assurance
- Business Analysis
- DevOps
- Design

**Features:**
- Users can belong to multiple teams
- Teams used for sub-project assignment
- Filter ticket views by team membership
- Collective assignment of teams to projects

**Service Calls:**
```csharp
// Create team
await _teamService.CreateTeamAsync(
    new CreateTeamRequest 
    { 
        Name = "Development",
        Description = "Development Team"
    }
);

// Add user to team
await _userAdminService.AddUserToTeamAsync(userId, teamId);

// Get team members
var members = await _userAdminService.GetUsersByTeamAsync(teamId);
```

---

## Database Schema

### Teams Table
```sql
CREATE TABLE Teams (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

### TeamMembers Table
```sql
CREATE TABLE TeamMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TeamId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    JoinedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_TeamMembers_Teams FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TeamMembers_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    UNIQUE (TeamId, UserId)
);
```

### Extended Users Columns
```sql
ALTER TABLE Users ADD
    DisplayName NVARCHAR(100),
    AvatarUrl NVARCHAR(500),
    NotificationsEnabled BIT NOT NULL DEFAULT 1,
    InvitationToken NVARCHAR(MAX),
    InvitationTokenExpiry DATETIME2,
    AccountSetupCompleted BIT NOT NULL DEFAULT 0,
    DeactivatedAt DATETIME2;
```

---

## Access Control

### Who Can Do What

| Operation | Admin | PM | Regular User |
|-----------|-------|----|----|
| Invite users | ✅ | ✅ | ❌ |
| Deactivate users | ✅ | ❌ | ❌ |
| Reactivate users | ✅ | ❌ | ❌ |
| Assign roles | ✅ | ❌ | ❌ |
| Create teams | ✅ | ✅ | ❌ |
| Manage team members | ✅ | ✅ | ❌ |
| Update own profile | ✅ | ✅ | ✅ |
| View user list | ✅ | ✅ | ❌ |

---

## Validation

### InviteUserRequestValidator
- Email must be valid
- First/Last names required (max 100 chars each)
- At least one role must be assigned

### CreateTeamRequestValidator
- Team name required (max 100 chars)
- Description optional (max 500 chars)
- Unique team names

---

## Implementation Checklist

✅ Domain Entities (User, Team, TeamMember) extended
✅ DTOs created (UserDTO, InviteUserRequest, UpdateProfileRequest, TeamDTO)
✅ Repositories implemented (IUserAdminRepository, ITeamRepository)
✅ Services implemented (UserAdminService, TeamService)
✅ Validators created
✅ Email service updated with invitation support
✅ Database context updated with Team/TeamMember configurations
✅ Build successful

---

## Next Steps

1. **Create Razor Pages for User Administration:**
   - `/Admin/Users/Index.cshtml` - User list with management options
   - `/Admin/Users/Invite.cshtml` - Invite new users form
   - `/Admin/Teams/Index.cshtml` - Team management
   - `/Account/Profile.cshtml` - User profile settings

2. **Create API Endpoints (if needed):**
   - For programmatic user/team management
   - For third-party integrations

3. **Create Database Migration:**
   - Team and TeamMember tables
   - Extended User columns

4. **Update Program.cs:**
   - Register IUserAdminService and ITeamService
   - Register repositories

5. **Implement Authorization Attributes:**
   - Ensure only admins can access admin pages
   - Add role-based authorization

---

## Code Files Created

- `PMTool.Domain\Entities\Team.cs`
- `PMTool.Domain\Entities\TeamMember.cs`
- `PMTool.Application\DTOs\User\UserDTO.cs`
- `PMTool.Application\DTOs\User\InviteUserRequest.cs`
- `PMTool.Application\DTOs\User\UpdateProfileRequest.cs`
- `PMTool.Application\DTOs\Team\TeamDTO.cs`
- `PMTool.Application\DTOs\Team\CreateTeamRequest.cs`
- `PMTool.Infrastructure\Repositories\Interfaces\ITeamRepository.cs`
- `PMTool.Infrastructure\Repositories\Interfaces\IUserAdminRepository.cs`
- `PMTool.Infrastructure\Repositories\TeamRepository.cs`
- `PMTool.Infrastructure\Repositories\UserAdminRepository.cs`
- `PMTool.Application\Services\User\IUserAdminService.cs`
- `PMTool.Application\Services\Team\ITeamService.cs`
- `PMTool.Application\Services\Admin\UserAdminService.cs`
- `PMTool.Application\Services\Admin\TeamService.cs`
- `PMTool.Application\Validators\User\InviteUserRequestValidator.cs`
- `PMTool.Application\Validators\Team\CreateTeamRequestValidator.cs`
