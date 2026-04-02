using PMTool.Application.DTOs.SubProject;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.SubProject;

public interface ISubProjectService
{
    Task<bool> CreateSubProjectAsync(CreateSubProjectRequest request);
    Task<bool> UpdateSubProjectAsync(Guid id, UpdateSubProjectRequest request);
    Task<bool> DeleteSubProjectAsync(Guid id);
    Task<SubProjectDTO?> GetSubProjectAsync(Guid id);
    Task<List<SubProjectDTO>> GetSubProjectsByProductAsync(Guid productId);
    Task<bool> AssignTeamAsync(Guid subProjectId, Guid teamId, string? role);
    Task<bool> RemoveTeamAsync(Guid subProjectId, Guid teamId);
    Task<bool> AddDependencyAsync(Guid subProjectId, Guid dependsOnSubProjectId, string? notes);
    Task<bool> RemoveDependencyAsync(Guid dependencyId);
    Task<int> UpdateProgressAsync(Guid subProjectId);
}

public class SubProjectService : ISubProjectService
{
    private readonly ISubProjectRepository _repository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;

    public SubProjectService(
        ISubProjectRepository repository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ITeamRepository teamRepository)
    {
        _repository = repository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
    }

    public async Task<bool> CreateSubProjectAsync(CreateSubProjectRequest request)
    {
        try
        {
            var subProject = new Domain.Entities.SubProject
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Name = request.Name,
                Description = request.Description,
                Status = 1, // NotStarted
                ModuleOwnerId = request.ModuleOwnerId,
                StartDate = request.StartDate,
                DueDate = request.DueDate,
                Progress = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (!await _repository.CreateSubProjectAsync(subProject))
                return false;

            // Assign teams
            if (request.TeamIds != null && request.TeamIds.Count > 0)
            {
                for (int i = 0; i < request.TeamIds.Count; i++)
                {
                    var role = i < request.TeamRoles.Count ? request.TeamRoles[i] : null;
                    await _repository.AssignTeamToSubProjectAsync(subProject.Id, request.TeamIds[i], role);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateSubProjectAsync(Guid id, UpdateSubProjectRequest request)
    {
        try
        {
            var subProject = await _repository.GetSubProjectByIdAsync(id);
            if (subProject == null) return false;

            subProject.Name = request.Name;
            subProject.Description = request.Description;
            subProject.Status = request.Status;
            subProject.ModuleOwnerId = request.ModuleOwnerId;
            subProject.StartDate = request.StartDate;
            subProject.DueDate = request.DueDate;
            subProject.UpdatedAt = DateTime.UtcNow;

            if (!await _repository.UpdateSubProjectAsync(subProject))
                return false;

            // Update team assignments
            var currentTeams = await _repository.GetSubProjectTeamsAsync(id);
            var currentTeamIds = currentTeams.Select(t => t.TeamId).ToList();

            // Remove teams no longer in the request
            foreach (var teamId in currentTeamIds.Where(t => !request.TeamIds.Contains(t)))
            {
                await _repository.RemoveTeamFromSubProjectAsync(id, teamId);
            }

            // Add new teams
            for (int i = 0; i < request.TeamIds.Count; i++)
            {
                if (!currentTeamIds.Contains(request.TeamIds[i]))
                {
                    var role = i < request.TeamRoles.Count ? request.TeamRoles[i] : null;
                    await _repository.AssignTeamToSubProjectAsync(id, request.TeamIds[i], role);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteSubProjectAsync(Guid id)
    {
        return await _repository.DeleteSubProjectAsync(id);
    }

    public async Task<SubProjectDTO?> GetSubProjectAsync(Guid id)
    {
        var subProject = await _repository.GetSubProjectByIdAsync(id);
        if (subProject == null) return null;

        return MapToDTO(subProject);
    }

    public async Task<List<SubProjectDTO>> GetSubProjectsByProductAsync(Guid productId)
    {
        var subProjects = await _repository.GetSubProjectsByProductWithDetailsAsync(productId);
        return subProjects.Select(MapToDTO).ToList();
    }

    public async Task<bool> AssignTeamAsync(Guid subProjectId, Guid teamId, string? role)
    {
        return await _repository.AssignTeamToSubProjectAsync(subProjectId, teamId, role);
    }

    public async Task<bool> RemoveTeamAsync(Guid subProjectId, Guid teamId)
    {
        return await _repository.RemoveTeamFromSubProjectAsync(subProjectId, teamId);
    }

    public async Task<bool> AddDependencyAsync(Guid subProjectId, Guid dependsOnSubProjectId, string? notes)
    {
        return await _repository.AddDependencyAsync(subProjectId, dependsOnSubProjectId, notes);
    }

    public async Task<bool> RemoveDependencyAsync(Guid dependencyId)
    {
        return await _repository.RemoveDependencyAsync(dependencyId);
    }

    public async Task<int> UpdateProgressAsync(Guid subProjectId)
    {
        var progress = await _repository.CalculateProgressAsync(subProjectId);
        await _repository.UpdateProgressAsync(subProjectId, progress);
        return progress;
    }

    private SubProjectDTO MapToDTO(Domain.Entities.SubProject subProject)
    {
        var teams = subProject.SubProjectTeams?.Select(spt => new SubProjectTeamDTO
        {
            TeamId = spt.TeamId,
            TeamName = spt.Team?.Name ?? string.Empty,
            Role = spt.Role,
            AssignedDate = spt.AssignedDate
        }).ToList() ?? new List<SubProjectTeamDTO>();

        var dependencies = subProject.DependsOn?.Select(sd => new SubProjectDependencyDTO
        {
            Id = sd.Id,
            DependsOnSubProjectId = sd.DependsOnSubProjectId,
            DependsOnSubProjectName = sd.DependsOnSubProject?.Name ?? string.Empty,
            Notes = sd.Notes
        }).ToList() ?? new List<SubProjectDependencyDTO>();

        var ticketCount = subProject.Backlog?.Count ?? 0;
        var completedTickets = subProject.Backlog?.Count(pb => pb.Status == 3) ?? 0;

        return new SubProjectDTO
        {
            Id = subProject.Id,
            ProductId = subProject.ProductId,
            Name = subProject.Name,
            Description = subProject.Description,
            Status = subProject.Status,
            ModuleOwnerName = subProject.ModuleOwner?.FirstName + " " + subProject.ModuleOwner?.LastName ?? string.Empty,
            StartDate = subProject.StartDate,
            DueDate = subProject.DueDate,
            Progress = subProject.Progress,
            TicketCount = ticketCount,
            CompletedTicketCount = completedTickets,
            Teams = teams,
            Dependencies = dependencies,
            CreatedAt = subProject.CreatedAt,
            UpdatedAt = subProject.UpdatedAt
        };
    }
}
