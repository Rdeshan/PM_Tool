// //using Microsoft.AspNetCore.Authorization;
// //using Microsoft.AspNetCore.Mvc;
// //using Microsoft.AspNetCore.Mvc.RazorPages;
// //using PMTool.Application.DTOs.Backlog;
// //using PMTool.Application.Interfaces;

// //namespace PMTool.Web.Pages.Products;

// //[Authorize]
// //public class BacklogModel : PageModel
// //{
// //    private readonly IProjectService _projectService;
// //    private readonly IProductService _productService;
// //    private readonly IProductBacklogService _productBacklogService;
// //    private readonly IUserAdminService _userService;

// //    public BacklogModel(
// //        IProjectService projectService,
// //        IProductService productService,
// //        IProductBacklogService productBacklogService,
// //        IUserAdminService userService)
// //    {
// //        _projectService = projectService;
// //        _productService = productService;
// //        _productBacklogService = productBacklogService;
// //        _userService = userService;
// //    }

// //    // Page Properties
// //    public Guid ProjectId { get; set; }
// //    public Guid ProductId { get; set; }
// //    public string ProjectName { get; set; } = string.Empty;
// //    public string ProductName { get; set; } = string.Empty;
// //    public bool IsEmbedded { get; set; }
// //    public bool CanEditBacklog { get; set; }

// //    public List<ProductBacklogItemDTO> BacklogItems { get; set; } = new();
// //    public List<BacklogItemTypeDTO> ItemTypes { get; set; } = new();
// //    public List<dynamic> ActiveUsers { get; set; } = new();

// //    public int DraftCount => BacklogItems.Count(x => x.Status == 1);
// //    public int ApprovedCount => BacklogItems.Count(x => x.Status == 2);

// //    // Bind Properties
// //    [BindProperty]
// //    public CreateProductBacklogItemRequest NewItem { get; set; } = new();

// //    // ====================== GET ======================
// //    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id, bool embedded = false)
// //    {
// //        ProjectId = projectId;
// //        ProductId = id;
// //        IsEmbedded = embedded;

// //        CanEditBacklog = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

// //        var project = await _projectService.GetProjectByIdAsync(projectId);
// //        ProjectName = project?.Name ?? string.Empty;

// //        var product = await _productService.GetProductByIdAsync(id);
// //        if (product == null) return NotFound();

// //        ProductName = product.VersionName;

// //        // Load Backlog Items
// //        BacklogItems = await _productBacklogService.GetBacklogItemsAsync(ProductId, null);

// //        // Load supporting data
// //        ItemTypes = _productBacklogService.GetBacklogItemTypes();

// //        var users = await _userService.GetActiveUsersAsync();
// //        ActiveUsers = users.Select(u => new
// //        {
// //            id = u.Id.ToString(),
// //            displayName = u.DisplayName ?? $"{u.FirstName} {u.LastName}"
// //        }).Cast<dynamic>().ToList();

// //        return Page();
// //    }

// //    // ====================== CREATE (AJAX + Traditional) ======================
// //    public async Task<IActionResult> OnPostCreateAsync()
// //    {
// //        if (!CanEditBacklog)
// //            return Forbid();

// //        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(NewItem.Title))
// //        {
// //            return BadRequest("Title is required");
// //        }

// //        NewItem.ProductId = ProductId;

// //        var createdItem = await _productBacklogService.CreateBacklogItemAsync(NewItem);

// //        if (createdItem == null)
// //            return BadRequest("Failed to create backlog item");

// //        // If AJAX request → return JSON (for instant UI update)
// //        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
// //            Request.ContentType?.Contains("application/json") == true)
// //        {
// //            return new JsonResult(new
// //            {
// //                success = true,
// //                item = new
// //                {
// //                    id = createdItem.Id,
// //                    key = createdItem.Key,
// //                    title = createdItem.Title,
// //                    type = createdItem.Type,
// //                    typeName = createdItem.TypeName ?? createdItem.Type,
// //                    status = createdItem.Status,
// //                    statusName = GetStatusName(createdItem.Status),
// //                    ownerName = createdItem.OwnerName ?? "Unassigned",
// //                    storyPoints = createdItem.StoryPoints,
// //                    dueDate = createdItem.DueDate,
// //                    priority = createdItem.Priority
// //                }
// //            });
// //        }

