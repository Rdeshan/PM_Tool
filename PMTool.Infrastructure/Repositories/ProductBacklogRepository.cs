using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using Microsoft.Extensions.Logging;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class ProductBacklogRepository : IProductBacklogRepository
{
    private readonly AppDbContext _context;
     private readonly ILogger<ProductBacklogRepository> _logger; 

    public ProductBacklogRepository(AppDbContext context ,  ILogger<ProductBacklogRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductBacklog?> GetByIdAsync(Guid id)
    {
        return await _context.ProductBacklogs
            .Include(x => x.Owner)
            .Include(x => x.SubProject)
            .Include(x => x.Subtasks).ThenInclude(s => s.Assignee)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ProductBacklog>> GetByFilterAsync(Guid productId, int? status)
    {
        var query = _context.ProductBacklogs
            .Include(x => x.Owner)
            .Include(x => x.SubProject)
            .Include(x => x.Subtasks).ThenInclude(s => s.Assignee)
            .Where(x => x.ProductId == productId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetNextPriorityAsync(Guid productId)
    {
        var max = await _context.ProductBacklogs
            .Where(x => x.ProductId == productId)
            .Select(x => (int?)x.Priority)
            .MaxAsync();

        return (max ?? 0) + 1;
    }

    public async Task<bool> CreateAsync(ProductBacklog item)
    {
        try
        {
            _context.ProductBacklogs.Add(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(ProductBacklog item)
    {
        try
        {
            _context.ProductBacklogs.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateRangeAsync(IEnumerable<ProductBacklog> items)
    {
        try
        {
            _context.ProductBacklogs.UpdateRange(items);
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
        var item = await _context.ProductBacklogs.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            _logger.LogWarning("DeleteAsync - Item not found. Id: {Id}", id);
            return false;
        }

        // 1. Delete SprintScopeChanges first (FK constraint)
        var scopeChanges = await _context.SprintScopeChanges
            .Where(x => x.BacklogItemId == id)
            .ToListAsync();
        if (scopeChanges.Any())
        {
            _context.SprintScopeChanges.RemoveRange(scopeChanges);
        }

        // 2. Delete Subtasks (if any)
        var subtasks = await _context.BacklogSubtasks
            .Where(x => x.ProductBacklogId == id)
            .ToListAsync();
        if (subtasks.Any())
        {
            _context.BacklogSubtasks.RemoveRange(subtasks);
        }

        // 3. Delete Comments (if any)
        var comments = await _context.BacklogItemComments
            .Where(x => x.BacklogItemId == id)
            .ToListAsync();
        if (comments.Any())
        {
            _context.BacklogItemComments.RemoveRange(comments);
        }

        // 4. Now delete the main item
        _context.ProductBacklogs.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "DeleteAsync - Exception deleting item. Id: {Id}", id);
        return false;
    }
}
}
