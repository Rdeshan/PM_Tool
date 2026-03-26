using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Auth;
using PMTool.Application.Services.Auth;
using System.Security.Claims;

namespace PMTool.Web.Pages.Auth;

[AllowAnonymous]
public class LoginTwoFactorModel : PageModel
{
    private readonly PMTool.Application.Services.Auth.IAuthenticationService _authService;

    [BindProperty]
    public TwoFactorRequest Input { get; set; } = new();

    public string ErrorMessage { get; set; } = string.Empty;

    public LoginTwoFactorModel(PMTool.Application.Services.Auth.IAuthenticationService authService)
    {
        _authService = authService;
    }

    public IActionResult OnGet()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage("./Login");
        }

        Input.Email = userEmail;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToPage("./Login");
        }

        Input.Email = userEmail;

        if (string.IsNullOrWhiteSpace(Input.Code) || Input.Code.Length != 6)
        {
            ErrorMessage = "Please enter a valid 6-digit code";
            return Page();
        }

        var result = await _authService.VerifyTwoFactorCodeAsync(Input.Email, Input.Code);

        if (!result)
        {
            ErrorMessage = "Invalid or expired verification code";
            return Page();
        }

        // Clear session data
        HttpContext.Session.Remove("UserEmail");
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Remove("TempToken");

        // Create authentication cookie
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Input.Email),
            new(ClaimTypes.Email, Input.Email),
            new(ClaimTypes.Role, "User")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync("Cookies", claimsPrincipal);

        return RedirectToPage("/Dashboard");
    }
}
