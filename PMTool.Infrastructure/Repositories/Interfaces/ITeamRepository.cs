using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<Team?> GetByNameAsync(string name);
    Task<IEnumerable<Team>> GetAllAsync();
    Task<IEnumerable<Team>> GetActiveAsync();
    Task<bool> CreateAsync(Team team);
    Task<bool> UpdateAsync(Team team);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<User>> GetTeamMembersAsync(Guid teamId);
    Task<bool> AddMemberAsync(Guid teamId, Guid userId);
    Task<bool> RemoveMemberAsync(Guid teamId, Guid userId);
    Task<bool> IsMemberAsync(Guid teamId, Guid userId);
}
