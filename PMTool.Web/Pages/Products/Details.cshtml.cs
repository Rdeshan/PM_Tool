using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Product;
using PMTool.Application.Services.Product;
using PMTool.Application.Services.Project;
using PMTool.Application.Services.SubProject;
using PMTool.Application.Services.Team;
using PMTool.Application.Interfaces;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.DTOs.Team;
using PMTool.Application.DTOs.User;
using System.Security.Claims;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IProjectService _projectService;
    private readonly ISubProjectService _subProjectService;
    private readonly ITeamService _teamService;
    private readonly IUserAdminService _userService;
    private readonly IBacklogService _backlogService;

    public DetailsModel(
        IProductService productService, 
        IProjectService projectService,
        ISubProjectService subProjectService,
        ITeamService teamService,
        IUserAdminService userService,
        IBacklogService backlogService)
    {
        _productService = productService;
        _projectService = projectService;
        _subProjectService = subProjectService;
        _teamService = teamService;
        _userService = userService;
        _backlogService = backlogService;
    }

    public Guid ProjectId { get; set; }
    public string Tab { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public ProductDTO? Product { get; set; }
    public List<ReleaseNotesDTO> ReleaseNotes { get; set; } = new();
    public List<SubProjectDTO> SubProjects { get; set; } = new();
    public List<TeamDTO> AvailableTeams { get; set; } = new();
    public List<UserDTO> ProjectManagers { get; set; } = new();
    public bool CanEditProduct { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id)
    {
        ProjectId = projectId;
        CanEditProduct = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

        var project = await _projectService.GetProjectByIdAsync(projectId);
        ProjectName = project?.Name ?? string.Empty;

        Product = await _productService.GetProductByIdAsync(id);
        if (Product == null)
            return NotFound();

        ReleaseNotes = (await _productService.GetReleaseNotesAsync(id)).ToList();
        SubProjects = await _subProjectService.GetSubProjectsByProductAsync(id);
        
        if (CanEditProduct)
        {
            AvailableTeams = (await _teamService.GetActiveTeamsAsync()).ToList();
            var allUsers = await _userService.GetActiveUsersAsync();
            ProjectManagers = allUsers.Where(u => u.Roles.Contains("Project Manager") || u.Roles.Contains("Administrator")).ToList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid projectId, Guid id)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        var result = await _productService.DeleteProductAsync(id);
        if (!result)
            return NotFound();

        return RedirectToPage("/Projects/Details", new { id = projectId, tab = "products" });
    }

    public async Task<IActionResult> OnPostAddReleaseNotesAsync(Guid projectId, Guid id, string title, string content)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        await _productService.AddReleaseNotesAsync(id, title, content, userId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostPublishReleaseNotesAsync(Guid projectId, Guid id, Guid releaseNotesId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _productService.PublishReleaseNotesAsync(releaseNotesId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostDeleteReleaseNotesAsync(Guid projectId, Guid id, Guid releaseNotesId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _productService.DeleteReleaseNotesAsync(releaseNotesId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostAddSubProjectAsync(Guid projectId, Guid id, CreateSubProjectRequest request)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        request.ProductId = id;
        var result = await _subProjectService.CreateSubProjectAsync(request);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostDeleteSubProjectAsync(Guid projectId, Guid id, Guid subProjectId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _subProjectService.DeleteSubProjectAsync(subProjectId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostUpdateSubProjectAsync(Guid projectId, Guid id, Guid subProjectId, UpdateSubProjectRequest request)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        var result = await _subProjectService.UpdateSubProjectAsync(subProjectId, request);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostAddDependencyAsync(Guid projectId, Guid id, Guid subProjectId, Guid dependsOnSubProjectId, string? notes)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _subProjectService.AddDependencyAsync(subProjectId, dependsOnSubProjectId, notes);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostRemoveDependencyAsync(Guid projectId, Guid id, Guid dependencyId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _subProjectService.RemoveDependencyAsync(dependencyId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<JsonResult> OnGetSubProjectBacklogAsync(Guid projectId, Guid id, Guid subId)
    {
        var items = await _backlogService.GetBacklogItemsAsync(projectId, id, subId, null);
        return new JsonResult(items);
    }

    public string GetStatusLabel(int status)
    {
        return status switch
        {
            1 => "Planned",
            2 => "In Development",
            3 => "In Testing",
            4 => "Released",
            5 => "Deprecated",
            _ => "Unknown"
        };
    }

    public string GetStatusBadgeClass(int status)
    {
        return status switch
        {
            1 => "bg-secondary",
            2 => "bg-info",
            3 => "bg-warning",
            4 => "bg-success",
            5 => "bg-danger",
            _ => "bg-secondary"
        };
    }

    public string GetReleaseTypeLabel(int releaseType)
    {
        return releaseType switch
        {
            1 => "Major",
            2 => "Minor",
            3 => "Patch",
            4 => "Hotfix",
            _ => "Unknown"
        };
    }

    public string GetSubProjectStatusLabel(int status)
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

    public string GetSubProjectStatusBadgeClass(int status)
    {
        return status switch
        {
            1 => "bg-secondary",
            2 => "bg-primary",
            3 => "bg-warning text-dark",
            4 => "bg-success",
            _ => "bg-secondary"
        };
    }

    public string GetProgressColorClass(int progress)
    {
        if (progress < 25) return "bg-danger";
        if (progress < 75) return "bg-warning";
        return "bg-success";
    }
}