// //        // Traditional form submit (fallback)
// //        TempData["SuccessMessage"] = "Backlog item created successfully!";
// //        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
// //    }

// //    // ====================== OTHER HANDLERS ======================
// //    public async Task<IActionResult> OnPostUpdateStatusAsync(Guid itemId, int status)
// //    {
// //        if (!CanEditBacklog) return Forbid();

// //        var request = new UpdateProductBacklogFieldRequest
// //        {
// //            ItemId = itemId,
// //            Field = "status",
// //            Value = status.ToString()
// //        };

// //        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

// //        TempData[result != null ? "SuccessMessage" : "ErrorMessage"] = 
// //            result != null ? "Status updated" : "Failed to update status";

// //        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
// //    }

// //    public async Task<IActionResult> OnPostUpdateStoryPointsAsync(Guid itemId, int storyPoints)
// //    {
// //        if (!CanEditBacklog) return Forbid();

// //        var request = new UpdateProductBacklogFieldRequest
// //        {
// //            ItemId = itemId,
// //            Field = "storypoints",
// //            Value = storyPoints.ToString()
// //        };

// //        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

// //        TempData[result != null ? "SuccessMessage" : "ErrorMessage"] = 
// //            result != null ? "Story points updated" : "Failed to update story points";

// //        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
// //    }

// //    public async Task<IActionResult> OnPostDeleteAsync(Guid itemId)
// //    {
// //        if (!CanEditBacklog) return Forbid();

// //        var success = await _productBacklogService.DeleteItemAsync(itemId);

// //        TempData[success ? "SuccessMessage" : "ErrorMessage"] = 
// //            success ? "Item deleted successfully" : "Failed to delete item";

// //        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
// //    }

// //    // Optional: Pure API-style handlers (you can use these from JS too)
// //    public async Task<IActionResult> OnPostCreateItemAsync([FromBody] CreateProductBacklogItemRequest request)
// //    {
// //        if (!CanEditBacklog) return Forbid();

// //        request.ProductId = ProductId;
// //        var item = await _productBacklogService.CreateBacklogItemAsync(request);

// //        return item != null 
// //            ? new JsonResult(new { success = true, item }) 
// //            : BadRequest(new { success = false, message = "Creation failed" });
// //    }

// //    // Helper method
// //    private string GetStatusName(int status) => status switch
// //    {
// //        1 => "Draft",
// //        2 => "Approved",
// //        3 => "In Progress",
// //        4 => "Done",
// //        _ => "To Do"
// //    };
// //}
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.RazorPages;
// using PMTool.Application.DTOs.Backlog;
// using PMTool.Application.Interfaces;
// using PMTool.Domain.Enums;
// using PMTool.Application.Services.SubProject;
// using PMTool.Application.DTOs.SubProject;

// namespace PMTool.Web.Pages.Products;

// [Authorize]
// public class BacklogModel : PageModel
// {
//     private readonly IProjectService _projectService;
//     private readonly IProductService _productService;
//     private readonly IProductBacklogService _productBacklogService;
//     private readonly IUserAdminService _userService;

//     public BacklogModel(
//         IProjectService projectService,
//         IProductService productService,
//         IProductBacklogService productBacklogService,
//         IUserAdminService userService)
//     {
//         _projectService = projectService;
//         _productService = productService;
//         _productBacklogService = productBacklogService;
//         _userService = userService;
//     }

//     // Page Properties
//     [BindProperty(SupportsGet = true)]
//     public Guid ProjectId { get; set; }
    
//     [BindProperty(SupportsGet = true, Name = "id")]
//     public Guid ProductId { get; set; }

//     public string ProjectName { get; set; } = string.Empty;
//     public string ProductName { get; set; } = string.Empty;
//     public bool IsEmbedded { get; set; }
//     public bool CanEditBacklog { get; set; }

//     public List<ProductBacklogItemDTO> BacklogItems { get; set; } = new();
//     public List<BacklogItemTypeDTO> ItemTypes { get; set; } = new();
//     public List<dynamic> ActiveUsers { get; set; } = new();

//     public int DraftCount => BacklogItems.Count(x => x.Status == 1);
//     public int ApprovedCount => BacklogItems.Count(x => x.Status == 2);

//     // Bind Properties
//     [BindProperty]
//     public CreateProductBacklogItemRequest NewItem { get; set; } = new();

