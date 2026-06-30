using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Infrastructure.Repositories.Interfaces;
using System.Security.Claims;

namespace PMTool.Web.Pages.Account;

[Authorize]
public class EmailConfigModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public bool EmailOnTaskAssigned { get; set; } = true;
    public bool EmailOnSprintEvent { get; set; } = true;
    public bool EmailOnProjectAssigned { get; set; } = true;

    public string StatusMessage { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public EmailConfigModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return RedirectToPage("/Auth/Login");

        EmailOnTaskAssigned    = user.EmailOnTaskAssigned;
        EmailOnSprintEvent     = user.EmailOnSprintEvent;
        EmailOnProjectAssigned = user.EmailOnProjectAssigned;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool EmailOnTaskAssigned, bool EmailOnSprintEvent, bool EmailOnProjectAssigned)
    {
        var user = await GetCurrentUserAsync();
        if (user == null) return RedirectToPage("/Auth/Login");

        user.EmailOnTaskAssigned    = EmailOnTaskAssigned;
        user.EmailOnSprintEvent     = EmailOnSprintEvent;
        user.EmailOnProjectAssigned = EmailOnProjectAssigned;

        var saved = await _userRepository.UpdateAsync(user);

        // Reflect values back to UI
        this.EmailOnTaskAssigned    = user.EmailOnTaskAssigned;
        this.EmailOnSprintEvent     = user.EmailOnSprintEvent;
        this.EmailOnProjectAssigned = user.EmailOnProjectAssigned;

        if (saved)
        {
            IsSuccess     = true;
            StatusMessage = "Email notification preferences saved successfully.";
        }
        else
        {
            IsSuccess     = false;
            StatusMessage = "Failed to save preferences. Please try again.";
        }

        return Page();
    }

    private async Task<PMTool.Domain.Entities.User?> GetCurrentUserAsync()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return null;
        return await _userRepository.GetByEmailAsync(email);
    }
}
