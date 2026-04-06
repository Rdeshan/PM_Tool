using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.Services.SubProject;
using System.Security.Claims;

namespace PMTool.Web.Pages.SubProjects;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ISubProjectService _subProjectService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(
        ISubProjectService subProjectService,
        ILogger<DetailsModel> logger)
    {
        _subProjectService = subProjectService;
        _logger = logger;
    }

    public SubProjectDTO? SubProject { get; set; }
    public Guid ProductId { get; set; }
    public bool CanEdit { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid productId)
    {
        try
        {
            ProductId = productId;
            SubProject = await _subProjectService.GetSubProjectAsync(id);

            if (SubProject == null)
            {
                TempData["Error"] = "Sub-project not found";
                return RedirectToPage("Index", new { productId });
            }

            CanEdit = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading sub-project details for {SubProjectId}", id);
            TempData["Error"] = "Failed to load sub-project details";
            return RedirectToPage("Index", new { productId });
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, Guid productId)
    {
        try
        {
            if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            {
                return Forbid();
            }

            if (await _subProjectService.DeleteSubProjectAsync(id))
            {
                TempData["Success"] = "Sub-project deleted successfully";
                return RedirectToPage("Index", new { productId });
            }

            TempData["Error"] = "Failed to delete sub-project";
            return RedirectToPage("Details", new { id, productId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sub-project {SubProjectId}", id);
            TempData["Error"] = "An error occurred while deleting the sub-project";
            return RedirectToPage("Details", new { id, productId });
        }
    }

    public async Task<IActionResult> OnPostAddDependencyAsync(Guid id, Guid productId, Guid dependsOnSubProjectId, string? notes)
    {
        try
        {
            if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            {
                return Forbid();
            }

            if (id == dependsOnSubProjectId)
            {
                TempData["Error"] = "A sub-project cannot depend on itself";
                return RedirectToPage("Details", new { id, productId });
            }

            if (await _subProjectService.AddDependencyAsync(id, dependsOnSubProjectId, notes))
            {
                TempData["Success"] = "Dependency added successfully";
            }
            else
            {
                TempData["Error"] = "Failed to add dependency";
            }

            return RedirectToPage("Details", new { id, productId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding dependency to sub-project {SubProjectId}", id);
            TempData["Error"] = "An error occurred while adding the dependency";
            return RedirectToPage("Details", new { id, productId });
        }
    }

    public async Task<IActionResult> OnPostRemoveDependencyAsync(Guid id, Guid productId, Guid dependencyId)
    {
        try
        {
            if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            {
                return Forbid();
            }

            if (await _subProjectService.RemoveDependencyAsync(dependencyId))
            {
                TempData["Success"] = "Dependency removed successfully";
            }
            else
            {
                TempData["Error"] = "Failed to remove dependency";
            }

            return RedirectToPage("Details", new { id, productId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing dependency from sub-project {SubProjectId}", id);
            TempData["Error"] = "An error occurred while removing the dependency";
            return RedirectToPage("Details", new { id, productId });
        }
    }

    public string GetStatusName(int status)
    {
        return status switch
        {
            1 => "Not Started",
            2 => "In Progress",
            3 => "In Review",
            4 => "Completed",
            _ => "Unknown"
        };
    }

    public string GetStatusBadgeClass(int status)
    {
        return status switch
        {
            1 => "bg-secondary",
            2 => "bg-info",
            3 => "bg-warning text-dark",
            4 => "bg-success",
            _ => "bg-secondary"
        };
    }

    public string GetProgressBarClass(int progress)
    {
        return progress switch
        {
            >= 75 => "bg-success",
            >= 50 => "bg-info",
            >= 25 => "bg-warning",
            _ => "bg-danger"
        };
    }
}
