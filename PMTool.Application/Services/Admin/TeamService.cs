using PMTool.Application.DTOs.Team;
using PMTool.Application.Services.Team;
using TeamEntity = PMTool.Domain.Entities.Team;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Admin;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;

    public TeamService(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<TeamDTO?> GetTeamByIdAsync(Guid id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        return team == null ? null : MapToDTO(team);
    }

    public async Task<TeamDTO?> GetTeamByNameAsync(string name)
    {
        var team = await _teamRepository.GetByNameAsync(name);
        return team == null ? null : MapToDTO(team);
    }

    public async Task<IEnumerable<TeamDTO>> GetAllTeamsAsync()
    {
        var teams = await _teamRepository.GetAllAsync();
        return teams.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<TeamDTO>> GetActiveTeamsAsync()
    {
        var teams = await _teamRepository.GetActiveAsync();
        return teams.Select(MapToDTO).ToList();
    }

    public async Task<bool> CreateTeamAsync(CreateTeamRequest request)
    {
        var team = new TeamEntity
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        return await _teamRepository.CreateAsync(team);
    }

    public async Task<bool> UpdateTeamAsync(Guid id, CreateTeamRequest request)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null)
            return false;

        team.Name = request.Name;
        team.Description = request.Description;

        return await _teamRepository.UpdateAsync(team);
    }

    public async Task<bool> DeleteTeamAsync(Guid id)
    {
        return await _teamRepository.DeleteAsync(id);
    }

    public async Task<bool> AddMemberAsync(Guid teamId, Guid userId)
    {
        return await _teamRepository.AddMemberAsync(teamId, userId);
    }

    public async Task<bool> RemoveMemberAsync(Guid teamId, Guid userId)
    {
        return await _teamRepository.RemoveMemberAsync(teamId, userId);
    }

    private TeamDTO MapToDTO(TeamEntity team)
    {
        return new TeamDTO
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            IsActive = team.IsActive,
            MemberCount = team.TeamMembers?.Count ?? 0,
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt
        };
    }
}
