using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Product;
using PMTool.Application.Services.Product;

namespace PMTool.Web.Pages.Products;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IProductService _productService;

    public IndexModel(IProductService productService)
    {
        _productService = productService;
    }

    public Guid ProjectId { get; set; }
    public List<ProductDTO> Products { get; set; } = new();
    public string FilterStatus { get; set; } = "all";
    public string ErrorMessage { get; set; } = string.Empty;
    public bool CanCreateProduct { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid projectId, string status = "all")
    {
        ProjectId = projectId;
        FilterStatus = status;
        CanCreateProduct = User.IsInRole("Administrator") || User.IsInRole("Project Manager");

        try
        {
            var products = status switch
            {
                "active" => await _productService.GetActiveProductsByProjectAsync(projectId),
                "released" => await _productService.GetReleasedProductsByProjectAsync(projectId),
                _ => await _productService.GetProductsByProjectAsync(projectId)
            };

            Products = products.ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load products: " + ex.Message;
        }

        return Page();
    }
}
