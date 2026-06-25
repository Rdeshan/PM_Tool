using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.Backlog;

[Authorize]
public class ManualEntryModel : PageModel
{
    private readonly IBacklogService _backlogService;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public ManualEntryModel(
        IBacklogService backlogService,
        IProjectRepository projectRepository,
        IUserRepository userRepository)
    {
        _backlogService = backlogService;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    [BindProperty(SupportsGet = true)] public Guid ProjectId { get; set; }

    public Project? Project { get; private set; }
    public List<User> Owners { get; private set; } = new();
    public string? SuccessMessage { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsAuthorized()) return RedirectToPage("/Auth/Login");
        if (ProjectId == Guid.Empty) return RedirectToPage("/Backlog/Index");

        Project = await _projectRepository.GetByIdAsync(ProjectId);
        if (Project == null) return NotFound();

        Owners = (await _userRepository.GetAllAsync()).OrderBy(u => u.FirstName).ToList();
        return Page();
    }

    private bool IsAuthorized() =>
        User.IsInRole("Administrator") || User.IsInRole("Project Manager");
}
