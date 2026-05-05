using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<Permission?> GetByTypeAsync(int permissionType)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionType == permissionType);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetActiveAsync()
    {
        return await _context.Permissions.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId)
    {
        return await _context.Roles
            .Where(r => r.Id == roleId)
            .SelectMany(r => r.Permissions)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Permission permission)
    {
        try
        {
            permission.Id = Guid.NewGuid();
            permission.CreatedAt = DateTime.UtcNow;
            permission.UpdatedAt = DateTime.UtcNow;
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Permission permission)
    {
        try
        {
            permission.UpdatedAt = DateTime.UtcNow;
            _context.Permissions.Update(permission);
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
            var permission = await GetByIdAsync(id);
            if (permission == null)
                return false;

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HasPermissionAsync(Guid userId, int permissionType)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Include(ur => ur.Role)
            .ThenInclude(r => r!.Permissions)
            .AnyAsync(ur => ur.Role!.Permissions.Any(p => p.PermissionType == permissionType && p.IsActive));
    }
}
