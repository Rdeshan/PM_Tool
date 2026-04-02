# User Administration Integration Guide

## Quick Start - Registering Services

Add to `Program.cs`:

```csharp
// Register User Admin Services
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Register Repositories
builder.Services.AddScoped<IUserAdminRepository, UserAdminRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
```

## Database Migration

Create a new migration in Package Manager Console:

```powershell
Add-Migration AddUserAdministration
Update-Database
```

Or using dotnet CLI:

```bash
dotnet ef migrations add AddUserAdministration
dotnet ef database update
```

## Minimal Razor Pages Examples

### Admin Invite Page (`/Admin/Users/Invite.cshtml.cs`)

```csharp
[Authorize(Roles = "Administrator")]
public class InviteModel : PageModel
{
    private readonly IUserAdminService _userAdminService;
    private readonly IRoleService _roleService;
    private readonly ITeamService _teamService;

    [BindProperty]
    public InviteUserRequest Input { get; set; } = new();

    public List<RoleDTO> AvailableRoles { get; set; } = new();
    public List<TeamDTO> AvailableTeams { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public InviteModel(IUserAdminService userAdminService, IRoleService roleService, ITeamService teamService)
    {
        _userAdminService = userAdminService;
        _roleService = roleService;
        _teamService = teamService;
    }

    public async Task OnGetAsync()
    {
        AvailableRoles = (await _roleService.GetAllRolesAsync()).ToList();
        AvailableTeams = (await _teamService.GetAllTeamsAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _userAdminService.InviteUserAsync(Input, userId);

        if (!result)
        {
            ErrorMessage = "Failed to invite user. Email may already exist.";
            await OnGetAsync();
            return Page();
        }

        SuccessMessage = $"Invitation sent to {Input.Email}";
        Input = new();
        await OnGetAsync();
        return Page();
    }
}
```

### Admin Users List Page (`/Admin/Users/Index.cshtml.cs`)

```csharp
[Authorize(Roles = "Administrator")]
public class IndexModel : PageModel
{
    private readonly IUserAdminService _userAdminService;

    public IEnumerable<UserDTO> Users { get; set; } = new List<UserDTO>();
    public bool ShowInactive { get; set; }

    public IndexModel(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    public async Task OnGetAsync(bool inactive = false)
    {
        ShowInactive = inactive;
        Users = inactive 
            ? await _userAdminService.GetInactiveUsersAsync()
            : await _userAdminService.GetActiveUsersAsync();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(Guid userId)
    {
        await _userAdminService.DeactivateUserAsync(userId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReactivateAsync(Guid userId)
    {
        await _userAdminService.ReactivateUserAsync(userId);
        return RedirectToPage();
    }
}
```

### User Profile Settings Page (`/Account/Profile.cshtml.cs`)

```csharp
[Authorize]
public class ProfileModel : PageModel
{
    private readonly IUserAdminService _userAdminService;

    [BindProperty]
    public UpdateProfileRequest Input { get; set; } = new();

    public UserDTO? CurrentUser { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public ProfileModel(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    public async Task OnGetAsync()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        CurrentUser = await _userAdminService.GetUserByIdAsync(userId);

        if (CurrentUser != null)
        {
            Input = new UpdateProfileRequest
            {
                FirstName = CurrentUser.FirstName,
                LastName = CurrentUser.LastName,
                DisplayName = CurrentUser.DisplayName,
                AvatarUrl = CurrentUser.AvatarUrl,
                NotificationsEnabled = CurrentUser.NotificationsEnabled
            };
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _userAdminService.UpdateProfileAsync(userId, Input);

        if (!result)
        {
            ErrorMessage = "Failed to update profile";
            await OnGetAsync();
            return Page();
        }

        SuccessMessage = "Profile updated successfully";
        await OnGetAsync();
        return Page();
    }
}
```

### Team Management Page (`/Admin/Teams/Index.cshtml.cs`)

