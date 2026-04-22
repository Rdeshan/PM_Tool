using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.Services.SubProject;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.SubProjects;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ISubProjectService _subProjectService;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        ISubProjectService subProjectService,
        IProductRepository productRepository,
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        ILogger<IndexModel> logger)
    {
        _subProjectService = subProjectService;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _logger = logger;
    }

    public List<SubProjectDTO> SubProjects { get; set; } = new();
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string StatusFilter { get; set; } = "All";
    public List<SelectListItem> ModuleOwners { get; set; } = new();
    public List<SelectListItem> Teams { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid productId, string? status = "All")
    {
        try
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToPage("/Products/Index");
            }

            ProductId = productId;
            ProductName = product.VersionName;
            StatusFilter = status ?? "All";

            var subProjects = await _subProjectService.GetSubProjectsByProductAsync(productId);

            // Filter by status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                SubProjects = subProjects
                    .Where(sp => GetStatusName(sp.Status) == status)
                    .ToList();
            }
            else
            {
                SubProjects = subProjects;
            }

            // Sort by due date
            SubProjects = SubProjects.OrderBy(sp => sp.DueDate ?? DateTime.MaxValue).ToList();

            if (User.IsInRole("Administrator") || User.IsInRole("Project Manager"))
            {
                var users = await _userRepository.GetAllAsync();
                ModuleOwners = users
                    .OrderBy(u => u.FirstName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.FirstName} {u.LastName} ({u.Email})"
                    })
                    .ToList();

                var teams = await _teamRepository.GetAllAsync();
                Teams = teams
                    .OrderBy(t => t.Name)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    })
                    .ToList();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading sub-projects for product {ProductId}", productId);
            TempData["Error"] = "Failed to load sub-projects";
            return Page();
        }
    }

    private string GetStatusName(int status)
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
            1 => "bg-secondary",      // Not Started
            2 => "bg-info",           // In Progress
            3 => "bg-warning text-dark", // In Review
            4 => "bg-success",        // Completed
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

    public async Task<IActionResult> OnPostUpdateSubProjectAsync(Guid productId, Guid subProjectId, UpdateSubProjectRequest request)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        if (request.StartDate.HasValue && request.DueDate.HasValue && request.StartDate > request.DueDate)
        {
            TempData["Error"] = "Due date must be after start date";
            return RedirectToPage(new { productId });
        }

        request.TeamIds ??= new List<Guid>();
        request.TeamRoles ??= new List<string>();

        var result = await _subProjectService.UpdateSubProjectAsync(subProjectId, request);
        TempData[result ? "Success" : "Error"] = result ? "Sub-project updated successfully" : "Failed to update sub-project";

        return RedirectToPage(new { productId });
    }
}
