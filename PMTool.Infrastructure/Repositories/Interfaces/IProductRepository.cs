using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetByProjectAndVersionAsync(Guid projectId, string versionName);
    Task<IEnumerable<Product>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<Product>> GetActiveByProjectAsync(Guid projectId);
    Task<IEnumerable<Product>> GetReleasedByProjectAsync(Guid projectId);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<bool> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> VersionExistsInProjectAsync(Guid projectId, string versionName);
    Task<IEnumerable<ReleaseNotes>> GetReleaseNotesAsync(Guid productId);
    Task<bool> AddReleaseNotesAsync(ReleaseNotes releaseNotes);
    Task<bool> UpdateReleaseNotesAsync(ReleaseNotes releaseNotes);
    Task<bool> DeleteReleaseNotesAsync(Guid releaseNotesId);
    Task<bool> PublishReleaseNotesAsync(Guid releaseNotesId);
}
