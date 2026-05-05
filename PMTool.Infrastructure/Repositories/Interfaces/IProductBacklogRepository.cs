using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IProductBacklogRepository
{
    Task<ProductBacklog?> GetByIdAsync(Guid id);
    Task<List<ProductBacklog>> GetByFilterAsync(Guid productId, int? status);
    Task<int> GetNextPriorityAsync(Guid productId);
    Task<bool> CreateAsync(ProductBacklog item);
    Task<bool> UpdateAsync(ProductBacklog item);
    Task<bool> UpdateRangeAsync(IEnumerable<ProductBacklog> items);
    Task<bool> DeleteAsync(Guid id);
}
