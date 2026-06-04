using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.Dashboard;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Web.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly IDashboardService _dashboardService;
    private readonly AppDbContext _context;

    public IEnumerable<ProductBacklog> BacklogItems { get; set; } = new List<ProductBacklog>();

    public PersonalDashboardDto Dashboard { get; set; } = new();

    public DashboardModel(IDashboardService dashboardService, AppDbContext context)
    {
        _dashboardService = dashboardService;
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToPage("/Auth/Login");

        if (User.IsInRole("Administrator"))
            return RedirectToPage("/Admin/Dashboard");

        if (User.IsInRole("Project Manager"))
            return RedirectToPage("/PM/Dashboard");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        Dashboard = await _dashboardService.GetPersonalDashboardAsync(userId);
        BacklogItems = await _context.ProductBacklogs.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToPage("/Auth/Login");
    }
}
