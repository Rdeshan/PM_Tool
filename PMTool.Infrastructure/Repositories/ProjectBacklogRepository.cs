using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class ProjectBacklogRepository : IProjectBacklogRepository
{
    private readonly AppDbContext _context;

    public ProjectBacklogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectBacklog?> GetByIdAsync(Guid id)
    {
        return await _context.ProjectBacklogs
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ProjectBacklog>> GetByFilterAsync(Guid projectId, Guid? productId, Guid? subProjectId, int? status)
    {
        var query = _context.ProjectBacklogs
            .Include(x => x.Owner)
            .Where(x => x.ProjectId == projectId);

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId);
        }

        if (subProjectId.HasValue)
        {
            query = query.Where(x => x.SubProjectId == subProjectId);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetNextPriorityAsync(Guid projectId, Guid? productId)
    {
        var max = await _context.ProjectBacklogs
            .Where(x => x.ProjectId == projectId && x.ProductId == productId)
            .Select(x => (int?)x.Priority)
            .MaxAsync();

        return (max ?? 0) + 1;
    }

    public async Task<bool> CreateAsync(ProjectBacklog item)
    {
        try
        {
            _context.ProjectBacklogs.Add(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(ProjectBacklog item)
    {
        try
        {
            _context.ProjectBacklogs.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateRangeAsync(IEnumerable<ProjectBacklog> items)
    {
        try
        {
            _context.ProjectBacklogs.UpdateRange(items);
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
            var item = await _context.ProjectBacklogs.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return false;
            }

            _context.ProjectBacklogs.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
