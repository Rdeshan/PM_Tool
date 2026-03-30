using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Services.Project;
using PMTool.Application.Validators.Project;
using PMTool.Domain.Enums;
using System.Security.Claims;

namespace PMTool.Web.Pages.Projects;

[Authorize(Roles = "Administrator,Project Manager")]
public class CreateModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    [BindProperty]
    public CreateProjectRequest Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public CreateModel(IProjectService projectService, IAuthorizationService authorizationService)
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var validator = new CreateProjectRequestValidator();
        var validationResult = await validator.ValidateAsync(Input);

        if (!validationResult.IsValid)
        {
            ErrorMessage = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Page();
        }

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        var result = await _projectService.CreateProjectAsync(Input, userId);

        if (!result)
        {
            ErrorMessage = "Failed to create project. Project code may already exist.";
            return Page();
        }

        SuccessMessage = "Project created successfully!";
        return RedirectToPage("./Index");
    }
}
