using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.DTOs.Sprint;
using PMTool.Application.Interfaces;
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

    public BoardModel(
        ISprintService sprintService,
        IProductBacklogService productBacklogService,
        IProductService productService,
        IProjectService projectService,
        IUserAdminService userService)
    {
        _sprintService         = sprintService;
        _productBacklogService = productBacklogService;
        _productService        = productService;
        _projectService        = projectService;
        _userService           = userService;
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
    public List<UserDTO> ActiveUsers { get; set; } = new();

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

        if (ActiveSprint?.BacklogItems != null)
        {
            TodoItems       = ActiveSprint.BacklogItems.Where(x => x.Status == 1).OrderBy(x => x.Priority).ToList();
            InProgressItems = ActiveSprint.BacklogItems.Where(x => x.Status == 2).OrderBy(x => x.Priority).ToList();
            InReviewItems   = ActiveSprint.BacklogItems.Where(x => x.Status == 3).OrderBy(x => x.Priority).ToList();
            DoneItems       = ActiveSprint.BacklogItems.Where(x => x.Status == 4).OrderBy(x => x.Priority).ToList();
        }

        var users = await _userService.GetActiveUsersAsync() ?? new List<UserDTO>();
        ActiveUsers = users.ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateOwnerAsync(Guid itemId, Guid? ownerId)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field = "owner",
            Value = ownerId?.ToString() ?? ""
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    // ── Update item status (drag-drop from board) ─────────────────────────────

    public async Task<IActionResult> OnPostUpdateItemStatusAsync(Guid itemId, int status)
    {
        SetPermissions();
        if (!CanEditBoard) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "status",
            Value  = status.ToString()
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
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
}
