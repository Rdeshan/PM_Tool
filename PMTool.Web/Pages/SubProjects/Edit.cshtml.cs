using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.Services.SubProject;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.SubProjects;

[Authorize(Roles = "Administrator,Project Manager")]
public class EditModel : PageModel
{
    private readonly ISubProjectService _subProjectService;
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        ISubProjectService subProjectService,
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IProductRepository productRepository,
        ILogger<EditModel> logger)
    {
        _subProjectService = subProjectService;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    [BindProperty]
    public UpdateSubProjectRequest Input { get; set; } = new();

    public List<SelectListItem> Statuses { get; set; } = new();
    public List<SelectListItem> ModuleOwners { get; set; } = new();
    public List<SelectListItem> Teams { get; set; } = new();
    public List<SubProjectTeamDTO> CurrentTeams { get; set; } = new();
    public Guid ProductId { get; set; }
    public Guid SubProjectId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, Guid productId)
    {
        try
        {
            ProductId = productId;
            SubProjectId = id;
            var subProject = await _subProjectService.GetSubProjectAsync(id);
            
            if (subProject == null)
            {
                TempData["Error"] = "Sub-project not found";
                return RedirectToPage("Index", new { productId });
            }

            Input.Name = subProject.Name;
            Input.Description = subProject.Description;
            Input.Status = subProject.Status;
            Input.ModuleOwnerId = subProject.ModuleOwnerName == string.Empty ? Guid.Empty : (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => (u.FirstName + " " + u.LastName) == subProject.ModuleOwnerName)?.Id ?? Guid.Empty;
            Input.StartDate = subProject.StartDate;
            Input.DueDate = subProject.DueDate;
            Input.TeamIds = subProject.Teams.Select(t => t.TeamId).ToList();
            Input.TeamRoles = subProject.Teams.Select(t => t.Role ?? "").ToList();
            CurrentTeams = subProject.Teams;

            // Load statuses
            Statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Not Started" },
                new SelectListItem { Value = "2", Text = "In Progress" },
                new SelectListItem { Value = "3", Text = "In Review" },
                new SelectListItem { Value = "4", Text = "Completed" }
            };

            // Load module owners
            var users = await _userRepository.GetAllAsync();
            ModuleOwners = users
                .OrderBy(u => u.FirstName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.FirstName} {u.LastName} ({u.Email})"
                })
                .ToList();

            // Load teams
            var teams = await _teamRepository.GetAllAsync();
            Teams = teams
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
                .ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit sub-project page for {SubProjectId}", id);
            TempData["Error"] = "Failed to load the sub-project";
            return RedirectToPage("Index", new { productId });
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id, Guid productId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(id, productId);
                return Page();
            }

            // Validate dates
            if (Input.StartDate.HasValue && Input.DueDate.HasValue && Input.StartDate > Input.DueDate)
            {
                ModelState.AddModelError("Input.DueDate", "Due date must be after start date");
                await OnGetAsync(id, productId);
                return Page();
            }

            if (await _subProjectService.UpdateSubProjectAsync(id, Input))
            {
                TempData["Success"] = "Sub-project updated successfully";
                return RedirectToPage("Details", new { id, productId });
            }

            TempData["Error"] = "Failed to update sub-project";
            await OnGetAsync(id, productId);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sub-project {SubProjectId}", id);
            TempData["Error"] = "An error occurred while updating the sub-project";
            await OnGetAsync(id, productId);
            return Page();
        }
    }
}
