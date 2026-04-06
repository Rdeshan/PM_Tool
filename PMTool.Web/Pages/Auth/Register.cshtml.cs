using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Auth;
using PMTool.Application.Interfaces;
using PMTool.Application.Validators.Auth;

namespace PMTool.Web.Pages.Auth;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly IAuthenticationService _authService;

    [BindProperty]
    public RegisterRequest Input { get; set; } = new();

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public RegisterModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validator = new RegisterRequestValidator();
        var validationResult = await validator.ValidateAsync(Input);

        if (!validationResult.IsValid)
        {
            ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Page();
        }

        var result = await _authService.RegisterAsync(Input);

        if (!result)
        {
            ErrorMessage = "An account with this email already exists.";
            return Page();
        }

        SuccessMessage = "Account created successfully! Please check your email to confirm your address before logging in.";
        Input = new RegisterRequest(); // Clear form
        return Page();
    }
}
