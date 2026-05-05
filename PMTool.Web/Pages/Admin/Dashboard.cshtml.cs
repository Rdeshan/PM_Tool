using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs;
using PMTool.Application.DTOs.Dashboard;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Interfaces;
using PMTool.Application.Services.Project;

namespace PMTool.Web.Pages.Admin;

[Authorize(Roles = "Administrator")]
public class DashboardModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IDashboardService _dashboardService;

    public List<ProjectDTO> RecentProjects { get; set; } = new();
    public DashboardDto Dashboard { get; set; } = new();

    public DashboardModel(IProjectService projectService, IDashboardService dashboardService)
    {
        _projectService = projectService;
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToPage("/Auth/Login");

        if (!User.IsInRole("Administrator"))
            return RedirectToPage("/Dashboard");

        // Load dashboard stats
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            Dashboard = await _dashboardService.GetDashboardDataAsync(userId);
        }
        catch
        {
            Dashboard = new DashboardDto();
        }

        // Load recent projects (last 6)
        try
        {
            var allProjects = await _projectService.GetAllProjectsAsync();
            RecentProjects = allProjects
                .OrderByDescending(p => p.UpdatedAt)
                .Take(6)
                .ToList();
        }
        catch
        {
            RecentProjects = new List<ProjectDTO>();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToPage("/Auth/Login");
    }

    public async Task<IActionResult> OnPostDeleteProjectAsync(Guid projectId)
    {
        if (!User.IsInRole("Administrator"))
            return Unauthorized();

        try
        {
            await _projectService.DeleteProjectAsync(projectId);
            TempData["SuccessMessage"] = "Project deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to delete project: {ex.Message}";
        }

        return RedirectToPage();
    }
}
