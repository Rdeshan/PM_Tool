using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class ReviewBacklogModel : PageModel
{
    private readonly AppDbContext _db;

    public ReviewBacklogModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)] public Guid ProjectId { get; set; }
    [BindProperty(SupportsGet = true, Name = "id")] public Guid ProductId { get; set; }
    [BindProperty(SupportsGet = true)] public Guid DraftId { get; set; }

    public AiBacklogDraft? Draft { get; private set; }
    public bool CanManage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        CanManage = IsAuthorized();
        Draft = await LoadDraftAsync();
        if (Draft == null) return NotFound();
        return Page();
    }

    // ── Save edits to a single item ───────────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateItemAsync([FromBody] UpdateItemRequest req)
    {
        if (!IsAuthorized()) return Forbid();

        var item = await _db.AiBacklogDraftItems
            .FirstOrDefaultAsync(i => i.Id == req.ItemId && i.DraftId == DraftId);

        if (item == null) return NotFound();

        item.Title = req.Title.Trim();
        item.Description = req.Description.Trim();
        item.Type = req.Type;
        item.Priority = req.Priority;
        item.StoryPoints = req.StoryPoints;
        await _db.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    // ── Soft-delete a single item ─────────────────────────────────────────────
    public async Task<IActionResult> OnPostDeleteItemAsync(Guid itemId)
    {
        if (!IsAuthorized()) return Forbid();

        var item = await _db.AiBacklogDraftItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.DraftId == DraftId);

        if (item == null) return NotFound();

        item.IsDeleted = true;
        await _db.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    // ── Approve: copy surviving items into ProjectBacklog ────────────────────
    public async Task<IActionResult> OnPostApproveAsync()
    {
        if (!IsAuthorized()) return Forbid();

        var draft = await LoadDraftAsync();
        if (draft == null || draft.IsApproved) return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var now = DateTime.UtcNow;

        var surviving = draft.Items.Where(i => !i.IsDeleted).OrderBy(i => i.SortOrder).ToList();

        foreach (var item in surviving)
        {
            _db.ProjectBacklogs.Add(new ProjectBacklog
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                ProductId = ProductId,
                Title = item.Title,
                Description = item.Description,
                Type = item.Type,
                Priority = item.Priority,
                StoryPoints = item.StoryPoints,
                Status = 1, // Draft — PM can promote later
                OwnerId = userId,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        draft.IsApproved = true;
        draft.ApprovedAt = now;
        draft.ApprovedByUserId = userId;

        await _db.SaveChangesAsync();

        return RedirectToPage("/Products/Backlog",
            new { projectId = ProjectId, id = ProductId });
    }

    private async Task<AiBacklogDraft?> LoadDraftAsync() =>
        await _db.AiBacklogDrafts
            .Include(d => d.Items.Where(i => !i.IsDeleted).OrderBy(i => i.SortOrder))
            .Include(d => d.GeneratedBy)
            .FirstOrDefaultAsync(d => d.Id == DraftId && d.ProductId == ProductId);

    private bool IsAuthorized() =>
        User.IsInRole("Administrator") || User.IsInRole("Project Manager");

    public class UpdateItemRequest
    {
        public Guid ItemId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Type { get; set; }
        public int Priority { get; set; }
        public int StoryPoints { get; set; }
    }
}
