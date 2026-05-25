using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class BoardColumnRepository : IBoardColumnRepository
{
    private readonly AppDbContext _context;

    public BoardColumnRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BoardColumn>> GetByProductIdAsync(Guid productId)
    {
        return await _context.BoardColumns
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.StatusValue)
            .ToListAsync();
    }

    public async Task<BoardColumn?> GetByProductAndStatusAsync(Guid productId, int statusValue)
    {
        return await _context.BoardColumns
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.StatusValue == statusValue);
    }

    public async Task<BoardColumn?> AddAsync(BoardColumn column)
    {
        try
        {
            _context.BoardColumns.Add(column);
            await _context.SaveChangesAsync();
            return column;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(BoardColumn column)
    {
        try
        {
            _context.BoardColumns.Update(column);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid productId, int statusValue)
    {
        try
        {
            var column = await GetByProductAndStatusAsync(productId, statusValue);
            if (column == null)
            {
                return false;
            }

            _context.BoardColumns.Remove(column);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
