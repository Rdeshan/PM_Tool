using PMTool.Domain.Entities;

namespace PMTool.Infrastructure.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project?> GetByCodeAsync(string projectCode);
    Task<IEnumerable<Project>> GetAllAsync();
    Task<IEnumerable<Project>> GetActiveAsync();
    Task<IEnumerable<Project>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Project>> GetArchivedAsync();
    Task<bool> CreateAsync(Project project);
    Task<bool> UpdateAsync(Project project);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    Task<bool> ProjectCodeExistsAsync(string projectCode);
    Task<IEnumerable<User>> GetProjectTeamAsync(Guid projectId);
    Task<bool> AddTeamMemberAsync(Guid projectId, Guid userId, Guid roleId);
    Task<bool> RemoveTeamMemberAsync(Guid projectId, Guid userId);
}
