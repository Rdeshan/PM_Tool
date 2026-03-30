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

        if (!result.Success)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        // Clear session data
        HttpContext.Session.Remove("UserEmail");
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Remove("TempToken");

        // Create authentication cookie with user roles
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

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync("Cookies", claimsPrincipal);

        return RedirectToPage("/Dashboard");
    }
}
