# Role-Based Dashboard Routing Fix

## Problem
When users logged in (especially with quick login), all users were shown the same common dashboard regardless of their role. Admins and Project Managers should be redirected to their specific dashboards, while regular users see the common dashboard.

## Solution

### Changes Made

#### 1. **Dashboard.cshtml.cs** - Added Role-Based Routing
Updated the `OnGet()` method to check user roles and redirect accordingly:

```csharp
public IActionResult OnGet()
{
    if (!User.Identity?.IsAuthenticated ?? true)
    {
        return RedirectToPage("/Auth/Login");
    }

    // Route users to their role-specific dashboards
    if (User.IsInRole("Administrator"))
    {
        return RedirectToPage("/Admin/Dashboard");
    }

    if (User.IsInRole("Project Manager"))
    {
        return RedirectToPage("/PM/Dashboard");
    }

    // Regular users stay on this dashboard
    return Page();
}
```

#### 2. **Dashboard.cshtml** - Simplified View
Removed role-check logic from the view since users are now automatically redirected based on their role.

#### 3. **LoginResponse.cs** - Added Roles Property
Added a `Roles` list to the LoginResponse DTO to carry role information from authentication service:

```csharp
public List<string> Roles { get; set; } = new();
```

#### 4. **IUserRepository.cs Updates**
Modified `GetByEmailAsync()` and `GetByIdAsync()` to include user roles:

```csharp
public async Task<User?> GetByEmailAsync(string email)
{
    return await _context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Email == email);
}
```

#### 5. **AuthenticationService.cs** - Populate Roles
Updated `LoginAsync()` to extract user roles from the database and include them in the response:

```csharp
var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "User").ToList() ?? new List<string> { "User" };

return new LoginResponse
{
    Success = true,
    Message = "Login successful",
    UserId = user.Id.ToString(),
    Roles = roles
};
```

#### 6. **Login.cshtml.cs** - Include Roles in Claims
Updated the cookie creation to include all user roles as claims instead of hardcoding "User":

```csharp
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, result.UserId ?? string.Empty),
    new(ClaimTypes.Email, Input.Email)
};

// Add all roles to claims
foreach (var role in result.Roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}
```

#### 7. **TwoFactorVerifyResponse.cs** - New DTO
Created a new response DTO for 2FA verification that includes roles:

```csharp
public class TwoFactorVerifyResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public List<string> Roles { get; set; } = new();
}
```

#### 8. **IAuthenticationService.cs** - Updated Interface
Changed the return type of `VerifyTwoFactorCodeAsync()` from `bool` to `TwoFactorVerifyResponse`:

```csharp
Task<TwoFactorVerifyResponse> VerifyTwoFactorCodeAsync(string email, string code);
```

#### 9. **AuthenticationService.cs** - 2FA Role Handling
Updated `VerifyTwoFactorCodeAsync()` to return roles similar to `LoginAsync()`:

```csharp
var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "User").ToList() ?? new List<string> { "User" };

return new TwoFactorVerifyResponse
{
    Success = true,
    Message = "Two-factor verification successful",
    UserId = user.Id.ToString(),
    Roles = roles
};
```

#### 10. **LoginTwoFactor.cshtml.cs** - Include Roles in 2FA Claims
Updated to use the new response and add roles to claims:

```csharp
var result = await _authService.VerifyTwoFactorCodeAsync(Input.Email, Input.Code);

if (!result.Success)
{
    ErrorMessage = result.Message;
    return Page();
}

// Add all roles to claims
foreach (var role in result.Roles)
{
    claims.Add(new Claim(ClaimTypes.Role, role));
}
```

## Flow After Fix

### For Admins:
1. Login → Dashboard redirects to Admin Dashboard
2. Admin sees project creation, editing, and deletion buttons

### For Project Managers:
1. Login → Dashboard redirects to PM Dashboard
2. PM sees project creation, editing, and deletion buttons

### For Regular Users:
1. Login → Dashboard displays common user dashboard
2. User can view projects but no creation, editing, or deletion options

## Key Benefits

✅ Roles are now properly fetched from the database during authentication
✅ All user roles are added to the authentication claims
✅ Users are automatically routed to the correct dashboard on login
✅ Works for both quick login and 2FA scenarios
✅ Consistent role-based access control across the application
