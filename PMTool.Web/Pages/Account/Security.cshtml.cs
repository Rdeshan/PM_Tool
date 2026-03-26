using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PMTool.Web.Pages.Account;

[Authorize]
public class ChangePasswordInput
{
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class SecurityModel : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    [BindProperty]
    public ChangePasswordInput Input { get; set; } = new();

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public SecurityModel(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public IActionResult OnGet()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Auth/Login");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
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

        if (string.IsNullOrWhiteSpace(Input.CurrentPassword))
        {
            ErrorMessage = "Current password is required";
            return Page();
        }

        if (Input.NewPassword.Length < 8)
        {
            ErrorMessage = "New password must be at least 8 characters";
            return Page();
        }

        if (Input.NewPassword != Input.ConfirmPassword)
        {
            ErrorMessage = "New passwords do not match";
            return Page();
        }

        var user = await _userRepository.GetByEmailAsync(userEmail);
        if (user == null)
        {
            return RedirectToPage("/Auth/Login");
        }

        if (!_tokenService.VerifyPassword(Input.CurrentPassword, user.PasswordHash))
        {
            ErrorMessage = "Current password is incorrect";
            return Page();
        }

        user.PasswordHash = _tokenService.HashPassword(Input.NewPassword);
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;

        var result = await _userRepository.UpdateAsync(user);

        if (!result)
        {
            ErrorMessage = "Failed to update password";
            return Page();
        }

        SuccessMessage = "Password has been changed successfully";
        Input = new ChangePasswordInput();
        return Page();
    }
}
