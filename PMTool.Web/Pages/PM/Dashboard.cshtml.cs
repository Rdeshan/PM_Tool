using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Dashboard;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;

namespace PMTool.Web.Pages.PM;

[Authorize(Roles = "Project Manager")]
public class DashboardModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IDashboardService _dashboardService;
    private readonly IDailyTaskService _dailyTaskService;

    public List<ProjectDTO> RecentProjects { get; set; } = new();
    public List<ProjectDTO> AllProjects { get; set; } = new();
    public DashboardDto Dashboard { get; set; } = new();
    public List<CollabProjectDto> CollabData { get; set; } = new();
    public List<DailyTask> PendingDailyTasks { get; set; } = new();

    public DashboardModel(IProjectService projectService, IDashboardService dashboardService, IDailyTaskService dailyTaskService)
    {
        _projectService = projectService;
        _dashboardService = dashboardService;
        _dailyTaskService = dailyTaskService;
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

        // Load all projects; recent = latest 6
        try
        {
            var allProjects = await _projectService.GetAllProjectsAsync();
            AllProjects = allProjects.OrderByDescending(p => p.UpdatedAt).ToList();
            RecentProjects = AllProjects.Take(6).ToList();
        }
        catch
        {
            AllProjects = new List<ProjectDTO>();
            RecentProjects = new List<ProjectDTO>();
        }

        try
        {
            CollabData = await _dashboardService.GetCollabDataAsync();
        }
        catch
        {
            CollabData = new List<CollabProjectDto>();
        }

        try
        {
            PendingDailyTasks = await _dailyTaskService.GetPendingTasksForPMAsync();
        }
        catch
        {
            PendingDailyTasks = new List<DailyTask>();
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

    public async Task<IActionResult> OnPostApproveDailyTaskAsync(Guid taskId)
    {
        if (!User.IsInRole("Project Manager"))
        {
            return Unauthorized();
        }

        var pmUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var success = await _dailyTaskService.AcceptTaskAsync(taskId, pmUserId);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Daily task approved." : "Failed to approve the daily task.";
        TempData["OpenApproveTasksModal"] = true;

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostClarifyDailyTaskAsync(Guid taskId, string comment)
    {
        if (!User.IsInRole("Project Manager"))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            TempData["ErrorMessage"] = "Please describe what needs clarification.";
            TempData["OpenApproveTasksModal"] = true;
            return RedirectToPage();
        }

        var pmUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var success = await _dailyTaskService.SendForClarificationAsync(taskId, pmUserId, comment);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Clarification requested from the user." : "Failed to update the daily task.";
        TempData["OpenApproveTasksModal"] = true;

        return RedirectToPage();
    }
}
