using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Services.Project;
using PMTool.Application.DTOs.Project;

namespace PMTool.Web.Pages.PM;

[Authorize(Roles = "Project Manager")]
public class DashboardModel : PageModel
{
    private readonly IProjectService _projectService;

    public List<ProjectDTO> RecentProjects { get; set; } = new();
    public int ActiveProjectCount { get; set; }
    public int TeamMemberCount { get; set; }

    public DashboardModel(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Auth/Login");
        }

        if (!User.IsInRole("Project Manager"))
        {
            return RedirectToPage("/Dashboard");
        }

        // Get recent projects (last 6)
        try
        {
            var allProjects = await _projectService.GetAllProjectsAsync();
            RecentProjects = allProjects
                .OrderByDescending(p => p.UpdatedAt)
                .Take(6)
                .ToList();

            ActiveProjectCount = allProjects.Count(p => p.Status == 1);
            TeamMemberCount = 0; // TODO: Implement team member count from team members
        }
        catch
        {
            RecentProjects = new List<ProjectDTO>();
            ActiveProjectCount = 0;
            TeamMemberCount = 0;
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
        if (!User.IsInRole("Project Manager"))
        {
            return Unauthorized();
        }

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
