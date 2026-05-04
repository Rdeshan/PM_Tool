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

namespace PMTool.Web.Pages.Products;

[Authorize]
public class BacklogModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IProductService _productService;
    private readonly IProductBacklogService _productBacklogService;
    private readonly IUserAdminService _userService;

    public BacklogModel(
        IProjectService projectService,
        IProductService productService,
        IProductBacklogService productBacklogService,
        IUserAdminService userService)
    {
        _projectService = projectService;
        _productService = productService;
        _productBacklogService = productBacklogService;
        _userService = userService;
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
    public List<dynamic> ActiveUsers { get; set; } = new();

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
        ItemTypes = _productBacklogService.GetBacklogItemTypes();

        var users = await _userService.GetActiveUsersAsync();
        ActiveUsers = users.Select(u => (dynamic)new
        {
            id = u.Id.ToString(),
            displayName = u.DisplayName ?? $"{u.FirstName} {u.LastName}"
        }).ToList();

        return Page();
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

    // ── UPDATE STATUS (AJAX-friendly) ─────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateStatusAsync(Guid itemId, int status)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "status",
            Value  = status.ToString()
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);

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

        var request = new UpdateProductBacklogFieldRequest
        {
            ItemId = itemId,
            Field  = "owner",
            Value  = ownerId
        };

        var result = await _productBacklogService.UpdateBacklogFieldAsync(request);
        return new JsonResult(new { success = result != null });
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

    // ── DELETE ────────────────────────────────────────────────────────────────
    public async Task<IActionResult> OnPostDeleteAsync(Guid itemId)
    {
        SetPermissions();
        if (!CanEditBacklog) return Forbid();

        var success = await _productBacklogService.DeleteItemAsync(itemId);

        TempData[success ? "SuccessMessage" : "ErrorMessage"] =
            success ? "Item deleted" : "Failed to delete item";
        return RedirectToPage(new { projectId = ProjectId, id = ProductId });
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
}