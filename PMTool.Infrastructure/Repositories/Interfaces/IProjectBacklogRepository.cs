using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IProjectBacklogRepository
{
    Task<ProjectBacklog?> GetByIdAsync(Guid id);
    Task<List<ProjectBacklog>> GetByFilterAsync(Guid projectId, Guid? productId, Guid? subProjectId, int? status);
    Task<int> GetNextPriorityAsync(Guid projectId, Guid? productId);
    Task<bool> CreateAsync(ProjectBacklog item);
    Task<bool> UpdateAsync(ProjectBacklog item);
    Task<bool> UpdateRangeAsync(IEnumerable<ProjectBacklog> items);
    Task<bool> DeleteAsync(Guid id);
}
