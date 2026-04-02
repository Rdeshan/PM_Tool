using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PMTool.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await _context.Teams
            .Include(t => t.TeamMembers)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Team?> GetByNameAsync(string name)
    {
        return await _context.Teams
            .Include(t => t.TeamMembers)
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<IEnumerable<Team>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.TeamMembers)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Team>> GetActiveAsync()
    {
        return await _context.Teams
            .Where(t => t.IsActive)
            .Include(t => t.TeamMembers)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(Team team)
    {
        try
        {
            team.Id = Guid.NewGuid();
            team.CreatedAt = DateTime.UtcNow;
            team.UpdatedAt = DateTime.UtcNow;
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Team team)
    {
        try
        {
            team.UpdatedAt = DateTime.UtcNow;
            _context.Teams.Update(team);
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
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return false;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<User>> GetTeamMembersAsync(Guid teamId)
    {
        return await _context.TeamMembers
            .Where(tm => tm.TeamId == teamId)
            .Include(tm => tm.User)
            .Select(tm => tm.User!)
            .ToListAsync();
    }

    public async Task<bool> AddMemberAsync(Guid teamId, Guid userId)
    {
        try
        {
            var isMember = await IsMemberAsync(teamId, userId);
            if (isMember)
                return true;

            var teamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveMemberAsync(Guid teamId, Guid userId)
    {
        try
        {
            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (teamMember == null)
                return false;

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsMemberAsync(Guid teamId, Guid userId)
    {
        return await _context.TeamMembers
            .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
    }
}
