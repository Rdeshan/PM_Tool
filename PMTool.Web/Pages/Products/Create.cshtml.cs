using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Product;
using PMTool.Application.Services.Product;
using FluentValidation;

namespace PMTool.Web.Pages.Products;

[Authorize(Roles = "Administrator,Project Manager")]
public class CreateModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateModel(IProductService productService, IValidator<CreateProductRequest> validator)
    {
        _productService = productService;
        _validator = validator;
    }

    public Guid ProjectId { get; set; }

    [BindProperty]
    public CreateProductRequest Input { get; set; } = new();

    public void OnGet(Guid projectId)
    {
        ProjectId = projectId;
        Input.ProjectId = projectId;
    }

    public async Task<IActionResult> OnPostAsync(Guid projectId)
    {
        ProjectId = projectId;
        Input.ProjectId = projectId;
        var isEmbedded = string.Equals(Request.Query["embedded"], "true", StringComparison.OrdinalIgnoreCase);

        // Validate using FluentValidation
        var validationResult = await _validator.ValidateAsync(Input);
        if (!validationResult.IsValid)
        {
            foreach (var failure in validationResult.Errors)
            {
                ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
            return Page();
        }

        var result = await _productService.CreateProductAsync(Input);
        if (!result)
        {
            ModelState.AddModelError("", "Failed to create product. Version name may already exist in this project.");
            return Page();
        }

        if (isEmbedded)
        {
            var targetUrl = Url.Page("./Index", new { projectId, created = true }) ?? $"/Products/{projectId}?created=true";
            var html = $"<script>window.parent.location.href='{targetUrl}';</script>";
            return Content(html, "text/html");
        }

        return RedirectToPage("./Index", new { projectId });
    }
}
