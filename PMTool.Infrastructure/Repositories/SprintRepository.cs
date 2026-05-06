using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class SprintRepository : ISprintRepository
{
    private readonly AppDbContext _context;

    public SprintRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sprint?> GetByIdAsync(Guid id)
    {
        return await _context.Sprints
            .Include(s => s.BacklogItems)
                .ThenInclude(b => b.Owner)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Sprint>> GetByProductIdAsync(Guid productId)
    {
        return await _context.Sprints
            .Where(s => s.ProductId == productId)
            .Include(s => s.BacklogItems)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<Sprint?> GetActiveByProductIdAsync(Guid productId)
    {
        return await _context.Sprints
            .Where(s => s.ProductId == productId && s.Status == 2)
            .Include(s => s.BacklogItems)
                .ThenInclude(b => b.Owner)
            .Include(s => s.BacklogItems)
                .ThenInclude(b => b.SubProject)
            .FirstOrDefaultAsync();
    }

    public async Task<Sprint> AddAsync(Sprint sprint)
    {
        sprint.CreatedAt = DateTime.UtcNow;
        sprint.UpdatedAt = DateTime.UtcNow;
        _context.Sprints.Add(sprint);
        await _context.SaveChangesAsync();
        return sprint;
    }

    public async Task UpdateAsync(Sprint sprint)
    {
        sprint.UpdatedAt = DateTime.UtcNow;
        _context.Sprints.Update(sprint);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var sprint = await _context.Sprints.FindAsync(id);
        if (sprint != null)
        {
            _context.Sprints.Remove(sprint);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddScopeChangeAsync(SprintScopeChange change)
    {
        change.ChangeDate = DateTime.UtcNow;
        _context.SprintScopeChanges.Add(change);
        await _context.SaveChangesAsync();
    }
}
