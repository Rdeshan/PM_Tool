using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.User;
using PMTool.Application.Services.User;
using System.Security.Claims;

namespace PMTool.Web.Pages.Account;

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
        SuccessMessage = TempData["SuccessMessage"]?.ToString();
        ErrorMessage = TempData["ErrorMessage"]?.ToString();

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
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fill in all required fields.";
            await OnGetAsync();
            return Page();
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _userAdminService.UpdateProfileAsync(userId, Input);

        if (!result)
        {
            ErrorMessage = "Failed to update profile. Please try again.";
            await OnGetAsync();
            return Page();
        }

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToPage();
    }
}
