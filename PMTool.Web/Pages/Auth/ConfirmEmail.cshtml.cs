using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Services.Auth;

namespace PMTool.Web.Pages.Auth;

[AllowAnonymous]
public class ConfirmEmailModel : PageModel
{
    private readonly IAuthenticationService _authService;

    public bool IsConfirmed { get; set; }
    public bool IsExpired { get; set; }
    public bool IsInvalid { get; set; }
    public string? Email { get; set; }

    public ConfirmEmailModel(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync(string? token, string? email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            IsInvalid = true;
            return Page();
        }

        Email = email;

        var result = await _authService.ConfirmEmailAsync(email, token);

        if (result)
        {
            IsConfirmed = true;
        }
        else
        {
            IsExpired = true;
        }

        return Page();
    }
}
