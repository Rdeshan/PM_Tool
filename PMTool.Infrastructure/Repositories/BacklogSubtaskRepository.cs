using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class BacklogSubtaskRepository : IBacklogSubtaskRepository
{
    private readonly AppDbContext _context;

    public BacklogSubtaskRepository(AppDbContext context)
    {
        _context = context;
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
        _context.BacklogSubtasks.Add(subtask);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(BacklogSubtask subtask)
    {
        _context.BacklogSubtasks.Update(subtask);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var subtask = await _context.BacklogSubtasks.FindAsync(id);
        if (subtask == null) return false;
        _context.BacklogSubtasks.Remove(subtask);
        return await _context.SaveChangesAsync() > 0;
    }
}