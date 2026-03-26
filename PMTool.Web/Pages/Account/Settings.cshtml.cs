using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Infrastructure.Repositories.Interfaces;
using System.Security.Claims;

namespace PMTool.Web.Pages.Account;

[Authorize]
public class SettingsModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string SuccessMessage { get; set; } = string.Empty;

    public SettingsModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Auth/Login");
        }

        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage("/Auth/Login");
        }

        var user = await _userRepository.GetByEmailAsync(userEmail);
        if (user == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        FirstName = user.FirstName;
        LastName = user.LastName;
        CreatedAt = user.CreatedAt;

        return Page();
    }
}
