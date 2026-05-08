using PMTool.Application.DTOs.SubTasks;

namespace PMTool.Application.Interfaces
{
    // NEW: Subtask service contract
    public interface ISubTaskService
    {
        // NEW: Create subtask
        Task CreateAsync(CreateSubTaskDto dto);

        // NEW: Complete subtask
        Task ToggleCompleteAsync(int id);

        // NEW: Delete subtask
        Task DeleteAsync(int id);
    }
}