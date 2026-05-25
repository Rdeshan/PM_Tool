using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class BacklogSubtaskRepository : IBacklogSubtaskRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<BacklogSubtaskRepository> _logger;

    public BacklogSubtaskRepository(AppDbContext context, ILogger<BacklogSubtaskRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BacklogSubtask?> GetByIdAsync(Guid id)
    {
        return await _context.BacklogSubtasks
            .Include(s => s.Assignee)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<BacklogSubtask>> GetByParentIdAsync(Guid parentId)
    {
        return await _context.BacklogSubtasks
            .Include(s => s.Assignee)
            .Where(s => s.ParentId == parentId)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(BacklogSubtask subtask)
    {
        try
        {
            _context.BacklogSubtasks.Add(subtask);
            var result = await _context.SaveChangesAsync();
            _logger?.LogInformation($"BacklogSubtask created: {subtask.Id}, SaveChangesAsync returned: {result}");
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error creating BacklogSubtask: {ex.Message}, InnerException: {ex.InnerException?.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(BacklogSubtask subtask)
    {
        try
        {
            _context.BacklogSubtasks.Update(subtask);
            var result = await _context.SaveChangesAsync();
            _logger?.LogInformation($"BacklogSubtask updated: {subtask.Id}, SaveChangesAsync returned: {result}");
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error updating BacklogSubtask {subtask.Id}: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var subtask = await _context.BacklogSubtasks.FindAsync(id);
            if (subtask == null)
            {
                _logger?.LogWarning($"BacklogSubtask not found for deletion: {id}");
                return false;
            }
            _context.BacklogSubtasks.Remove(subtask);
            var result = await _context.SaveChangesAsync();
            _logger?.LogInformation($"BacklogSubtask deleted: {id}, SaveChangesAsync returned: {result}");
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error deleting BacklogSubtask {id}: {ex.Message}");
            throw;
        }
    }
}