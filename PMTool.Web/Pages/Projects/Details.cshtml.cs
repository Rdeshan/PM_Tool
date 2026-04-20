using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.Project;
using PMTool.Application.DTOs.Team;
using PMTool.Application.Services.Project;
using PMTool.Application.Services.Team;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
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
    private readonly ITeamService _teamService;

    public ProjectDTO? Project { get; set; }
    public IEnumerable<Domain.Entities.User> TeamMembers { get; set; } = new List<Domain.Entities.User>();
    public Dictionary<Guid, List<TeamBadgeViewModel>> MemberTeams { get; set; } = new();
    public IEnumerable<ProjectDocumentViewModel> ProjectDocuments { get; set; } = new List<ProjectDocumentViewModel>();

    public IEnumerable<TeamDTO> Teams { get; set; } = new List<TeamDTO>();
    public string? ErrorMessage { get; set; }

    [BindProperty]
    public UploadDocumentInput UploadDocument { get; set; } = new();

    public DetailsModel(IProjectService projectService, AppDbContext context, IWebHostEnvironment environment , ITeamService teamService)
    {
        _projectService = projectService;
        _context = context;
        _environment = environment;
        _teamService = teamService;
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

    public async Task<IActionResult> OnPostRemoveTeamMemberAsync(Guid id, Guid userId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
        {
            return Forbid();
        }

        await _projectService.RemoveTeamMemberAsync(id, userId);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostAddTeamMembersAsync(Guid id, Guid teamId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
        {
            return Forbid();
        }

        var loaded = await LoadProjectDataAsync(id);
        if (!loaded)
        {
            return NotFound();
        }

        var teamUserIds = await _context.TeamMembers
            .Where(tm => tm.TeamId == teamId)
            .Select(tm => tm.UserId)
            .Distinct()
            .ToListAsync();

        if (!teamUserIds.Any())
        {
            ErrorMessage = "Selected team has no members.";
            return Page();
        }

        var existingProjectUserIds = await _context.UserRoles
            .Where(ur => ur.ProjectId == id && ur.IsActive && teamUserIds.Contains(ur.UserId))
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync();

        var orgRoleMap = await _context.UserRoles
            .Where(ur => ur.ProjectId == null && ur.IsActive && teamUserIds.Contains(ur.UserId))
            .GroupBy(ur => ur.UserId)
            .Select(g => new { UserId = g.Key, RoleId = g.OrderBy(x => x.AssignedAt).Select(x => x.RoleId).FirstOrDefault() })
            .ToDictionaryAsync(x => x.UserId, x => x.RoleId);

        var fallbackRoleId = await _context.Roles
            .Where(r => r.IsActive && r.RoleType == (int)RoleType.Developer)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (fallbackRoleId == Guid.Empty)
        {
            ErrorMessage = "No active fallback role is configured.";
            return Page();
        }

        var now = DateTime.UtcNow;
        var userRolesToAdd = new List<Domain.Entities.UserRole>();

        foreach (var userId in teamUserIds)
        {
            if (existingProjectUserIds.Contains(userId))
            {
                continue;
            }

            var roleId = orgRoleMap.TryGetValue(userId, out var mappedRoleId) && mappedRoleId != Guid.Empty
                ? mappedRoleId
                : fallbackRoleId;

            userRolesToAdd.Add(new Domain.Entities.UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                ProjectId = id,
                IsActive = true,
                AssignedAt = now,
                UpdatedAt = now
            });
        }

        if (userRolesToAdd.Count > 0)
        {
            _context.UserRoles.AddRange(userRolesToAdd);
            await _context.SaveChangesAsync();
        }

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
        Teams = await _teamService.GetAllTeamsAsync();

        var teamMemberUserIds = TeamMembers.Select(u => u.Id).Distinct().ToList();
        var teamMemberships = await _context.TeamMembers
            .Where(tm => teamMemberUserIds.Contains(tm.UserId))
            .Include(tm => tm.Team)
            .Select(tm => new
            {
                tm.UserId,
                TeamName = tm.Team != null ? tm.Team.Name : string.Empty,
                TeamColorCode = tm.Team != null ? tm.Team.ColorCode : "#6c757d"
            })
            .ToListAsync();

        MemberTeams = teamMemberships
            .GroupBy(x => x.UserId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new TeamBadgeViewModel
                {
                    TeamName = x.TeamName,
                    TeamColorCode = string.IsNullOrWhiteSpace(x.TeamColorCode) ? "#6c757d" : x.TeamColorCode
                }).ToList());

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

    public class TeamBadgeViewModel
    {
        public string TeamName { get; set; } = string.Empty;
        public string TeamColorCode { get; set; } = "#6c757d";
    }
}
