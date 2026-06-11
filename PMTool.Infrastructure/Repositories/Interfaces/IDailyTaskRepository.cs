using PMTool.Domain.Entities;
using PMTool.Domain.Enums;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IDailyTaskRepository
{
    Task<List<DailyTask>> GetAllAsync();
    Task<List<DailyTask>> GetByUserIdAsync(Guid userId);
    Task<DailyTask?> GetByIdAsync(Guid id);
    Task<DailyTask?> AddAsync(DailyTask task);
    Task<bool> UpdateAsync(DailyTask task);
    Task<bool> DeleteAsync(Guid id);

    // =========================
    // PM WORKFLOW FUNCTIONS
    // =========================

    Task<List<DailyTask>> GetPendingTasksForPMAsync();
    Task<List<DailyTask>> GetTasksByStatusAsync(DailyTaskStatus status);
    Task<List<DailyTask>> GetAcceptedTasksForAdminAsync();
    Task<bool> UpdateStatusAsync(Guid taskId, DailyTaskStatus status);
    Task<bool> AddPMCommentAsync(Guid taskId, string comment, Guid pmUserId);
    Task<bool> AssignReviewerAsync(Guid taskId, Guid pmUserId);
}
