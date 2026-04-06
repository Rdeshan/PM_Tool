using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PMTool.Web.Pages.Auth;

[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly IAuthenticationService _authService;

    [BindProperty]
    public string Token { get; set; } = string.Empty;

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool IsSuccess { get; set; }
    public bool IsInvalid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public ResetPasswordModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public IActionResult OnGet(string? token, string? email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            IsInvalid = true;
            return Page();
        }

        Token = token;
        Email = email;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Email))
        {
            IsInvalid = true;
            return Page();
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match";
            return Page();
        }

        if (Password.Length < 8)
        {
            ErrorMessage = "Password must be at least 8 characters";
            return Page();
        }

        var result = await _authService.ResetPasswordAsync(Email, Token, Password);

        if (!result)
        {
            IsInvalid = true;
            return Page();
        }

        IsSuccess = true;
        return Page();
    }
}
