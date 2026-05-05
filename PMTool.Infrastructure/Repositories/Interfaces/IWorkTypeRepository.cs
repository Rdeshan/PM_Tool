using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IWorkTypeRepository
{
    Task<List<WorkType>> GetCustomAsync();
    Task<bool> ExistsByNameAsync(string name);
    Task<WorkType?> CreateAsync(WorkType workType);
}
