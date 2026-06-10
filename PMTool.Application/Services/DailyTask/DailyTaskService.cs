using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services;

public class DailyTaskService : IDailyTaskService
{
    private readonly IDailyTaskRepository _repository;

    public DailyTaskService(IDailyTaskRepository repository)
    {
        _repository = repository;
    }

    // =========================
    // BASIC USER OPERATIONS
    // =========================

    public Task<List<DailyTask>> GetUserTasksAsync(Guid userId)
        => _repository.GetByUserIdAsync(userId);

    public Task<DailyTask?> GetByIdAsync(Guid id)
        => _repository.GetByIdAsync(id);

    public Task<DailyTask?> CreateAsync(DailyTask task)
    {
        // default workflow state
        task.Status = DailyTaskStatus.Pending;
        task.CreatedAt = DateTime.UtcNow;

        return _repository.AddAsync(task);
    }

    public Task<bool> UpdateAsync(DailyTask task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        return _repository.UpdateAsync(task);
    }

    public Task<bool> DeleteAsync(Guid id)
        => _repository.DeleteAsync(id);

    // =========================
    // PM WORKFLOW OPERATIONS
    // =========================

    public Task<List<DailyTask>> GetPendingTasksForPMAsync()
        => _repository.GetPendingTasksForPMAsync();

    public async Task<bool> AcceptTaskAsync(Guid taskId, Guid pmUserId)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) return false;

        task.Status = DailyTaskStatus.Accepted;
        task.ReviewedBy = pmUserId;
        task.ReviewedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(task);
    }

    public async Task<bool> SendForClarificationAsync(
        Guid taskId,
        Guid pmUserId,
        string comment)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) return false;

        task.Status = DailyTaskStatus.NeedsClarification;
        task.PMComment = comment;
        task.ReviewedBy = pmUserId;
        task.ReviewedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(task);
    }

    // =========================
    // ADMIN OPERATIONS
    // =========================

    public Task<List<DailyTask>> GetAcceptedTasksForAdminAsync()
        => _repository.GetAcceptedTasksForAdminAsync();
}