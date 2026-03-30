using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Services.Project;

namespace PMTool.Web.Pages.Projects;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IProjectService _projectService;

    public IEnumerable<ProjectDTO> Projects { get; set; } = new List<ProjectDTO>();
    public string? FilterStatus { get; set; }
    public bool ShowArchived { get; set; } = false;

    public IndexModel(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task OnGetAsync(string? status, bool archived = false)
    {
        ShowArchived = archived;
        FilterStatus = status;

        if (archived)
        {
            Projects = await _projectService.GetArchivedProjectsAsync();
        }
        else
        {
            var allProjects = await _projectService.GetActiveProjectsAsync();
            
            if (!string.IsNullOrEmpty(status) && int.TryParse(status, out var statusValue))
            {
                Projects = allProjects.Where(p => p.Status == statusValue);
            }
            else
            {
                Projects = allProjects;
            }
        }
    }

    public async Task<IActionResult> OnPostArchiveAsync(Guid projectId)
    {
        // Only Admin and Project Manager can archive
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
        {
            return Forbid();
        }

        var result = await _projectService.ArchiveProjectAsync(projectId);
        if (result)
            return RedirectToPage();

        return NotFound();
    }
}
