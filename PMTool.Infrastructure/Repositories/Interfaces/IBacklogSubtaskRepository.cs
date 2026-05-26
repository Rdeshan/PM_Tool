using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IBacklogSubtaskRepository
{
    Task<BacklogSubtask?> GetByIdAsync(Guid id);
    Task<List<BacklogSubtask>> GetByParentIdAsync(Guid parentId);
    Task<bool> CreateAsync(BacklogSubtask subtask);
    Task<bool> UpdateAsync(BacklogSubtask subtask);
    Task<bool> DeleteAsync(Guid id);
}