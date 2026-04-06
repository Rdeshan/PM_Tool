using PMTool.Application.DTOs.Team;

namespace PMTool.Application.Services.Team;

public interface ITeamService
{
    Task<TeamDTO?> GetTeamByIdAsync(Guid id);
    Task<TeamDTO?> GetTeamByNameAsync(string name);
    Task<IEnumerable<TeamDTO>> GetAllTeamsAsync();
    Task<IEnumerable<TeamDTO>> GetActiveTeamsAsync();
    Task<bool> CreateTeamAsync(CreateTeamRequest request);
    Task<bool> UpdateTeamAsync(Guid id, CreateTeamRequest request);
    Task<bool> DeleteTeamAsync(Guid id);
    Task<bool> AddMemberAsync(Guid teamId, Guid userId);
    Task<bool> RemoveMemberAsync(Guid teamId, Guid userId);
}
