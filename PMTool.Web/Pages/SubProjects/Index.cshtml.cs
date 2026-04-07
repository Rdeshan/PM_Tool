using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.Services.SubProject;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.SubProjects;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ISubProjectService _subProjectService;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        ISubProjectService subProjectService,
        IProductRepository productRepository,
        ILogger<IndexModel> logger)
    {
        _subProjectService = subProjectService;
        _productRepository = productRepository;
        _logger = logger;
    }

    public List<SubProjectDTO> SubProjects { get; set; } = new();
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string StatusFilter { get; set; } = "All";

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
}
