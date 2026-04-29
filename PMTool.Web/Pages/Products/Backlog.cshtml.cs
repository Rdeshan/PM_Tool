using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Domain.Enums;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class BacklogModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IProductService _productService;
    private readonly IBacklogService _backlogService;

    public BacklogModel(IProjectService projectService, IProductService productService, IBacklogService backlogService)
    {
        _projectService = projectService;
        _productService = productService;
        _backlogService = backlogService;
    }

    public Guid ProjectId { get; set; }
    public Guid ProductId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string BacklogUrl { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }
    public List<BacklogItemDTO> BacklogItems { get; set; } = new();
    public bool CanEditBacklog { get; set; }
    public int DraftCount => BacklogItems.Count(x => x.Status == 1);
    public int ApprovedCount => BacklogItems.Count(x => x.Status == 2);

    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id, bool embedded = false)
    {
        ProjectId = projectId;
        ProductId = id;
        IsEmbedded = embedded;
        CanEditBacklog = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

        var project = await _projectService.GetProjectByIdAsync(projectId);
        ProjectName = project?.Name ?? string.Empty;

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        ProductName = product.VersionName;

        // Load backlog items for this product
        var backlogItems = await _backlogService.GetBacklogItemsAsync(ProjectId, ProductId, null, null);
        BacklogItems = backlogItems;

        BacklogUrl = Url.Page("/Backlog/Index", new { projectId, productId = id, embedded = true }) ?? string.Empty;

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateFieldAsync([FromBody] UpdateBacklogFieldRequest request)
    {
        if (request.ItemId == Guid.Empty || string.IsNullOrWhiteSpace(request.Field))
        {
            return new JsonResult(new { success = false, message = "Invalid update request." });
        }

        var updated = await _backlogService.UpdateBacklogFieldAsync(request);
        if (updated == null)
        {
            return new JsonResult(new { success = false, message = "Failed to update item." });
        }

        return new JsonResult(new { success = true, item = updated, message = "Issue updated." });
    }

    public async Task<IActionResult> OnPostCreateAsync([FromBody] CreateBacklogItemRequest request)
    {
        if (request.ProjectId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return new JsonResult(new { success = false, message = "Project and title are required." });
        }

        if (request.Type == 0) request.Type = (int)BacklogItemType.UserStory;
        if (request.Status == 0) request.Status = (int)BacklogItemStatus.Draft;

        var created = await _backlogService.CreateBacklogItemAsync(request);
        if (created == null)
        {
            return new JsonResult(new { success = false, message = "Failed to create backlog item." });
        }

        return new JsonResult(new { success = true, item = created, message = "Issue created." });
    }
}