//     private void SetPermissions()
//     {
//         CanEditBacklog = User.IsInRole("Administrator") || User.IsInRole("Project Manager");
//     }

//     // ====================== GET ======================
//     public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id, bool embedded = false)
//     {
//         ProjectId = projectId;
//         ProductId = id;
//         IsEmbedded = embedded;

//         SetPermissions();

//         var project = await _projectService.GetProjectByIdAsync(projectId);
//         ProjectName = project?.Name ?? string.Empty;

//         var product = await _productService.GetProductByIdAsync(id);
//         if (product == null) return NotFound();

//         ProductName = product.VersionName;

//         // Load Backlog Items
//         BacklogItems = await _productBacklogService.GetBacklogItemsAsync(ProductId, null);

//         // Load supporting data
//         ItemTypes = _productBacklogService.GetBacklogItemTypes();

//         var users = await _userService.GetActiveUsersAsync();
//         ActiveUsers = users.Select(u => new
//         {
//             id = u.Id.ToString(),
//             displayName = u.DisplayName ?? $"{u.FirstName} {u.LastName}"
//         }).Cast<dynamic>().ToList();

//         return Page();
//     }

//     // ====================== CREATE (AJAX + Traditional) ======================
//     public async Task<IActionResult> OnPostCreateAsync()
//     {
//         SetPermissions();
//         if (!CanEditBacklog)
//             return Forbid();

//         if (!ModelState.IsValid || string.IsNullOrWhiteSpace(NewItem.Title))
//         {
//             return new JsonResult(new
//             {
//                 success = false,
//                 message = "Title is required"
//             })
//             { StatusCode = 400 };
//         }

//         NewItem.ProductId = ProductId;

//         var createdItem = await _productBacklogService.CreateBacklogItemAsync(NewItem);

//         if (createdItem == null)
//         {
//             return new JsonResult(new
//             {
//                 success = false,
//                 message = "Failed to create backlog item"
//             })
//             { StatusCode = 400 };
//         }

//         // Always return JSON (this is what the JavaScript expects)
//         return new JsonResult(new
//         {
//             success = true,
//             item = new
//             {
//                 id = createdItem.Id,
//                 key = $"PB-{createdItem.Id.ToString("N").Substring(0, 8).ToUpper()}",
//                 title = createdItem.Title,
//                 type = createdItem.Type,
//                 typeName = !string.IsNullOrEmpty(createdItem.TypeName)
//                            ? createdItem.TypeName
//                            : createdItem.Type.ToString(),
//                 status = createdItem.Status,
//                 statusName = GetStatusName(createdItem.Status),
//                 ownerName = createdItem.OwnerName ?? "Unassigned",
//                 storyPoints = createdItem.StoryPoints,
//                 dueDate = createdItem.DueDate?.ToString("MMM dd") ?? "",
//                 priority = createdItem.Priority
//             }
//         });
//     }
//     // ====================== OTHER HANDLERS ======================
//     public async Task<IActionResult> OnPostUpdateStatusAsync(Guid itemId, int status)
//     {
//         SetPermissions();
//         if (!CanEditBacklog) return Forbid();

//         var request = new UpdateProductBacklogFieldRequest
//         {
//             ItemId = itemId,
//             Field = "status",
//             Value = status.ToString()
//         };

//         var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

//         TempData[result != null ? "SuccessMessage" : "ErrorMessage"] =
//             result != null ? "Status updated" : "Failed to update status";

//         return RedirectToPage(new { projectId = ProjectId, id = ProductId });
//     }

//     public async Task<IActionResult> OnPostUpdateStoryPointsAsync(Guid itemId, int storyPoints)
//     {
//         SetPermissions();
//         if (!CanEditBacklog) return Forbid();

//         var request = new UpdateProductBacklogFieldRequest
//         {
//             ItemId = itemId,
//             Field = "storypoints",
//             Value = storyPoints.ToString()
//         };

//         var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

//         TempData[result != null ? "SuccessMessage" : "ErrorMessage"] =
//             result != null ? "Story points updated" : "Failed to update story points";

//         return RedirectToPage(new { projectId = ProjectId, id = ProductId });
//     }

//     public async Task<IActionResult> OnPostDeleteAsync(Guid itemId)
//     {
//         SetPermissions();
//         if (!CanEditBacklog) return Forbid();

//         var success = await _productBacklogService.DeleteItemAsync(itemId);

