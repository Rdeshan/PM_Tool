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
    public List<BacklogItemDTO> Brds => Items
        .Where(x => x.Type == (int)BacklogItemType.BRD)
        .OrderBy(x => x.Priority)
        .ToList();

    public List<BacklogItemDTO> Epics => Items
        .Where(x => x.Type == (int)BacklogItemType.Epic)
        .OrderBy(x => x.Priority)
        .ToList();

    public List<BacklogItemDTO> UserStories => Items
        .Where(x => x.Type == (int)BacklogItemType.UserStory)
        .OrderBy(x => x.Priority)
        .ToList();

    public List<BacklogItemDTO> UseCases => Items
        .Where(x => x.Type == (int)BacklogItemType.UseCase)
        .OrderBy(x => x.Priority)
        .ToList();

    public List<BacklogItemDTO> ChangeRequests => Items
        .Where(x => x.Type == (int)BacklogItemType.ChangeRequest)
        .OrderBy(x => x.Priority)
        .ToList();
    public List<BacklogItemDTO> OrderedItems => BuildOrderedItems();
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
        Items = await _backlogService.GetBacklogItemsAsync(SelectedProjectId, SelectedProductId, null, SelectedStatus);

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

        var currentItems = await _backlogService.GetBacklogItemsAsync(request.ProjectId, request.ProductId, null, null);
        var brdItems = currentItems.Where(x => x.Type == (int)BacklogItemType.BRD).ToList();

        if (request.Type == (int)BacklogItemType.BRD)
        {
            if (brdItems.Count > 0)
            {
                return new JsonResult(new { success = false, message = "Only one BRD is allowed per backlog." });
            }

            request.ParentBacklogItemId = null;
        }
        else
        {
            if (brdItems.Count == 0)
            {
                return new JsonResult(new { success = false, message = "Create BRD first." });
            }
        }

        if (request.Type == (int)BacklogItemType.Epic)
        {
            if (!request.ParentBacklogItemId.HasValue)
            {
                request.ParentBacklogItemId = brdItems[0].Id;
            }

            var parentBrd = currentItems.FirstOrDefault(x => x.Id == request.ParentBacklogItemId.Value && x.Type == (int)BacklogItemType.BRD);
            if (parentBrd == null)
            {
                return new JsonResult(new { success = false, message = "Epic must belong to BRD." });
            }
        }

        if (request.Type == (int)BacklogItemType.UserStory)
        {
            if (!request.ParentBacklogItemId.HasValue)
            {
                return new JsonResult(new { success = false, message = "User Story must belong to an Epic." });
            }

            var parentEpic = currentItems.FirstOrDefault(x => x.Id == request.ParentBacklogItemId.Value && x.Type == (int)BacklogItemType.Epic);
            if (parentEpic == null)
            {
                return new JsonResult(new { success = false, message = "User Story must belong to an Epic." });
            }
        }

        if (request.Type == (int)BacklogItemType.UseCase)
        {
            if (!request.ParentBacklogItemId.HasValue)
            {
                return new JsonResult(new { success = false, message = "Use Case must belong to a User Story." });
            }

            var parentStory = currentItems.FirstOrDefault(x => x.Id == request.ParentBacklogItemId.Value && x.Type == (int)BacklogItemType.UserStory);
            if (parentStory == null)
            {
                return new JsonResult(new { success = false, message = "Use Case must belong to a User Story." });
            }
        }

        if (request.Type == (int)BacklogItemType.ChangeRequest)
        {
            if (!request.ParentBacklogItemId.HasValue)
            {
                return new JsonResult(new { success = false, message = "Change Request must belong to a User Story." });
            }

            var parentStory = currentItems.FirstOrDefault(x => x.Id == request.ParentBacklogItemId.Value && x.Type == (int)BacklogItemType.UserStory);
            if (parentStory == null)
            {
                return new JsonResult(new { success = false, message = "Change Request must belong to a User Story." });
            }

            if (!request.Title.Trim().StartsWith("CR-", StringComparison.OrdinalIgnoreCase))
            {
                var nextCrNumber = currentItems.Count(x => x.Type == (int)BacklogItemType.ChangeRequest) + 1;
                request.Title = $"CR-{nextCrNumber:000} {request.Title.Trim()}";
            }
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

    public async Task<IActionResult> OnPostDeleteManyAsync([FromBody] DeleteManyRequest request)
    {
        if (request.ItemIds.Count == 0)
        {
            return new JsonResult(new { success = false, message = "No items selected." });
        }

        var deletedCount = 0;
        foreach (var itemId in request.ItemIds.Where(x => x != Guid.Empty).Distinct())
        {
            if (await _backlogService.DeleteItemAsync(itemId))
            {
                deletedCount++;
            }
        }

        if (deletedCount == 0)
        {
            return new JsonResult(new { success = false, message = "Failed to delete selected items." });
        }

        return new JsonResult(new
        {
            success = true,
            message = $"Deleted {deletedCount} item(s)."
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

    public List<BacklogItemDTO> GetStoriesForEpic(Guid epicId)
    {
        return UserStories.Where(x => x.ParentBacklogItemId == epicId).ToList();
    }

    public List<BacklogItemDTO> GetEpicsForBrd(Guid brdId)
    {
        return Epics.Where(x => x.ParentBacklogItemId == brdId).ToList();
    }

    public List<BacklogItemDTO> GetUseCasesForStory(Guid storyId)
    {
        return UseCases.Where(x => x.ParentBacklogItemId == storyId).ToList();
    }

    public List<BacklogItemDTO> GetChangeRequestsForStory(Guid storyId)
    {
        return ChangeRequests.Where(x => x.ParentBacklogItemId == storyId).ToList();
    }

    public int GetIndentLevel(BacklogItemDTO item)
    {
        return item.Type switch
        {
            (int)BacklogItemType.BRD => 0,
            (int)BacklogItemType.Epic => 1,
            (int)BacklogItemType.UserStory => 2,
            (int)BacklogItemType.UseCase => 3,
            (int)BacklogItemType.ChangeRequest => 3,
            _ => 0
        };
    }

    public bool HasChildren(BacklogItemDTO item)
    {
        return Items.Any(x => x.ParentBacklogItemId == item.Id);
    }

    private List<BacklogItemDTO> BuildOrderedItems()
    {
        var ordered = new List<BacklogItemDTO>();
        var added = new HashSet<Guid>();

        foreach (var brd in Brds)
        {
            ordered.Add(brd);
            added.Add(brd.Id);

            var epics = GetEpicsForBrd(brd.Id);
            foreach (var epic in epics)
            {
                ordered.Add(epic);
                added.Add(epic.Id);

                var stories = GetStoriesForEpic(epic.Id);
                foreach (var story in stories)
                {
                    ordered.Add(story);
                    added.Add(story.Id);

                    var useCases = GetUseCasesForStory(story.Id);
                    foreach (var useCase in useCases)
                    {
                        ordered.Add(useCase);
                        added.Add(useCase.Id);
                    }

                    var changeRequests = GetChangeRequestsForStory(story.Id);
                    foreach (var changeRequest in changeRequests)
                    {
                        ordered.Add(changeRequest);
                        added.Add(changeRequest.Id);
                    }
                }
            }
        }

        var remaining = Items
            .Where(x => !added.Contains(x.Id))
            .OrderBy(x => x.Priority);

        ordered.AddRange(remaining);
        return ordered;
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

    public class DeleteManyRequest
    {
        public List<Guid> ItemIds { get; set; } = new();
    }
}
