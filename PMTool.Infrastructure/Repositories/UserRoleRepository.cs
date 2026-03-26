using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AppDbContext _context;

    public UserRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserRole?> GetByIdAsync(Guid id)
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.Id == id);
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Include(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetByUserAndProjectAsync(Guid userId, Guid? projectId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.ProjectId == projectId && ur.IsActive)
            .Include(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Role>> GetUserProjectRolesAsync(Guid userId, Guid projectId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.ProjectId == projectId && ur.IsActive)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> AssignRoleAsync(UserRole userRole)
    {
        try
        {
            userRole.Id = Guid.NewGuid();
            userRole.AssignedAt = DateTime.UtcNow;
            userRole.UpdatedAt = DateTime.UtcNow;
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveRoleAsync(Guid userRoleId)
    {
        try
        {
            var userRole = await GetByIdAsync(userRoleId);
            if (userRole == null)
                return false;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(UserRole userRole)
    {
        try
        {
            userRole.UpdatedAt = DateTime.UtcNow;
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HasRoleAsync(Guid userId, int roleType, Guid? projectId = null)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.IsActive)
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.Role!.RoleType == roleType && ur.ProjectId == projectId);
    }
}
