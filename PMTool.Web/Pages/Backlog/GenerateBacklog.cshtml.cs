using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.Backlog;

[Authorize]
public class GenerateBacklogModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IGeminiService _gemini;
    private readonly IDocumentExtractorService _extractor;
    private readonly IProjectRepository _projectRepository;
    private readonly IWebHostEnvironment _env;

    public GenerateBacklogModel(
        AppDbContext db,
        IGeminiService gemini,
        IDocumentExtractorService extractor,
        IProjectRepository projectRepository,
        IWebHostEnvironment env)
    {
        _db = db;
        _gemini = gemini;
        _extractor = extractor;
        _projectRepository = projectRepository;
        _env = env;
    }

    [BindProperty(SupportsGet = true)] public Guid ProjectId { get; set; }

    public Project? Project { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsAuthorized()) return RedirectToPage("/Auth/Login");
        if (ProjectId == Guid.Empty) return RedirectToPage("/Backlog/Index");

        Project = await _projectRepository.GetByIdAsync(ProjectId);
        if (Project == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile document)
    {
        if (!IsAuthorized()) return Forbid();

        Project = await _projectRepository.GetByIdAsync(ProjectId);
        if (Project == null) return NotFound();

        if (document == null || document.Length == 0)
        {
            ErrorMessage = "Please select a file to upload.";
            return Page();
        }

        if (document.Length > 10 * 1024 * 1024)
        {
            ErrorMessage = "File is too large. Maximum size is 10 MB.";
            return Page();
        }

        string documentText;
        try
        {
            await using var stream = document.OpenReadStream();
            documentText = await _extractor.ExtractTextAsync(stream, document.FileName);
        }
        catch (NotSupportedException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }

        if (string.IsNullOrWhiteSpace(documentText) || documentText.Length < 100)
        {
            ErrorMessage = "Could not extract meaningful text from the document. Please check the file.";
            return Page();
        }

        List<AiBacklogDraftItem> items;
        try
        {
            items = await _gemini.GenerateBacklogItemsAsync(documentText);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"AI generation failed: {ex.Message}";
            return Page();
        }

        if (items.Count == 0)
        {
            ErrorMessage = "The AI could not extract any backlog items from this document. Try a more detailed BRD/SRS.";
            return Page();
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var draft = new AiBacklogDraft
        {
            Id = Guid.NewGuid(),
            ProductId = null,   // project-level — no version required
            ProjectId = ProjectId,
            SourceFileName = document.FileName,
            GeneratedAt = DateTime.UtcNow,
            GeneratedByUserId = userId,
            Items = items
        };

        foreach (var item in draft.Items)
            item.DraftId = draft.Id;

        _db.AiBacklogDrafts.Add(draft);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Backlog/ReviewBacklog",
            new { projectId = ProjectId, draftId = draft.Id });
    }

    // ── Generate from an existing project document (by ID) ───────────────────
    public async Task<IActionResult> OnPostFromDocumentAsync(Guid documentId, Guid projectId)
    {
        if (!IsAuthorized()) return Forbid();

        ProjectId = projectId;

        var doc = await _db.ProjectDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.ProjectId == projectId);

        if (doc == null) return NotFound();

        var relativePath = doc.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var subPath = relativePath.StartsWith("uploads" + Path.DirectorySeparatorChar)
            ? relativePath[("uploads" + Path.DirectorySeparatorChar).Length..]
            : relativePath;
        var fullPath = Path.Combine(_env.ContentRootPath, "upload-store", subPath);

        if (!System.IO.File.Exists(fullPath))
            return BadRequest("Document file not found on disk.");

        string documentText;
        try
        {
            await using var stream = System.IO.File.OpenRead(fullPath);
            documentText = await _extractor.ExtractTextAsync(stream, doc.OriginalFileName);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }

        if (string.IsNullOrWhiteSpace(documentText) || documentText.Length < 100)
            return BadRequest("Could not extract meaningful text from the document.");

        List<AiBacklogDraftItem> items;
        try
        {
            items = await _gemini.GenerateBacklogItemsAsync(documentText);
        }
        catch (Exception ex)
        {
            Project = await _projectRepository.GetByIdAsync(ProjectId);
            ErrorMessage = $"AI generation failed: {ex.Message}";
            return Page();
        }

        if (items.Count == 0)
        {
            Project = await _projectRepository.GetByIdAsync(ProjectId);
            ErrorMessage = "The AI could not extract any backlog items from this document. Try a more detailed BRD/SRS.";
            return Page();
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var draft = new AiBacklogDraft
        {
            Id = Guid.NewGuid(),
            ProductId = null,
            ProjectId = ProjectId,
            SourceFileName = doc.OriginalFileName,
            GeneratedAt = DateTime.UtcNow,
            GeneratedByUserId = userId,
            Items = items
        };

        foreach (var item in draft.Items)
            item.DraftId = draft.Id;

        _db.AiBacklogDrafts.Add(draft);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Backlog/ReviewBacklog",
            new { projectId = ProjectId, draftId = draft.Id });
    }

    private bool IsAuthorized() =>
        User.IsInRole("Administrator") || User.IsInRole("Project Manager");
}
