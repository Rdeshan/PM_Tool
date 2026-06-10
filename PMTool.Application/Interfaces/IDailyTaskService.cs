using PMTool.Domain.Entities;

namespace PMTool.Application.Interfaces;

public interface IDailyTaskService
{
    // =========================
    // BASIC USER OPERATIONS
    // =========================

    Task<List<DailyTask>> GetUserTasksAsync(Guid userId);
    Task<DailyTask?> GetByIdAsync(Guid id);

    Task<DailyTask?> CreateAsync(DailyTask task);
    Task<bool> UpdateAsync(DailyTask task);
    Task<bool> DeleteAsync(Guid id);

    // =========================
    // PM WORKFLOW OPERATIONS
    // =========================

    Task<List<DailyTask>> GetPendingTasksForPMAsync();

    Task<bool> AcceptTaskAsync(Guid taskId, Guid pmUserId);

    Task<bool> SendForClarificationAsync(
        Guid taskId,
        Guid pmUserId,
        string comment);

    // =========================
    // ADMIN OPERATIONS
    // =========================

    Task<List<DailyTask>> GetAcceptedTasksForAdminAsync();
}