//         TempData[success ? "SuccessMessage" : "ErrorMessage"] =
//             success ? "Item deleted successfully" : "Failed to delete item";

//         return RedirectToPage(new { projectId = ProjectId, id = ProductId });
//     }

//     // Optional: Pure API-style handlers (you can use these from JS too)
//     public async Task<IActionResult> OnPostCreateItemAsync([FromBody] CreateProductBacklogItemRequest request)
//     {
//         if (!CanEditBacklog) return Forbid();

//         request.ProductId = ProductId;
//         var item = await _productBacklogService.CreateBacklogItemAsync(request);

//         return item != null
//             ? new JsonResult(new { success = true, item })
//             : BadRequest(new { success = false, message = "Creation failed" });
//     }

//     // Helper method
//     private string GetStatusName(int status) => status switch
//     {
//         1 => "Draft",
//         2 => "Approved",
//         3 => "In Progress",
//         4 => "Done",
//         _ => "To Do"
//     };
// }
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.Interfaces;
using PMTool.Application.Services.SubProject;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.DTOs.Sprint;
using PMTool.Application.DTOs.Board;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using PMTool.Web.Hubs;
using PMTool.Application.DTOs.User;
using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class BacklogModel : PageModel
{
    private const int CustomWorkTypeOffset = 1000;
    private readonly IProjectService _projectService;
    private readonly IProductService _productService;
    private readonly IProductBacklogService _productBacklogService;
    private readonly IUserAdminService _userService;
    private readonly ISubProjectService _subProjectService;
    private readonly ISprintService _sprintService;
    private readonly IWorkTypeService _workTypeService;
    private readonly IBoardColumnService _boardColumnService;
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationsHub> _notificationsHub;
    private readonly AppDbContext _db;

    public BacklogModel(
        IProjectService projectService,
        IProductService productService,
        IProductBacklogService productBacklogService,
        IUserAdminService userService,
        ISubProjectService subProjectService,
        ISprintService sprintService,
        IWorkTypeService workTypeService,
        IBoardColumnService boardColumnService,
        INotificationService notificationService,
        IHubContext<NotificationsHub> notificationsHub,
        AppDbContext db)
    {
        _projectService = projectService;
        _productService = productService;
        _productBacklogService = productBacklogService;
        _userService = userService;
        _subProjectService = subProjectService;
        _sprintService = sprintService;
        _workTypeService = workTypeService;
        _boardColumnService = boardColumnService;
        _notificationService = notificationService;
        _notificationsHub = notificationsHub;
        _db = db;
    }

    // ── Page Properties ───────────────────────────────────────────────────────
    [BindProperty(SupportsGet = true)]
    public Guid ProjectId { get; set; }

    [BindProperty(SupportsGet = true, Name = "id")]
    public Guid ProductId { get; set; }

    public string ProjectName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public bool IsEmbedded { get; set; }
    public bool CanEditBacklog { get; set; }

    public List<ProductBacklogItemDTO> BacklogItems { get; set; } = new();
    public List<BacklogItemTypeDTO> ItemTypes { get; set; } = new();
    public List<WorkTypeOptionDTO> WorkTypes { get; set; } = new();
    public List<UserDTO> ActiveUsers { get; set; } = new();
    public List<SubProjectDTO> ProductSubProjects { get; set; } = new();
    public List<SprintDTO> Sprints { get; set; } = new();
    public List<BoardColumnDTO> CustomBoardColumns { get; set; } = new();
    public Dictionary<Guid, List<BacklogSubtaskDto>> SubtasksByItem { get; set; } = new();

    // Status counts — 1=To do, 2=In progress, 3=In review, 4=Done
    public int TodoCount      => BacklogItems.Count(x => x.Status == 1);
    public int InProgressCount=> BacklogItems.Count(x => x.Status == 2);
    public int InReviewCount  => BacklogItems.Count(x => x.Status == 3);
    public int DoneCount      => BacklogItems.Count(x => x.Status == 4);

    // Legacy — kept for any existing references
    public int DraftCount    => TodoCount;
    public int ApprovedCount => InProgressCount;

    [BindProperty]
    public CreateProductBacklogItemRequest NewItem { get; set; } = new();

    // ── Permissions ───────────────────────────────────────────────────────────
    private void SetPermissions()
    {
        CanEditBacklog = User.IsInRole("Administrator") || User.IsInRole("Project Manager");
    }

    // ── GET ───────────────────────────────────────────────────────────────────
    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id, bool embedded = false)
    {
        ProjectId = projectId;
        ProductId = id;
        IsEmbedded = embedded;
        SetPermissions();

        var project = await _projectService.GetProjectByIdAsync(projectId);
        ProjectName = project?.Name ?? string.Empty;

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        ProductName = product.VersionName;

        BacklogItems = await _productBacklogService.GetBacklogItemsAsync(ProductId, null);
        SubtasksByItem = BacklogItems.ToDictionary(x => x.Id, x => x.Subtasks);
        ItemTypes = _productBacklogService.GetBacklogItemTypes();
        var customWorkTypes = await _workTypeService.GetCustomWorkTypesAsync();
        WorkTypes = BuildWorkTypes(customWorkTypes);
        ProductSubProjects = await _subProjectService.GetSubProjectsByProductAsync(ProductId);
        Sprints = await _sprintService.GetSprintsByProductAsync(ProductId);
        CustomBoardColumns = await _boardColumnService.GetColumnsByProductAsync(ProductId);

        var users = await _userService.GetActiveUsersAsync() ?? new List<UserDTO>();
        ActiveUsers = users.ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostCreateWorkTypeAsync([FromBody] CreateWorkTypeRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog)
        {
            return Forbid();
        }

        var created = await _workTypeService.CreateWorkTypeAsync(request);
        if (created == null)
        {
            return new JsonResult(new { success = false, message = "Failed to create work type." });
        }

        var option = new WorkTypeOptionDTO
        {
            Value = CustomWorkTypeOffset + created.Id,
            Name = created.Name,
            Description = created.Description,
            IconClass = created.IconClass,
            IsCustom = true
        };

        return new JsonResult(new { success = true, workType = option });
    }

    // ── CREATE (AJAX) ─────────────────────────────────────────────────────────
    public async Task<IActionResult> OnPostCreateAsync()
    {
        SetPermissions();
        if (!CanEditBacklog)
            return new JsonResult(new { success = false, message = "Forbidden" }) { StatusCode = 403 };

        if (string.IsNullOrWhiteSpace(NewItem.Title))
            return new JsonResult(new { success = false, message = "Title is required" }) { StatusCode = 400 };

        NewItem.ProductId = ProductId;

        var created = await _productBacklogService.CreateBacklogItemAsync(NewItem);
        if (created == null)
            return new JsonResult(new { success = false, message = "Failed to create item" }) { StatusCode = 400 };

        return new JsonResult(new
        {
            success = true,
            item = new
            {
                id         = created.Id,
                title      = created.Title,
                type       = created.Type,
                typeName   = created.TypeName ?? created.Type.ToString(),
                status     = created.Status,
                statusName = GetStatusLabel(created.Status),
                ownerName  = created.OwnerName ?? "Unassigned",
                storyPoints= created.StoryPoints,
                dueDate    = created.DueDate?.ToString("MMM d") ?? "",
                priority   = created.Priority
            }
        });
    }

    // ── UPDATE STATUS (AJAX-friendly) ─────────────────────────────────
    public async Task<IActionResult> OnPostUpdateStatusAsync(Guid itemId, int status)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        // Fetch item before update to know its sub-project
        var existing = await _productBacklogService.GetItemByIdAsync(itemId);

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "status",
            Value  = status.ToString()
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

        // Recalculate sub-project progress if item belongs to one
        if (result != null && existing?.SubProjectId.HasValue == true)
        {
            await _subProjectService.UpdateProgressAsync(existing.SubProjectId.Value);
        }

        // Return JSON if it looks like an AJAX call, otherwise redirect
        if (IsAjaxRequest())
            return new JsonResult(new { success = result != null });

        TempData[result != null ? "SuccessMessage" : "ErrorMessage"] =
            result != null ? "Status updated" : "Failed to update status";
        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
    }

    // ── UPDATE STORY POINTS (AJAX) ────────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateStoryPointsAsync(Guid itemId, int storyPoints)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "storypoints",
            Value  = storyPoints.ToString()
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

        if (IsAjaxRequest())
            return new JsonResult(new { success = result != null });

        TempData[result != null ? "SuccessMessage" : "ErrorMessage"] =
            result != null ? "Points updated" : "Failed to update points";
        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
    }

    // ── UPDATE DUE DATE (AJAX) ────────────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateDueDateAsync(Guid itemId, string dueDate)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "duedate",
            Value  = dueDate
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    // ── UPDATE OWNER (AJAX) ───────────────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateOwnerAsync(Guid itemId, string ownerId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var existing = await _productBacklogService.GetItemByIdAsync(itemId);
        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "owner",
            Value  = ownerId
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        if (result != null)
        {
            await NotifyAssignmentAsync(existing?.OwnerId, result);
        }
        return new JsonResult(new { success = result != null });
    }

    private async Task NotifyAssignmentAsync(Guid? previousOwnerId, ProductBacklogItemDTO updated)
    {
        if (!updated.OwnerId.HasValue || updated.OwnerId == previousOwnerId)
        {
            return;
        }

        var message = $"Assigned: {updated.Key} {updated.Title}".Trim();
        var notification = await _notificationService.CreateAsync(updated.OwnerId.Value, message, updated.Id);
        if (notification == null)
        {
            return;
        }

        await _notificationsHub.Clients.User(updated.OwnerId.Value.ToString())
            .SendAsync("notificationReceived", notification);
    }

    // ── UPDATE TYPE (AJAX) ────────────────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateTypeAsync(Guid itemId, int type)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "type",
            Value  = type.ToString()
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    // ── UPDATE TITLE (AJAX) ───────────────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateTitleAsync(Guid itemId, string title)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "title",
            Value  = title
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    public async Task<IActionResult> OnPostUpdateDescriptionAsync(Guid itemId, string description)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field = "description",
            Value = description ?? string.Empty
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    public async Task<IActionResult> OnPostUpdateStartDateAsync(Guid itemId, string startDate)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field = "startdate",
            Value = startDate ?? string.Empty
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
    }

    // ── UPDATE SUBPROJECT (AJAX) ──────────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateSubProjectAsync(Guid itemId, string subProjectId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var existing = await _productBacklogService.GetItemByIdAsync(itemId);
        var oldSubProjectId = existing?.SubProjectId;

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "subproject",
            Value  = subProjectId
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

        if (result != null)
        {
            if (oldSubProjectId.HasValue)
            {
                await _subProjectService.UpdateProgressAsync(oldSubProjectId.Value);
            }

            if (Guid.TryParse(subProjectId, out var newSubProjGuid) && newSubProjGuid != Guid.Empty)
            {
                await _subProjectService.UpdateProgressAsync(newSubProjGuid);
            }
        }

        return new JsonResult(new { success = result != null });
    }

    public async Task<IActionResult> OnPostSubtaskAsync([FromBody] CreateBacklogSubtaskDto request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        if (request.ParentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return BadJson("Subtask title is required.");
        }

        if (!await BacklogItemBelongsToRouteAsync(request.ParentId))
        {
            return BadJson("Invalid backlog item.");
        }

        var subtask = await _productBacklogService.CreateSubtaskAsync(request.ParentId, request);
        if (subtask == null)
        {
            return BadJson("Failed to create subtask.");
        }

        return new JsonResult(new
        {
            success = true,
            subtask = new
            {
                id = subtask.Id,
                title = subtask.Title,
                status = subtask.Status,
                priority = subtask.Priority,
                assigneeId = subtask.AssigneeId
            }
        });
    }

    public async Task<IActionResult> OnPostUpdateSubtaskStatusAsync([FromBody] UpdateSubtaskStatusRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        if (request.SubtaskId == Guid.Empty)
        {
            return BadJson("Invalid subtask.");
        }

        if (request.Status is < 1 or > 3)
        {
            return BadJson("Invalid status.");
        }

        if (!await SubtaskBelongsToRouteAsync(request.SubtaskId))
        {
            return BadJson("Invalid subtask.");
        }

        var success = await _productBacklogService.UpdateSubtaskStatusAsync(request.SubtaskId, request.Status);
        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostUpdateSubtaskAsync([FromBody] UpdateSubtaskRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        if (request.SubtaskId == Guid.Empty || string.IsNullOrWhiteSpace(request.Title))
        {
            return BadJson("Subtask title is required.");
        }

        if (request.Priority is < 1 or > 4 || request.Status is < 1 or > 3)
        {
            return BadJson("Invalid subtask values.");
        }

        if (!await SubtaskBelongsToRouteAsync(request.SubtaskId))
        {
            return BadJson("Invalid subtask.");
        }

        var dto = new CreateBacklogSubtaskDto
        {
            Title = request.Title,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            Status = request.Status
        };

        var success = await _productBacklogService.UpdateSubtaskAsync(request.SubtaskId, dto);
        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostDeleteSubtaskAsync(Guid subtaskId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        if (subtaskId == Guid.Empty || !await SubtaskBelongsToRouteAsync(subtaskId))
        {
            return BadJson("Invalid subtask.");
        }

        var success = await _productBacklogService.DeleteSubtaskAsync(subtaskId);
        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostAddCommentAsync([FromBody] AddCommentRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Forbid();

        if (request.ItemId == Guid.Empty || string.IsNullOrWhiteSpace(request.Body))
        {
            return BadJson("Comment body is required.");
        }

        if (!await BacklogItemBelongsToRouteAsync(request.ItemId))
        {
            return BadJson("Invalid backlog item.");
        }

        var comment = new BacklogItemComment
        {
            Id = Guid.NewGuid(),
            BacklogItemId = request.ItemId,
            AuthorId = userId.Value,
            Body = request.Body.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.BacklogItemComments.Add(comment);
        await _db.SaveChangesAsync();
        comment.Author = await _db.Users.FindAsync(comment.AuthorId);

        return new JsonResult(new
        {
            success = true,
            comment = MapComment(comment, userId.Value)
        });
    }

    public async Task<IActionResult> OnPostUpdateCommentAsync([FromBody] UpdateCommentRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Forbid();

        if (request.CommentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Body))
        {
            return BadJson("Comment body is required.");
        }

        var comment = await _db.BacklogItemComments
            .Include(c => c.BacklogItem)
            .FirstOrDefaultAsync(c => c.Id == request.CommentId);

        if (comment == null || comment.AuthorId != userId.Value || !BacklogItemMatchesRoute(comment.BacklogItem))
        {
            return BadJson("Invalid comment.");
        }

        comment.Body = request.Body.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostDeleteCommentAsync(Guid commentId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Forbid();

        var comment = await _db.BacklogItemComments
            .Include(c => c.BacklogItem)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null || comment.AuthorId != userId.Value || !BacklogItemMatchesRoute(comment.BacklogItem))
        {
            return BadJson("Invalid comment.");
        }

        _db.BacklogItemComments.Remove(comment);
        await _db.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnGetCommentsAsync(Guid itemId)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Forbid();

        if (itemId == Guid.Empty || !await BacklogItemBelongsToRouteAsync(itemId))
        {
            return BadJson("Invalid backlog item.");
        }

        var comments = await _db.BacklogItemComments
            .Include(c => c.Author)
            .Where(c => c.BacklogItemId == itemId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return new JsonResult(new
        {
            success = true,
            comments = comments.Select(c => MapComment(c, userId.Value))
        });
    }

    // ── DELETE ────────────────────────────────────────────────────────────────
    public async Task<IActionResult> OnPostDeleteAsync(Guid itemId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var existing = await _productBacklogService.GetItemByIdAsync(itemId);
        var subProjectId = existing?.SubProjectId;

        var success = await _productBacklogService.DeleteItemAsync(itemId);

        if (success && subProjectId.HasValue)
        {
            await _subProjectService.UpdateProgressAsync(subProjectId.Value);
        }

        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Item deleted" : "Failed to delete item";
        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
    }

    // ── SPRINT HANDLERS ───────────────────────────────────────────────────────
    public async Task<IActionResult> OnPostCreateSprintAsync([FromBody] CreateSprintRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        request.ProductId = ProductId;
        var sprint = await _sprintService.CreateSprintAsync(request);
        return new JsonResult(new { success = true, sprint });
    }

    public async Task<IActionResult> OnPostMoveToSprintAsync(Guid itemId, Guid? sprintId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Forbid();
        var userId = Guid.Parse(userIdStr);

        var success = await _sprintService.MoveToSprintAsync(itemId, sprintId, userId);
        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostDeleteSprintAsync(Guid sprintId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var success = await _sprintService.DeleteSprintAsync(sprintId);
        return new JsonResult(new { success });
    }

    public async Task<IActionResult> OnPostStartSprintAsync([FromBody] StartSprintRequest request)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var sprint = await _sprintService.StartSprintAsync(request.SprintId, request.StartDate, request.EndDate);
        if (sprint == null)
            return new JsonResult(new { success = false, message = "Sprint not found" });

        var boardUrl = Url.Page("/Products/Board",
            new { projectId = ProjectId, id = ProductId });

        return new JsonResult(new { success = true, boardUrl });
    }

    public async Task<IActionResult> OnPostCompleteSprintAsync(Guid sprintId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var success = await _sprintService.CompleteSprintAsync(sprintId);
        return new JsonResult(new { success });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private bool IsAjaxRequest() =>
        Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
        Request.Headers["Accept"].ToString().Contains("application/json");

    private static string GetStatusLabel(int status) => status switch
    {
        1 => "To do",
        2 => "In progress",
        3 => "In review",
        4 => "Done",
        _ => "To do"
    };

    private JsonResult BadJson(string message) =>
        new(new { success = false, message }) { StatusCode = 400 };

    private Guid? GetCurrentUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }

    private async Task<bool> BacklogItemBelongsToRouteAsync(Guid itemId)
    {
        var item = await _db.ProductBacklogs
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == itemId);

        return BacklogItemMatchesRoute(item);
    }

    private bool BacklogItemMatchesRoute(ProductBacklog? item)
    {
        if (item == null)
        {
            return false;
        }

        return item.ProductId == ProductId
            && (ProjectId == Guid.Empty || item.Product?.ProjectId == ProjectId);
    }

    private async Task<bool> SubtaskBelongsToRouteAsync(Guid subtaskId)
    {
        return await _db.BacklogSubtasks
            .Include(s => s.ProductBacklog)
            .ThenInclude(p => p!.Product)
            .AnyAsync(s => s.Id == subtaskId
                && s.ProductBacklog != null
                && s.ProductBacklog.ProductId == ProductId
                && (ProjectId == Guid.Empty || s.ProductBacklog.Product!.ProjectId == ProjectId));
    }

    private static object MapComment(BacklogItemComment comment, Guid currentUserId)
    {
        var authorName = GetUserName(comment.Author);
        return new
        {
            id = comment.Id,
            authorName,
            authorInitials = GetInitials(authorName),
            body = comment.Body,
            createdAt = comment.CreatedAt,
            isOwn = comment.AuthorId == currentUserId
        };
    }

    private static string GetUserName(User? user)
    {
        if (user == null)
        {
            return "Unknown user";
        }

        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return !string.IsNullOrWhiteSpace(user.DisplayName)
            ? user.DisplayName
            : string.IsNullOrWhiteSpace(fullName) ? user.Email : fullName;
    }

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant()
            : name[..Math.Min(2, name.Length)].ToUpperInvariant();
    }

    private static List<WorkTypeOptionDTO> BuildWorkTypes(IEnumerable<WorkTypeDTO> customTypes)
    {
        var workTypes = new List<WorkTypeOptionDTO>
        {
            new() { Value = 1, Name = "Story", Description = "User Story", IconClass = "bi bi-book", IsCustom = false },
            new() { Value = 2, Name = "Task", Description = "Task", IconClass = "bi bi-check2-square", IsCustom = false },
            new() { Value = 3, Name = "Bug", Description = "Bug", IconClass = "bi bi-bug-fill", IsCustom = false },
            new() { Value = 4, Name = "Epic", Description = "Epic", IconClass = "bi bi-stars", IsCustom = false }
        };

        workTypes.AddRange(customTypes.Select(type => new WorkTypeOptionDTO
        {
            Value = CustomWorkTypeOffset + type.Id,
            Name = type.Name,
            Description = type.Description,
            IconClass = string.IsNullOrWhiteSpace(type.IconClass) ? "bi bi-tag" : type.IconClass,
            IsCustom = true
        }));

        return workTypes;
    }

    public class UpdateSubtaskStatusRequest
    {
        public Guid SubtaskId { get; set; }
        public int Status { get; set; }
    }

    public class UpdateSubtaskRequest
    {
        public Guid SubtaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public Guid? AssigneeId { get; set; }
        public int Status { get; set; }
    }

    public class AddCommentRequest
    {
        public Guid ItemId { get; set; }
        public string Body { get; set; } = string.Empty;
    }

    public class UpdateCommentRequest
    {
        public Guid CommentId { get; set; }
        public string Body { get; set; } = string.Empty;
    }
}
