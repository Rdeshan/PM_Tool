using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Product;
using PMTool.Application.Services.Product;
using FluentValidation;

namespace PMTool.Web.Pages.Products;

[Authorize(Roles = "Administrator,Project Manager")]
public class EditModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IValidator<UpdateProductRequest> _validator;

    public EditModel(IProductService productService, IValidator<UpdateProductRequest> validator)
    {
        _productService = productService;
        _validator = validator;
    }

    public Guid ProjectId { get; set; }
    public ProductDTO? Product { get; set; }

    [BindProperty]
    public UpdateProductRequest Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id)
    {
        ProjectId = projectId;
        Product = await _productService.GetProductByIdAsync(id);
        
        if (Product == null)
            return NotFound();

        // Populate input from product
        Input.VersionName = Product.VersionName;
        Input.Description = Product.Description;
        Input.PlannedReleaseDate = Product.PlannedReleaseDate;
        Input.ActualReleaseDate = Product.ActualReleaseDate;
        Input.Status = Product.Status;
        Input.ReleaseType = Product.ReleaseType;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid projectId, Guid id)
    {
        ProjectId = projectId;
        Product = await _productService.GetProductByIdAsync(id);
        
        if (Product == null)
            return NotFound();

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

        var result = await _productService.UpdateProductAsync(id, Input);
        if (!result)
        {
            ModelState.AddModelError("", "Failed to update product. Version name may already exist in this project.");
            return Page();
        }

        return RedirectToPage("./Details", new { projectId, id });
    }
}
