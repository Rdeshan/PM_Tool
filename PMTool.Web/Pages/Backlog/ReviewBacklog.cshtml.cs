using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Data;

namespace PMTool.Web.Pages.Backlog;

[Authorize]
public class ReviewBacklogModel : PageModel
{
    private readonly AppDbContext _db;

    public ReviewBacklogModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)] public Guid ProjectId { get; set; }
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

    // Approve: copy surviving items into ProjectBacklog with automatic hierarchy
    public async Task<IActionResult> OnPostApproveAsync()
    {
        if (!IsAuthorized()) return Forbid();

        var draft = await LoadDraftAsync();
        if (draft == null || draft.IsApproved) return NotFound();

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var now = DateTime.UtcNow;

        var surviving = draft.Items
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.SortOrder)
            .ToList();

        // Build hierarchy automatically: track current BRD/Epic/Story IDs as we insert in order
        Guid? currentBrdId = null;
        Guid? currentEpicId = null;
        Guid? currentStoryId = null;

        foreach (var item in surviving)
        {
            Guid? parentId = item.Type switch
            {
                (int)BacklogItemType.BRD => null,
                (int)BacklogItemType.Epic => currentBrdId,
                (int)BacklogItemType.UserStory => currentEpicId,
                (int)BacklogItemType.UseCase => currentStoryId,
                (int)BacklogItemType.ChangeRequest => currentStoryId,
                _ => null
            };

            var newItem = new ProjectBacklog
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                ProductId = null,
                ParentBacklogItemId = parentId,
                Title = item.Title,
                Description = item.Description,
                Type = item.Type,
                Priority = item.Priority,
                StoryPoints = item.StoryPoints,
                Status = (int)BacklogItemStatus.Draft,
                OwnerId = userId,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.ProjectBacklogs.Add(newItem);

            // Advance the context tracker
            switch (item.Type)
            {
                case (int)BacklogItemType.BRD:
                    currentBrdId = newItem.Id;
                    currentEpicId = null;
                    currentStoryId = null;
                    break;
                case (int)BacklogItemType.Epic:
                    currentEpicId = newItem.Id;
                    currentStoryId = null;
                    break;
                case (int)BacklogItemType.UserStory:
                    currentStoryId = newItem.Id;
                    break;
            }
        }

        draft.IsApproved = true;
        draft.ApprovedAt = now;
        draft.ApprovedByUserId = userId;

        await _db.SaveChangesAsync();

        return RedirectToPage("/Backlog/Index", new { projectId = ProjectId });
    }

    private async Task<AiBacklogDraft?> LoadDraftAsync() =>
        await _db.AiBacklogDrafts
            .Include(d => d.Items.Where(i => !i.IsDeleted).OrderBy(i => i.SortOrder))
            .Include(d => d.GeneratedBy)
            .FirstOrDefaultAsync(d => d.Id == DraftId && d.ProjectId == ProjectId && d.ProductId == null);

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