```csharp
[Authorize(Roles = "Administrator,Project Manager")]
public class IndexModel : PageModel
{
    private readonly ITeamService _teamService;
    private readonly IUserAdminService _userAdminService;

    public IEnumerable<TeamDTO> Teams { get; set; } = new List<TeamDTO>();
    public Guid? SelectedTeamId { get; set; }
    public IEnumerable<UserDTO> TeamMembers { get; set; } = new List<UserDTO>();

    [BindProperty]
    public CreateTeamRequest Input { get; set; } = new();

    public IndexModel(ITeamService teamService, IUserAdminService userAdminService)
    {
        _teamService = teamService;
        _userAdminService = userAdminService;
    }

    public async Task OnGetAsync(Guid? teamId = null)
    {
        Teams = await _teamService.GetAllTeamsAsync();
        
        if (teamId.HasValue && Guid.Empty != teamId.Value)
        {
            SelectedTeamId = teamId;
            TeamMembers = await _userAdminService.GetUsersByTeamAsync(teamId.Value);
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var result = await _teamService.CreateTeamAsync(Input);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(Guid userId, Guid teamId)
    {
        await _userAdminService.RemoveUserFromTeamAsync(userId, teamId);
        return RedirectToPage(new { teamId });
    }
}
```

## Typical User Flow

### 1. Admin Invites User
- Admin navigates to `/Admin/Users/Invite`
- Fills in email, name, selects roles and teams
- System sends invitation email
- User receives email with setup link (valid 7 days)

### 2. User Completes Setup
- User clicks setup link in email
- Completes account setup page (sets password, confirms details)
- Account is activated
- User can now login

### 3. Admin Can Deactivate User
- Admin navigates to `/Admin/Users`
- Clicks deactivate on user
- User marked inactive, cannot login
- User's data preserved
- Can reactivate anytime

### 4. User Can Update Profile
- User navigates to `/Account/Profile`
- Updates display name, avatar, etc.
- Changes notification preferences
- Saves changes

### 5. Admin/PM Manages Teams
- Admin navigates to `/Admin/Teams`
- Creates/edits teams
- Assigns users to teams
- Teams used for collective project assignment

---

## Testing Checklist

- [ ] Send invitation email to valid email
- [ ] Verify 7-day token expiration
- [ ] Complete account setup with invitation token
- [ ] Deactivate user and verify cannot login
- [ ] Reactivate deactivated user
- [ ] Update user profile fields
- [ ] Create team
- [ ] Add user to team
- [ ] Remove user from team
- [ ] Verify role assignments are persisted
- [ ] Verify notification preferences saved
- [ ] Test with avatar upload

---

## Common Issues & Solutions

### Issue: UserAdminRepository injection fails
**Solution:** Make sure to register repositories in Program.cs:
```csharp
builder.Services.AddScoped<IUserAdminRepository, UserAdminRepository>();
```

### Issue: Team namespace conflict
**Solution:** Already resolved using `TeamEntity` alias in TeamService

### Issue: Invitation email not sent
**Solution:** Verify email settings in appsettings.json:
```json
"Email": {
  "IsEnabled": true,
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "your-app-password",
  "SenderName": "PMTool",
  "UseSSL": true
}
```

---

## Performance Considerations

1. **User Retrieval:** Includes related roles and teams via Include
2. **Team Members:** Eager-loaded to avoid N+1 queries
3. **Indexes:** Unique constraints on Email, Team names, TeamId+UserId
4. **Batch Operations:** Team member assignment in loops (consider batch insert for high volume)

---

## Security Considerations

✅ Invitation tokens hashed and time-limited (7 days)
✅ Only admins can invite/deactivate users
✅ Account setup must verify token
✅ Deactivated users cannot login even with correct password
✅ All operations validated and authorized
✅ User data preserved on deactivation (no hard delete)

---

## Future Enhancements

- [ ] Bulk user import from CSV
- [ ] User activity audit log
- [ ] Profile pictures upload to cloud storage
- [ ] Team hierarchy/nesting
- [ ] User ban/suspension with reasons
- [ ] Admin dashboard showing user statistics
- [ ] Email notification templates customization
- [ ] User invitation resend functionality
