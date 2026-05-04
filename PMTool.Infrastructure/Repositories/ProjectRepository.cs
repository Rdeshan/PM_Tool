using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project?> GetByCodeAsync(string projectCode)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectCode == projectCode);
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetActiveAsync()
    {
        return await _context.Projects
            .Where(p => p.Status == 1 && !p.IsArchived)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByUserAsync(Guid userId)
    {
        return await _context.Projects
            .Where(p => p.TeamMembers.Any(tm => tm.UserId == userId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetArchivedAsync()
    {
        return await _context.Projects
            .Where(p => p.IsArchived)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Project project)
    {
        try
        {
            project.Id = Guid.NewGuid();
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Project project)
    {
        try
        {
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);
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
            var project = await GetByIdAsync(id);
            if (project == null || project.IsArchived)
                return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        try
        {
            var project = await GetByIdAsync(id);
            if (project == null)
                return false;

            project.IsArchived = true;
            project.UpdatedAt = DateTime.UtcNow;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ProjectCodeExistsAsync(string projectCode)
    {
        return await _context.Projects
            .AnyAsync(p => p.ProjectCode == projectCode);
    }

    public async Task<IEnumerable<User>> GetProjectTeamAsync(Guid projectId)
    {
        return await _context.UserRoles
            .Where(ur => ur.ProjectId == projectId)
            .Include(ur => ur.User)
            .Select(ur => ur.User!)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> AddTeamMemberAsync(Guid projectId, Guid userId, Guid roleId)
    {
        try
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                ProjectId = projectId,
                AssignedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveTeamMemberAsync(Guid projectId, Guid userId)
    {
        try
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.ProjectId == projectId && ur.UserId == userId)
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
