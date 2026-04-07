using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Interfaces;

namespace PMTool.Web.Pages.Auth;

[AllowAnonymous]
public class ForgotPasswordModel : PageModel
{
    private readonly IAuthenticationService _authService;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public ForgotPasswordModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Please enter a valid email address";
            return Page();
        }

        var result = await _authService.RequestPasswordResetAsync(Email);

        if (!result)
        {
            // Don't reveal if email exists or not (security best practice)
            SuccessMessage = "If an account exists with this email, you will receive a password reset link shortly.";
            Email = string.Empty;
            return Page();
        }

        SuccessMessage = "Password reset link has been sent to your email. Please check your inbox (and spam folder).";
        Email = string.Empty;
        return Page();
    }
}
