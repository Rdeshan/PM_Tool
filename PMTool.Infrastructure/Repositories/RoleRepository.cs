using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Role?> GetByTypeAsync(int roleType)
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.RoleType == roleType);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Include(r => r.Permissions)
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetActiveAsync()
    {
        return await _context.Roles
            .Where(r => r.IsActive)
            .Include(r => r.Permissions)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Role role)
    {
        try
        {
            role.Id = Guid.NewGuid();
            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = DateTime.UtcNow;
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Role role)
    {
        try
        {
            role.UpdatedAt = DateTime.UtcNow;
            _context.Roles.Update(role);
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
            var role = await GetByIdAsync(id);
            if (role == null || role.IsSystemRole)
                return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<Permission>> GetPermissionsAsync(Guid roleId)
    {
        var role = await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId);

        return role?.Permissions ?? new List<Permission>();
    }
}
