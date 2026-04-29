using PMTool.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMTool.Application.Interfaces
{
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
}
