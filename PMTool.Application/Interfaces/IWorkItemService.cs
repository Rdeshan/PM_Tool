using PMTool.Application.DTOs.WorkItems;

namespace PMTool.Application.Interfaces
{
    // NEW: Work item service contract
    public interface IWorkItemService
    {
        // NEW: Get all work items
        Task<List<WorkItemDto>> GetAllAsync();

        // NEW: Get work item by ID
        Task<WorkItemDto?> GetByIdAsync(int id);

        // NEW: Create work item
        Task CreateAsync(CreateWorkItemDto dto);

        // NEW: Update work item
        Task UpdateAsync(UpdateWorkItemDto dto);

        // NEW: Delete work item
        Task DeleteAsync(int id);
    }
}