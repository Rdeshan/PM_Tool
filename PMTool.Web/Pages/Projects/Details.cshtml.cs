using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.Project;
using PMTool.Application.Services.Project;
using PMTool.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PMTool.Web.Pages.Projects;

[Authorize]
public class DetailsModel : PageModel
{
    private const long MaxUploadFileSize = 15 * 1024 * 1024;

    private readonly IProjectService _projectService;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProjectDTO? Project { get; set; }
    public IEnumerable<Domain.Entities.User> TeamMembers { get; set; } = new List<Domain.Entities.User>();
    public IEnumerable<ProjectDocumentViewModel> ProjectDocuments { get; set; } = new List<ProjectDocumentViewModel>();
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public UploadDocumentInput UploadDocument { get; set; } = new();

    public DetailsModel(IProjectService projectService, AppDbContext context, IWebHostEnvironment environment)
    {
        _projectService = projectService;
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var loaded = await LoadProjectDataAsync(id);
        if (!loaded)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostUploadDocumentAsync(Guid id)
    {
        var loaded = await LoadProjectDataAsync(id);
        if (!loaded)
            return NotFound();

        if (UploadDocument.File == null || UploadDocument.File.Length == 0)
        {
            ModelState.AddModelError("UploadDocument.File", "Please choose a file to upload.");
            return Page();
        }

        if (UploadDocument.File.Length > MaxUploadFileSize)
        {
            ModelState.AddModelError("UploadDocument.File", "File size cannot exceed 15 MB.");
            return Page();
        }

        if (!ModelState.IsValid)
            return Page();

        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            ErrorMessage = "Unable to identify the logged-in user.";
            return Page();
        }

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "project-documents", id.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(UploadDocument.File.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var absolutePath = Path.Combine(uploadsRoot, storedFileName);

        await using (var stream = new FileStream(absolutePath, FileMode.Create))
        {
            await UploadDocument.File.CopyToAsync(stream);
        }

        var document = new PMTool.Domain.Entities.ProjectDocument
        {
            Id = Guid.NewGuid(),
            ProjectId = id,
            DocumentName = UploadDocument.DocumentName.Trim(),
            OriginalFileName = UploadDocument.File.FileName,
            FilePath = $"/uploads/project-documents/{id}/{storedFileName}",
            ContentType = UploadDocument.File.ContentType ?? "application/octet-stream",
            FileSize = UploadDocument.File.Length,
            SubmittedByUserId = userId,
            SubmittedAt = DateTime.UtcNow
        };

        _context.ProjectDocuments.Add(document);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        // Only Admin and Project Manager can delete
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
        {
            return Forbid();
        }

        var result = await _projectService.DeleteProjectAsync(id);
        if (!result)
        {
            ErrorMessage = "Failed to delete project.";
            return RedirectToPage();
        }

        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnPostDeleteDocumentAsync(Guid id, Guid documentId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
        {
            return Forbid();
        }

        var document = await _context.ProjectDocuments
            .FirstOrDefaultAsync(d => d.Id == documentId && d.ProjectId == id);

        if (document == null)
        {
            return RedirectToPage(new { id });
        }

        if (!string.IsNullOrWhiteSpace(document.FilePath))
        {
            var relativePath = document.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var absolutePath = Path.Combine(_environment.WebRootPath, relativePath);

            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
            }
        }

        _context.ProjectDocuments.Remove(document);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    private async Task<bool> LoadProjectDataAsync(Guid id)
    {
        Project = await _projectService.GetProjectByIdAsync(id);
        if (Project == null)
            return false;

        TeamMembers = await _projectService.GetProjectTeamAsync(id);

        ProjectDocuments = await _context.ProjectDocuments
            .Where(d => d.ProjectId == id)
            .Include(d => d.SubmittedByUser)
            .OrderByDescending(d => d.SubmittedAt)
            .Select(d => new ProjectDocumentViewModel
            {
                Id = d.Id,
                DocumentName = d.DocumentName,
                OriginalFileName = d.OriginalFileName,
                FilePath = d.FilePath,
                SubmittedAt = d.SubmittedAt,
                SubmittedBy = $"{d.SubmittedByUser.FirstName} {d.SubmittedByUser.LastName}".Trim(),
                SubmittedByEmail = d.SubmittedByUser.Email
            })
            .ToListAsync();

        return true;
    }

    public class UploadDocumentInput
    {
        [Required]
        [StringLength(200)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        public IFormFile? File { get; set; }
    }

    public class ProjectDocumentViewModel
    {
        public Guid Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public string SubmittedBy { get; set; } = string.Empty;
        public string SubmittedByEmail { get; set; } = string.Empty;
    }
}
