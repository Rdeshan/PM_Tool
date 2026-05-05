using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class WorkTypeRepository : IWorkTypeRepository
{
    private readonly AppDbContext _context;

    public WorkTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkType>> GetCustomAsync()
    {
        return await _context.WorkTypes
            .Where(x => !x.IsDefault)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.WorkTypes.AnyAsync(x => x.Name == name);
    }

    public async Task<WorkType?> CreateAsync(WorkType workType)
    {
        try
        {
            _context.WorkTypes.Add(workType);
            await _context.SaveChangesAsync();
            return workType;
        }
        catch
        {
            return null;
        }
    }
}
