using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Web.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IDailyTaskService _dailyTaskService;
    private readonly AppDbContext _context;

    public List<DailyTask> MyTasks { get; set; } = new();

    public bool IsAllTasksView { get; set; }

    public IEnumerable<ProductBacklog> BacklogItems { get; set; } = new List<ProductBacklog>();

    public IndexModel(IDailyTaskService dailyTaskService, AppDbContext context)
    {
        _dailyTaskService = dailyTaskService;
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.IsInRole("Project Manager") || User.IsInRole("Administrator"))
        {
            IsAllTasksView = true;
            MyTasks = await _dailyTaskService.GetAllTasksAsync();
        }
        else
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (Guid.TryParse(userId, out var userGuid))
            {
                MyTasks = await _dailyTaskService.GetUserTasksAsync(userGuid);
            }

            BacklogItems = await _context.ProductBacklogs.ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddDailyTaskAsync(string taskName, string? description, Guid? backlogItemId)
    {
        if (User.IsInRole("Project Manager") || User.IsInRole("Administrator"))
        {
            return Forbid();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(taskName))
        {
            TempData["ErrorMessage"] = "Task name is required.";
            TempData["OpenDailyTaskPanel"] = true;
            return RedirectToPage();
        }

        var task = new DailyTask
        {
            TaskName = taskName.Trim(),
            Description = description?.Trim() ?? string.Empty,
            ProductBacklogId = backlogItemId,
            UserId = userGuid
        };

        var created = await _dailyTaskService.CreateAsync(task);
        TempData[created != null ? "SuccessMessage" : "ErrorMessage"] =
            created != null ? "Daily task submitted for review." : "Failed to submit daily task.";
        if (created == null)
        {
            TempData["OpenDailyTaskPanel"] = true;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditDailyTaskAsync(Guid taskId, string taskName, string? description, Guid? backlogItemId)
    {
        if (User.IsInRole("Project Manager") || User.IsInRole("Administrator"))
        {
            return Forbid();
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(taskName))
        {
            TempData["ErrorMessage"] = "Task name is required.";
            return RedirectToPage();
        }

        var success = await _dailyTaskService.EditAndResubmitAsync(taskId, userGuid, taskName, description, backlogItemId);
        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Task updated and resubmitted for review." : "Failed to update the task.";

        return RedirectToPage();
    }
}
