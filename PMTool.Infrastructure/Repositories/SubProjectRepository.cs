using Microsoft.EntityFrameworkCore;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Infrastructure.Repositories;

public class SubProjectRepository : ISubProjectRepository
{
    private readonly AppDbContext _context;

    public SubProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SubProject?> GetSubProjectByIdAsync(Guid id)
    {
        return await _context.SubProjects
            .Include(sp => sp.Product)
            .Include(sp => sp.ModuleOwner)
            .Include(sp => sp.SubProjectTeams)
                .ThenInclude(spt => spt.Team)
            .Include(sp => sp.DependsOn)
                .ThenInclude(sd => sd.DependsOnSubProject)
            .FirstOrDefaultAsync(sp => sp.Id == id);
    }

    public async Task<List<SubProject>> GetSubProjectsByProductAsync(Guid productId)
    {
        return await _context.SubProjects
            .Where(sp => sp.ProductId == productId)
            .OrderByDescending(sp => sp.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<SubProject>> GetSubProjectsByProductWithDetailsAsync(Guid productId)
    {
        return await _context.SubProjects
            .Where(sp => sp.ProductId == productId)
            .Include(sp => sp.Product)
            .Include(sp => sp.ModuleOwner)
            .Include(sp => sp.SubProjectTeams)
                .ThenInclude(spt => spt.Team)
            .Include(sp => sp.DependsOn)
                .ThenInclude(sd => sd.DependsOnSubProject)
            .Include(sp => sp.Backlog)
            .OrderByDescending(sp => sp.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CreateSubProjectAsync(SubProject subProject)
    {
        try
        {
            subProject.CreatedAt = DateTime.UtcNow;
            subProject.UpdatedAt = DateTime.UtcNow;
            _context.SubProjects.Add(subProject);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateSubProjectAsync(SubProject subProject)
    {
        try
        {
            subProject.UpdatedAt = DateTime.UtcNow;
            _context.SubProjects.Update(subProject);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteSubProjectAsync(Guid id)
    {
        try
        {
            var subProject = await _context.SubProjects.FirstOrDefaultAsync(sp => sp.Id == id);
            if (subProject == null) return false;

            _context.SubProjects.Remove(subProject);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AssignTeamToSubProjectAsync(Guid subProjectId, Guid teamId, string? role)
    {
        try
        {
            // Check if already assigned
            var exists = await _context.SubProjectTeams
                .AnyAsync(spt => spt.SubProjectId == subProjectId && spt.TeamId == teamId);

            if (exists) return false;

            var assignment = new SubProjectTeam
            {
                Id = Guid.NewGuid(),
                SubProjectId = subProjectId,
                TeamId = teamId,
                Role = role,
                AssignedDate = DateTime.UtcNow
            };

            _context.SubProjectTeams.Add(assignment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveTeamFromSubProjectAsync(Guid subProjectId, Guid teamId)
    {
        try
        {
            var assignment = await _context.SubProjectTeams
                .FirstOrDefaultAsync(spt => spt.SubProjectId == subProjectId && spt.TeamId == teamId);

            if (assignment == null) return false;

            _context.SubProjectTeams.Remove(assignment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<SubProjectTeam>> GetSubProjectTeamsAsync(Guid subProjectId)
    {
        return await _context.SubProjectTeams
            .Where(spt => spt.SubProjectId == subProjectId)
            .Include(spt => spt.Team)
            .ToListAsync();
    }

    public async Task<bool> AddDependencyAsync(Guid subProjectId, Guid dependsOnSubProjectId, string? notes)
    {
        try
        {
            // Check if dependency already exists
            var exists = await _context.SubProjectDependencies
                .AnyAsync(sd => sd.SubProjectId == subProjectId && sd.DependsOnSubProjectId == dependsOnSubProjectId);

            if (exists) return false;

            // Check for circular dependencies
            if (await WouldCreateCircularDependencyAsync(subProjectId, dependsOnSubProjectId))
                return false;

            var dependency = new SubProjectDependency
            {
                Id = Guid.NewGuid(),
                SubProjectId = subProjectId,
                DependsOnSubProjectId = dependsOnSubProjectId,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.SubProjectDependencies.Add(dependency);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveDependencyAsync(Guid dependencyId)
    {
        try
        {
            var dependency = await _context.SubProjectDependencies.FirstOrDefaultAsync(sd => sd.Id == dependencyId);
            if (dependency == null) return false;

            _context.SubProjectDependencies.Remove(dependency);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<SubProjectDependency>> GetSubProjectDependenciesAsync(Guid subProjectId)
    {
        return await _context.SubProjectDependencies
            .Where(sd => sd.SubProjectId == subProjectId)
            .Include(sd => sd.DependsOnSubProject)
            .ToListAsync();
    }

    public async Task<List<SubProjectDependency>> GetDependentSubProjectsAsync(Guid subProjectId)
    {
        return await _context.SubProjectDependencies
            .Where(sd => sd.DependsOnSubProjectId == subProjectId)
            .Include(sd => sd.SubProject)
            .ToListAsync();
    }

    public async Task<int> CalculateProgressAsync(Guid subProjectId)
    {
        var tickets = await _context.ProjectBacklogs
            .Where(pb => pb.SubProjectId == subProjectId)
            .ToListAsync();

        if (tickets.Count == 0) return 0;

        var completedTickets = tickets.Count(pb => pb.Status == 3); // Assuming 3 = Completed
        return (completedTickets * 100) / tickets.Count;
    }

    public async Task<bool> UpdateProgressAsync(Guid subProjectId, int progress)
    {
        try
        {
            var subProject = await _context.SubProjects.FirstOrDefaultAsync(sp => sp.Id == subProjectId);
            if (subProject == null) return false;

            subProject.Progress = Math.Clamp(progress, 0, 100);
            subProject.UpdatedAt = DateTime.UtcNow;
            _context.SubProjects.Update(subProject);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<SubProject>> GetSubProjectsByStatusAsync(Guid productId, int status)
    {
        return await _context.SubProjects
            .Where(sp => sp.ProductId == productId && sp.Status == status)
            .OrderByDescending(sp => sp.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsTeamAlreadyAssignedAsync(Guid subProjectId, Guid teamId)
    {
        return await _context.SubProjectTeams
            .AnyAsync(spt => spt.SubProjectId == subProjectId && spt.TeamId == teamId);
    }

    public async Task<bool> DependencyExistsAsync(Guid subProjectId, Guid dependsOnSubProjectId)
    {
        return await _context.SubProjectDependencies
            .AnyAsync(sd => sd.SubProjectId == subProjectId && sd.DependsOnSubProjectId == dependsOnSubProjectId);
    }

    private async Task<bool> WouldCreateCircularDependencyAsync(Guid subProjectId, Guid dependsOnSubProjectId)
    {
        // Check if dependsOnSubProjectId already depends on subProjectId (which would create a cycle)
        var existingDependencies = await _context.SubProjectDependencies
            .Where(sd => sd.SubProjectId == dependsOnSubProjectId)
            .Select(sd => sd.DependsOnSubProjectId)
            .ToListAsync();

        if (existingDependencies.Contains(subProjectId))
            return true;

        // Recursively check for transitive cycles
        foreach (var depId in existingDependencies)
        {
            if (await WouldCreateCircularDependencyAsync(subProjectId, depId))
                return true;
        }

        return false;
    }
}
