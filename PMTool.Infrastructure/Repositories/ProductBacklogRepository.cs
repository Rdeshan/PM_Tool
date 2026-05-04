using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class ProductBacklogRepository : IProductBacklogRepository
{
    private readonly AppDbContext _context;

    public ProductBacklogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProductBacklog?> GetByIdAsync(Guid id)
    {
        return await _context.ProductBacklogs
            .Include(x => x.Owner)
            .Include(x => x.SubProject)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ProductBacklog>> GetByFilterAsync(Guid productId, int? status)
    {
        var query = _context.ProductBacklogs
            .Include(x => x.Owner)
            .Include(x => x.SubProject)
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
                return false;
            }

            _context.ProductBacklogs.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
