using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class DailyTaskRepository : IDailyTaskRepository
{
    private readonly AppDbContext _context;

    public DailyTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // BASIC CRUD
    // =========================

    public async Task<List<DailyTask>> GetAllAsync()
    {
        return await _context.DailyTasks
            .Include(x => x.User)
            .Include(x => x.ProductBacklog)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<DailyTask>> GetByUserIdAsync(Guid userId)
    {
        return await _context.DailyTasks
            .Where(x => x.UserId == userId)
            .Include(x => x.ProductBacklog)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<DailyTask?> GetByIdAsync(Guid id)
    {
        return await _context.DailyTasks
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<DailyTask?> AddAsync(DailyTask task)
    {
        try
        {
            _context.DailyTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }
        catch
        {
            return null;
        }
    }
                        
    public async Task<bool> UpdateAsync(DailyTask task)
    {
        try
        {
            _context.DailyTasks.Update(task);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var task = await GetByIdAsync(id);
            if (task == null) return false;

            _context.DailyTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // =========================
    // PM WORKFLOW LOGIC
    // =========================

    public async Task<List<DailyTask>> GetPendingTasksForPMAsync()
    {
        return await _context.DailyTasks
            .Where(x => x.Status == DailyTaskStatus.Pending)
            .Include(x => x.User)
            .Include(x => x.ProductBacklog)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<DailyTask>> GetTasksByStatusAsync(DailyTaskStatus status)
    {
        return await _context.DailyTasks
            .Where(x => x.Status == status)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<List<DailyTask>> GetAcceptedTasksForAdminAsync()
    {
        return await _context.DailyTasks
            .Where(x => x.Status == DailyTaskStatus.Accepted)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(Guid taskId, DailyTaskStatus status)
    {
        try
        {
            var task = await _context.DailyTasks.FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) return false;

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddPMCommentAsync(Guid taskId, string comment, Guid pmUserId)
    {
        try
        {
            var task = await _context.DailyTasks.FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) return false;

            task.PMComment = comment;
            task.ReviewedBy = pmUserId;
            task.ReviewedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AssignReviewerAsync(Guid taskId, Guid pmUserId)
    {
        try
        {
            var task = await _context.DailyTasks.FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null) return false;

            task.ReviewedBy = pmUserId;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}