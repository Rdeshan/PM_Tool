using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface ISubProjectRepository
{
    // CRUD operations
    Task<SubProject?> GetSubProjectByIdAsync(Guid id);
    Task<List<SubProject>> GetSubProjectsByProductAsync(Guid productId);
    Task<List<SubProject>> GetSubProjectsByProductWithDetailsAsync(Guid productId);
    Task<bool> CreateSubProjectAsync(SubProject subProject);
    Task<bool> UpdateSubProjectAsync(SubProject subProject);
    Task<bool> DeleteSubProjectAsync(Guid id);

    // Team management
    Task<bool> AssignTeamToSubProjectAsync(Guid subProjectId, Guid teamId, string? role);
    Task<bool> RemoveTeamFromSubProjectAsync(Guid subProjectId, Guid teamId);
    Task<List<SubProjectTeam>> GetSubProjectTeamsAsync(Guid subProjectId);

    // Dependency management
    Task<bool> AddDependencyAsync(Guid subProjectId, Guid dependsOnSubProjectId, string? notes);
    Task<bool> RemoveDependencyAsync(Guid dependencyId);
    Task<List<SubProjectDependency>> GetSubProjectDependenciesAsync(Guid subProjectId);
    Task<List<SubProjectDependency>> GetDependentSubProjectsAsync(Guid subProjectId);

    // Progress calculation
    Task<int> CalculateProgressAsync(Guid subProjectId);
    Task<bool> UpdateProgressAsync(Guid subProjectId, int progress);

    // Filtering and queries
    Task<List<SubProject>> GetSubProjectsByStatusAsync(Guid productId, int status);
    Task<bool> IsTeamAlreadyAssignedAsync(Guid subProjectId, Guid teamId);
    Task<bool> DependencyExistsAsync(Guid subProjectId, Guid dependsOnSubProjectId);
}
