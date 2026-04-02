using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Auth;
using PMTool.Application.Services.Auth;
using PMTool.Application.Validators.Auth;
using System.Security.Claims;

namespace PMTool.Web.Pages.Auth;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly PMTool.Application.Services.Auth.IAuthenticationService _authService;

    [BindProperty]
    public LoginRequest Input { get; set; } = new();

    public string ErrorMessage { get; set; } = string.Empty;

    public LoginModel(PMTool.Application.Services.Auth.IAuthenticationService authService)
    {
        _authService = authService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validator = new LoginRequestValidator();
        var validationResult = await validator.ValidateAsync(Input);

        if (!validationResult.IsValid)
        {
            ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Page();
        }

        var result = await _authService.LoginAsync(Input);

        if (!result.Success)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        if (result.RequiresTwoFactor)
        {
            // Store user ID and temp token in session for 2FA verification
            HttpContext.Session.SetString("UserId", result.UserId ?? string.Empty);
            HttpContext.Session.SetString("TempToken", result.TempToken ?? string.Empty);
            HttpContext.Session.SetString("UserEmail", Input.Email);
            return RedirectToPage("./LoginTwoFactor");
        }

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

    public async Task<IActionResult> OnPostQuickLoginAsync(string email, string password)
    {
        Input = new LoginRequest { Email = email, Password = password };
        return await OnPostAsync();
    }
}
