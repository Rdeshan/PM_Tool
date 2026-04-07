using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.User;
using PMTool.Application.Interfaces;

namespace PMTool.Web.Pages.Admin.Users;

[Authorize(Roles = "Administrator")]
public class IndexModel : PageModel
{
    private readonly IUserAdminService _userAdminService;

    public IEnumerable<UserDTO> Users { get; set; } = new List<UserDTO>();
    public bool ShowInactive { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public IndexModel(IUserAdminService userAdminService)
    {
        _userAdminService = userAdminService;
    }

    public async Task OnGetAsync(bool inactive = false)
    {
        ShowInactive = inactive;
        SuccessMessage = TempData["SuccessMessage"]?.ToString();
        ErrorMessage = TempData["ErrorMessage"]?.ToString();

        Users = inactive
            ? await _userAdminService.GetInactiveUsersAsync()
            : await _userAdminService.GetActiveUsersAsync();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(Guid userId)
    {
        var result = await _userAdminService.DeactivateUserAsync(userId);
        if (result)
        {
            TempData["SuccessMessage"] = "User deactivated successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to deactivate user.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReactivateAsync(Guid userId)
    {
        var result = await _userAdminService.ReactivateUserAsync(userId);
        if (result)
        {
            TempData["SuccessMessage"] = "User reactivated successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to reactivate user.";
        }

        return RedirectToPage();
    }
}
