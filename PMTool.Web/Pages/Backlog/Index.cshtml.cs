using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.Backlog;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IBacklogService _backlogService;
    private readonly IProjectRepository _projectRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public IndexModel(
        IBacklogService backlogService,
        IProjectRepository projectRepository,
        IProductRepository productRepository,
        IUserRepository userRepository)
    {
        _backlogService = backlogService;
        _projectRepository = projectRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public List<BacklogItemDTO> Items { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<User> Owners { get; set; } = new();

    public Guid SelectedProjectId { get; set; }
    public Guid? SelectedProductId { get; set; }
    public int? SelectedStatus { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? productId = null, Guid? projectId = null, int? status = null)
    {
        Projects = (await _projectRepository.GetActiveAsync()).ToList();
        Owners = (await _userRepository.GetAllAsync()).OrderBy(x => x.FirstName).ToList();

        if (productId.HasValue && productId.Value != Guid.Empty)
        {
            var product = await _productRepository.GetByIdAsync(productId.Value);
            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToPage("/Projects/Index");
            }

            projectId = product.ProjectId;
        }

        if (!projectId.HasValue || projectId.Value == Guid.Empty)
        {
            projectId = Projects.FirstOrDefault()?.Id;
        }

        if (!projectId.HasValue || projectId.Value == Guid.Empty)
        {
            return Page();
        }

        SelectedProjectId = projectId.Value;
        SelectedProductId = productId;
        SelectedStatus = status;

        Products = (await _productRepository.GetByProjectAsync(SelectedProjectId)).ToList();
        Items = await _backlogService.GetBacklogItemsAsync(SelectedProjectId, SelectedProductId, SelectedStatus);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync([FromBody] CreateBacklogItemRequest request)
    {
        if (request.ProjectId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return new JsonResult(new { success = false, message = "Project and title are required." });
        }

        if (request.Type == 0)
        {
            request.Type = (int)BacklogItemType.UserStory;
        }

        if (request.Status == 0)
        {
            request.Status = (int)BacklogItemStatus.Draft;
        }

        var created = await _backlogService.CreateBacklogItemAsync(request);
        if (created == null)
        {
            return new JsonResult(new { success = false, message = "Failed to create backlog item." });
        }

        return new JsonResult(new { success = true, item = created, message = "Issue created." });
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

    public async Task<IActionResult> OnPostReorderAsync([FromBody] ReorderRequest request)
    {
        if (request.ProjectId == Guid.Empty || request.ItemIds.Count == 0)
        {
            return new JsonResult(new { success = false, message = "Invalid reorder request." });
        }

        var reorderItems = request.ItemIds
            .Select((id, index) => new ReorderBacklogItemRequest
            {
                ItemId = id,
                Priority = index + 1
            })
            .ToList();

        var result = await _backlogService.ReorderItemsAsync(request.ProjectId, request.ProductId, reorderItems);
        return new JsonResult(new
        {
            success = result,
            message = result ? "Priority updated." : "Failed to reorder items."
        });
    }

    public async Task<IActionResult> OnPostDeleteAsync([FromBody] DeleteRequest request)
    {
        if (request.ItemId == Guid.Empty)
        {
            return new JsonResult(new { success = false, message = "Invalid item." });
        }

        var result = await _backlogService.DeleteItemAsync(request.ItemId);
        return new JsonResult(new
        {
            success = result,
            message = result ? "Issue deleted." : "Failed to delete issue."
        });
    }

    public string GetTypeBadgeClass(int type)
    {
        return type switch
        {
            (int)BacklogItemType.BRD => "bg-primary",
            (int)BacklogItemType.UserStory => "bg-info",
            (int)BacklogItemType.UseCase => "bg-secondary",
            (int)BacklogItemType.Epic => "bg-purple",
            (int)BacklogItemType.ChangeRequest => "bg-warning text-dark",
            _ => "bg-secondary"
        };
    }

    public class ReorderRequest
    {
        public Guid ProjectId { get; set; }
        public Guid? ProductId { get; set; }
        public List<Guid> ItemIds { get; set; } = new();
    }

    public class DeleteRequest
    {
        public Guid ItemId { get; set; }
    }
}
