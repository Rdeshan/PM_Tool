using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Services.Project;
using PMTool.Domain.Entities;

namespace PMTool.Web.Pages.Projects;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IProjectService _projectService;

    public ProjectDTO? Project { get; set; }
    public IEnumerable<Domain.Entities.User> TeamMembers { get; set; } = new List<Domain.Entities.User>();
    public string? ErrorMessage { get; set; }

    public DetailsModel(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Project = await _projectService.GetProjectByIdAsync(id);
        if (Project == null)
            return NotFound();

        TeamMembers = await _projectService.GetProjectTeamAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        // Only Admin and Project Manager can delete
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
        {
            return Forbid();
        }

        var result = await _projectService.DeleteProjectAsync(id);
        if (!result)
        {
            ErrorMessage = "Failed to delete project.";
            return RedirectToPage();
        }

        return RedirectToPage("./Index");
    }
}
