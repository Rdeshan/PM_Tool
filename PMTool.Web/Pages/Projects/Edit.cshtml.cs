using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Services.Project;
using PMTool.Application.Validators.Project;
using PMTool.Domain.Enums;

namespace PMTool.Web.Pages.Projects;

[Authorize(Roles = "Administrator,Project Manager")]
public class EditModel : PageModel
{
    private readonly IProjectService _projectService;

    [BindProperty]
    public UpdateProjectRequest Input { get; set; } = new();

    public ProjectDTO? Project { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public Guid ProjectId { get; set; }

    public EditModel(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Project = await _projectService.GetProjectByIdAsync(id);
        if (Project == null)
            return NotFound();

        ProjectId = id;
        Input = new UpdateProjectRequest
        {
            Name = Project.Name,
            Description = Project.Description,
            ClientName = Project.ClientName,
            ProjectCode = Project.ProjectCode,
            StartDate = Project.StartDate,
            ExpectedEndDate = Project.ExpectedEndDate,
            ColourCode = Project.ColourCode,
            Status = Project.Status
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id)
    {
        var isEmbedded = string.Equals(Request.Query["embedded"], "true", StringComparison.OrdinalIgnoreCase);

        var validator = new CreateProjectRequestValidator();
        var validationResult = await validator.ValidateAsync(new CreateProjectRequest
        {
            Name = Input.Name,
            Description = Input.Description,
            ClientName = Input.ClientName,
            ProjectCode = Input.ProjectCode,
            StartDate = Input.StartDate,
            ExpectedEndDate = Input.ExpectedEndDate,
            ColourCode = Input.ColourCode
        });

        if (!validationResult.IsValid)
        {
            ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            Project = await _projectService.GetProjectByIdAsync(id);
            ProjectId = id;
            return Page();
        }

        var result = await _projectService.UpdateProjectAsync(id, Input);

        if (!result)
        {
            ErrorMessage = "Failed to update project.";
            Project = await _projectService.GetProjectByIdAsync(id);
            ProjectId = id;
            return Page();
        }

        if (isEmbedded)
        {
            return Content("<script>window.parent.location.reload();</script>", "text/html");
        }

        return RedirectToPage("./Details", new { id });
    }
}

