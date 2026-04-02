using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class UserAdminRepository : IUserAdminRepository
{
    private readonly AppDbContext _context;

    public UserAdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.TeamMembers)
            .ThenInclude(tm => tm.Team)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.TeamMembers)
            .ThenInclude(tm => tm.Team)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.TeamMembers)
            .ThenInclude(tm => tm.Team)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetActiveAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.TeamMembers)
            .ThenInclude(tm => tm.Team)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetInactiveAsync()
    {
        return await _context.Users
            .Where(u => !u.IsActive)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.TeamMembers)
            .ThenInclude(tm => tm.Team)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(User user)
    {
        try
        {
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(User user)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeactivateAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            user.DeactivatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ReactivateAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = true;
            user.DeactivatedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<User>> GetUsersByTeamAsync(Guid teamId)
    {
        return await _context.TeamMembers
            .Where(tm => tm.TeamId == teamId)
            .Include(tm => tm.User)
            .ThenInclude(u => u!.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(tm => tm.User!)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(Guid roleId)
    {
        return await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Include(ur => ur.User)
            .Select(ur => ur.User)
            .OrderBy(u => u.Email)
            .ToListAsync();
    }
}
