using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.Services.SubProject;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Web.Pages.SubProjects;

[Authorize(Roles = "Administrator,Project Manager")]
public class CreateModel : PageModel
{
    private readonly ISubProjectService _subProjectService;
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        ISubProjectService subProjectService,
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IProductRepository productRepository,
        ILogger<CreateModel> logger)
    {
        _subProjectService = subProjectService;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    [BindProperty]
    public CreateSubProjectRequest Input { get; set; } = new();

    public List<SelectListItem> ModuleOwners { get; set; } = new();
    public List<SelectListItem> Teams { get; set; } = new();
    public Guid ProductId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid productId)
    {
        try
        {
            // Validate product exists
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToPage("/Products/Index");
            }

            ProductId = productId;
            Input.ProductId = productId;

            // Load module owners (all users)
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
            _logger.LogError(ex, "Error loading create sub-project page for product {ProductId}", productId);
            TempData["Error"] = "Failed to load the form";
            return RedirectToPage("Index", new { productId });
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid productId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(Input.ProductId);
                return Page();
            }

            // Validate product exists
            var product = await _productRepository.GetByIdAsync(Input.ProductId);
            if (product == null)
            {
                ModelState.AddModelError("", "Product not found");
                await OnGetAsync(Input.ProductId);
                return Page();
            }

            // Validate dates
            if (Input.StartDate.HasValue && Input.DueDate.HasValue && Input.StartDate > Input.DueDate)
            {
                ModelState.AddModelError("Input.DueDate", "Due date must be after start date");
                await OnGetAsync(Input.ProductId);
                return Page();
            }

            // Validate module owner exists
            var owner = await _userRepository.GetByIdAsync(Input.ModuleOwnerId);
            if (owner == null)
            {
                ModelState.AddModelError("Input.ModuleOwnerId", "Selected module owner does not exist");
                await OnGetAsync(Input.ProductId);
                return Page();
            }

            if (await _subProjectService.CreateSubProjectAsync(Input))
            {
                TempData["Success"] = "Sub-project created successfully";
                return RedirectToPage("Index", new { productId = Input.ProductId });
            }

            TempData["Error"] = "Failed to create sub-project";
            await OnGetAsync(Input.ProductId);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sub-project");
            TempData["Error"] = "An error occurred while creating the sub-project";
            await OnGetAsync(Input.ProductId);
            return Page();
        }
    }
}
