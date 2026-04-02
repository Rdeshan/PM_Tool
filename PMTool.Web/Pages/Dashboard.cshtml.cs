using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PMTool.Web.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    public IActionResult OnGet()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Auth/Login");
        }

        // Route users to their role-specific dashboards
        if (User.IsInRole("Administrator"))
        {
            return RedirectToPage("/Admin/Dashboard");
        }

        if (User.IsInRole("Project Manager"))
        {
            return RedirectToPage("/PM/Dashboard");
        }

        // Regular users stay on this dashboard
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToPage("/Auth/Login");
    }
}
