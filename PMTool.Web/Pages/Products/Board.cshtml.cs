using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.DTOs.Board;
using PMTool.Application.DTOs.Sprint;
using PMTool.Application.Interfaces;
using PMTool.Application.Services.SubProject;
using System.Security.Claims;

using PMTool.Application.DTOs.User;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class BoardModel : PageModel
{
    private readonly ISprintService _sprintService;
    private readonly IProductBacklogService _productBacklogService;
    private readonly IProductService _productService;
    private readonly IProjectService _projectService;
    private readonly IUserAdminService _userService;
    private readonly IBoardColumnService _boardColumnService;
    private readonly ISubProjectService _subProjectService;

    public BoardModel(
        ISprintService sprintService,
        IProductBacklogService productBacklogService,
        IProductService productService,
        IProjectService projectService,
        IUserAdminService userService,
        IBoardColumnService boardColumnService,
        ISubProjectService subProjectService)
    {
        _sprintService         = sprintService;
        _productBacklogService = productBacklogService;
        _productService        = productService;
        _projectService        = projectService;
        _userService           = userService;
        _boardColumnService    = boardColumnService;
        _subProjectService     = subProjectService;
    }

    // ── Page properties ───────────────────────────────────────────────────────

    [BindProperty(SupportsGet = true)]
    public Guid ProjectId { get; set; }

    [BindProperty(SupportsGet = true, Name = "id")]
    public Guid ProductId { get; set; }

    public string ProjectName  { get; set; } = string.Empty;
    public string ProductName  { get; set; } = string.Empty;
    public bool   CanEditBoard { get; set; }

    public SprintDTO?                 ActiveSprint { get; set; }
    public List<ProductBacklogItemDTO> TodoItems      { get; set; } = new();
    public List<ProductBacklogItemDTO> InProgressItems{ get; set; } = new();
    public List<ProductBacklogItemDTO> InReviewItems  { get; set; } = new();
    public List<ProductBacklogItemDTO> DoneItems       { get; set; } = new();
    public List<ProductBacklogItemDTO> OtherItems      { get; set; } = new();
    public List<UserDTO> ActiveUsers { get; set; } = new();
    public List<BoardColumnDTO> CustomBoardColumns { get; set; } = new();

    // ── Permissions ───────────────────────────────────────────────────────────

    private void SetPermissions()
        => CanEditBoard = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

    // ── GET ───────────────────────────────────────────────────────────────────

    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id)
    {
        ProjectId = projectId;
        ProductId = id;
        SetPermissions();

        var project = await _projectService.GetProjectByIdAsync(projectId);
        ProjectName = project?.Name ?? string.Empty;

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        ProductName = product.VersionName;

        ActiveSprint = await _sprintService.GetActiveSprintAsync(ProductId);
        CustomBoardColumns = await _boardColumnService.GetColumnsByProductAsync(ProductId);

        if (ActiveSprint?.BacklogItems != null)
        {
            var allItems = ActiveSprint.BacklogItems.OrderBy(x => x.Priority).ToList();
            TodoItems       = allItems.Where(x => x.Status == 1 || x.Status == 0).ToList();
            InProgressItems = allItems.Where(x => x.Status == 2).ToList();
            InReviewItems   = allItems.Where(x => x.Status == 3).ToList();
            DoneItems       = allItems.Where(x => x.Status == 4).ToList();
            OtherItems      = allItems.Where(x => x.Status < 1 || x.Status > 4).ToList();
        }

        var users = await _userService.GetActiveUsersAsync() ?? new List<UserDTO>();
        ActiveUsers = users.ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateFieldAsync(Guid itemId, string field, string value)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field = field,
            Value = value
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    public async Task<IActionResult> OnPostUpdateOwnerAsync(Guid itemId, Guid? ownerId)
    {
        return await OnPostUpdateFieldAsync(itemId, "owner", ownerId?.ToString() ?? "");
    }

    // ── Update item status (drag-drop from board) ─────────────────────────────

    public async Task<IActionResult> OnPostUpdateItemStatusAsync(Guid itemId, int status)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        // Fetch item first so we know its SubProjectId
        var item = await _productBacklogService.GetItemByIdAsync(itemId);

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "status",
            Value  = status.ToString()
        };
        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

        // Recalculate sub-project progress whenever a board item changes status
        if (result != null && item?.SubProjectId.HasValue == true)
        {
            await _subProjectService.UpdateProgressAsync(item.SubProjectId.Value);
        }

        return new JsonResult(new { success = result != null });
    }

    // ── Complete sprint from Board ────────────────────────────────────────────

    public async Task<IActionResult> OnPostCompleteSprintAsync(Guid sprintId)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        var success = await _sprintService.CompleteSprintAsync(sprintId);

        if (success)
        {
            var backlogUrl = Url.Page("/Products/Backlog",
                new { projectId = ProjectId, id = ProductId });
            return new JsonResult(new { success = true, backlogUrl });
        }

        return new JsonResult(new { success = false });
    }

    public async Task<IActionResult> OnPostDeleteItemAsync(Guid itemId)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        var existing = await _productBacklogService.GetItemByIdAsync(itemId);
        var subProjectId = existing?.SubProjectId;

        var success = await _productBacklogService.DeleteItemAsync(itemId);

        if (success && subProjectId.HasValue)
        {
            await _subProjectService.UpdateProgressAsync(subProjectId.Value);
        }

        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostCreateItemAsync(int status, string title)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        if (string.IsNullOrWhiteSpace(title)) return new JsonResult(new { success = false, message = "Title is required" });

        var activeSprint = await _sprintService.GetActiveSprintAsync(ProductId);
        if (activeSprint == null) return new JsonResult(new { success = false, message = "No active sprint found" });

        var request = new CreateProductBacklogItemRequest
        {
            ProductId = ProductId,
            Title = title.Trim(),
            Status = status,
            Type = 2, // Default to UserStory or similar
            SprintId = activeSprint.Id
        };

        var result = await _productBacklogService.CreateBacklogItemAsync(request);
        return new JsonResult(new { success = result != null });
    }

    public async Task<IActionResult> OnPostCreateBoardColumnAsync([FromBody] CreateBoardColumnRequest request)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        request.ProductId = request.ProductId == Guid.Empty ? ProductId : request.ProductId;
        var created = await _boardColumnService.CreateColumnAsync(request);
        return new JsonResult(new { success = created != null, column = created });
    }

    public async Task<IActionResult> OnPostUpdateBoardColumnAsync([FromBody] UpdateBoardColumnRequest request)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        request.ProductId = request.ProductId == Guid.Empty ? ProductId : request.ProductId;
        var success = await _boardColumnService.UpdateColumnAsync(request);
        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostDeleteBoardColumnAsync(int statusValue)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        var success = await _boardColumnService.DeleteColumnAsync(ProductId, statusValue);
        return new JsonResult(new { success });
    }
}
