using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Product;
using PMTool.Application.Services.Product;
using System.Security.Claims;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IProductService _productService;

    public DetailsModel(IProductService productService)
    {
        _productService = productService;
    }

    public Guid ProjectId { get; set; }
    public ProductDTO? Product { get; set; }
    public List<ReleaseNotesDTO> ReleaseNotes { get; set; } = new();
    public bool CanEditProduct { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id)
    {
        ProjectId = projectId;
        CanEditProduct = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

        Product = await _productService.GetProductByIdAsync(id);
        if (Product == null)
            return NotFound();

        ReleaseNotes = (await _productService.GetReleaseNotesAsync(id)).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid projectId, Guid id)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        var result = await _productService.DeleteProductAsync(id);
        if (!result)
            return NotFound();

        return RedirectToPage("./Index", new { projectId });
    }

    public async Task<IActionResult> OnPostAddReleaseNotesAsync(Guid projectId, Guid id, string title, string content)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
        await _productService.AddReleaseNotesAsync(id, title, content, userId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostPublishReleaseNotesAsync(Guid projectId, Guid id, Guid releaseNotesId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _productService.PublishReleaseNotesAsync(releaseNotesId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public async Task<IActionResult> OnPostDeleteReleaseNotesAsync(Guid projectId, Guid id, Guid releaseNotesId)
    {
        if (!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
            return Forbid();

        await _productService.DeleteReleaseNotesAsync(releaseNotesId);

        return RedirectToPage("./Details", new { projectId, id });
    }

    public string GetStatusLabel(int status)
    {
        return status switch
        {
            1 => "Planned",
            2 => "In Development",
            3 => "In Testing",
            4 => "Released",
            5 => "Deprecated",
            _ => "Unknown"
        };
    }

    public string GetStatusBadgeClass(int status)
    {
        return status switch
        {
            1 => "bg-secondary",
            2 => "bg-info",
            3 => "bg-warning",
            4 => "bg-success",
            5 => "bg-danger",
            _ => "bg-secondary"
        };
    }

    public string GetReleaseTypeLabel(int releaseType)
    {
        return releaseType switch
        {
            1 => "Major",
            2 => "Minor",
            3 => "Patch",
            4 => "Hotfix",
            _ => "Unknown"
        };
    }
}
