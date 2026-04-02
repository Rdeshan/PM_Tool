using PMTool.Application.DTOs.Product;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Product;

public interface IProductService
{
    Task<ProductDTO?> GetProductByIdAsync(Guid id);
    Task<ProductDTO?> GetProductByVersionAsync(Guid projectId, string versionName);
    Task<IEnumerable<ProductDTO>> GetProductsByProjectAsync(Guid projectId);
    Task<IEnumerable<ProductDTO>> GetActiveProductsByProjectAsync(Guid projectId);
    Task<IEnumerable<ProductDTO>> GetReleasedProductsByProjectAsync(Guid projectId);
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<bool> CreateProductAsync(CreateProductRequest request);
    Task<bool> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(Guid id);
    Task<bool> VersionExistsInProjectAsync(Guid projectId, string versionName);
    Task<IEnumerable<ReleaseNotesDTO>> GetReleaseNotesAsync(Guid productId);
    Task<bool> AddReleaseNotesAsync(Guid productId, string title, string content, Guid createdByUserId);
    Task<bool> UpdateReleaseNotesAsync(Guid releaseNotesId, string title, string content);
    Task<bool> DeleteReleaseNotesAsync(Guid releaseNotesId);
    Task<bool> PublishReleaseNotesAsync(Guid releaseNotesId);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDTO?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDTO(product);
    }

    public async Task<ProductDTO?> GetProductByVersionAsync(Guid projectId, string versionName)
    {
        var product = await _productRepository.GetByProjectAndVersionAsync(projectId, versionName);
        return product == null ? null : MapToDTO(product);
    }

    public async Task<IEnumerable<ProductDTO>> GetProductsByProjectAsync(Guid projectId)
    {
        var products = await _productRepository.GetByProjectAsync(projectId);
        return products.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<ProductDTO>> GetActiveProductsByProjectAsync(Guid projectId)
    {
        var products = await _productRepository.GetActiveByProjectAsync(projectId);
        return products.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<ProductDTO>> GetReleasedProductsByProjectAsync(Guid projectId)
    {
        var products = await _productRepository.GetReleasedByProjectAsync(projectId);
        return products.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDTO).ToList();
    }

    public async Task<bool> CreateProductAsync(CreateProductRequest request)
    {
        // Check if version already exists in the project
        var versionExists = await _productRepository.VersionExistsInProjectAsync(
            request.ProjectId, request.VersionName);
        
        if (versionExists)
            return false;

        var product = new Domain.Entities.Product
        {
            ProjectId = request.ProjectId,
            VersionName = request.VersionName,
            Description = request.Description,
            PlannedReleaseDate = request.PlannedReleaseDate,
            Status = 1, // Planned
            ReleaseType = request.ReleaseType
        };

        return await _productRepository.CreateAsync(product);
    }

    public async Task<bool> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return false;

        // Check if version name is being changed and if new version already exists
        if (product.VersionName != request.VersionName)
        {
            var versionExists = await _productRepository.VersionExistsInProjectAsync(
                product.ProjectId, request.VersionName);
            
            if (versionExists)
                return false;
        }

        product.VersionName = request.VersionName;
        product.Description = request.Description;
        product.PlannedReleaseDate = request.PlannedReleaseDate;
        product.ActualReleaseDate = request.ActualReleaseDate;
        product.Status = request.Status;
        product.ReleaseType = request.ReleaseType;

        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    public async Task<bool> VersionExistsInProjectAsync(Guid projectId, string versionName)
    {
        return await _productRepository.VersionExistsInProjectAsync(projectId, versionName);
    }

    public async Task<IEnumerable<ReleaseNotesDTO>> GetReleaseNotesAsync(Guid productId)
    {
        var releaseNotes = await _productRepository.GetReleaseNotesAsync(productId);
        return releaseNotes.Select(MapReleaseNotesToDTO).ToList();
    }

    public async Task<bool> AddReleaseNotesAsync(Guid productId, string title, string content, Guid createdByUserId)
    {
        var releaseNotes = new ReleaseNotes
        {
            ProductId = productId,
            Title = title,
            Content = content,
            CreatedByUserId = createdByUserId,
            IsPublished = false
        };

        return await _productRepository.AddReleaseNotesAsync(releaseNotes);
    }

    public async Task<bool> UpdateReleaseNotesAsync(Guid releaseNotesId, string title, string content)
    {
        var releaseNotes = new ReleaseNotes
        {
            Id = releaseNotesId,
            Title = title,
            Content = content
        };

        return await _productRepository.UpdateReleaseNotesAsync(releaseNotes);
    }

    public async Task<bool> DeleteReleaseNotesAsync(Guid releaseNotesId)
    {
        return await _productRepository.DeleteReleaseNotesAsync(releaseNotesId);
    }

    public async Task<bool> PublishReleaseNotesAsync(Guid releaseNotesId)
    {
        return await _productRepository.PublishReleaseNotesAsync(releaseNotesId);
    }

    private ProductDTO MapToDTO(Domain.Entities.Product product)
    {
        return new ProductDTO
        {
            Id = product.Id,
            ProjectId = product.ProjectId,
            VersionName = product.VersionName,
            Description = product.Description,
            PlannedReleaseDate = product.PlannedReleaseDate,
            ActualReleaseDate = product.ActualReleaseDate,
            Status = product.Status,
            ReleaseType = product.ReleaseType,
            BacklogItemCount = product.Backlogs?.Count ?? 0,
            ReleaseNotesCount = product.ReleaseNotes?.Count ?? 0,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private ReleaseNotesDTO MapReleaseNotesToDTO(ReleaseNotes releaseNotes)
    {
        return new ReleaseNotesDTO
        {
            Id = releaseNotes.Id,
            ProductId = releaseNotes.ProductId,
            Title = releaseNotes.Title,
            Content = releaseNotes.Content,
            IsPublished = releaseNotes.IsPublished,
            PublishedDate = releaseNotes.PublishedDate,
            CreatedByUserId = releaseNotes.CreatedByUserId,
            CreatedAt = releaseNotes.CreatedAt,
            UpdatedAt = releaseNotes.UpdatedAt
        };
    }
}
