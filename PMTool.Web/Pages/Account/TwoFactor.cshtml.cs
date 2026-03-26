using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Services.Auth;
using PMTool.Infrastructure.Repositories.Interfaces;
using System.Security.Claims;

namespace PMTool.Web.Pages.Account;

[Authorize]
public class TwoFactorModel : PageModel
{
    private readonly IAuthenticationService _authService;
    private readonly IUserRepository _userRepository;

    public bool IsTwoFactorEnabled { get; set; }
    public string SuccessMessage { get; set; } = string.Empty;

    public TwoFactorModel(IAuthenticationService authService, IUserRepository userRepository)
    {
        _authService = authService;
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

        IsTwoFactorEnabled = user.TwoFactorEnabled;
        return Page();
    }

    public async Task<IActionResult> OnPostEnableAsync()
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

        await _authService.EnableTwoFactorAsync(userEmail);
        SuccessMessage = "Two-Factor Authentication has been enabled. You'll need to enter a code on your next login.";
        IsTwoFactorEnabled = true;
        return Page();
    }

    public async Task<IActionResult> OnPostDisableAsync()
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

        await _authService.DisableTwoFactorAsync(userEmail);
        SuccessMessage = "Two-Factor Authentication has been disabled.";
        IsTwoFactorEnabled = false;
        return Page();
    }
}
