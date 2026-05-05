using PMTool.Application.DTOs.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMTool.Application.Interfaces
{

    public interface IProjectService
    {
        Task<ProjectDTO?> GetProjectByIdAsync(Guid id);
        Task<ProjectDTO?> GetProjectByCodeAsync(string projectCode);
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
        Task<IEnumerable<ProjectDTO>> GetActiveProjectsAsync();
        Task<IEnumerable<ProjectDTO>> GetUserProjectsAsync(Guid userId);
        Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync();
        Task<bool> CreateProjectAsync(CreateProjectRequest request, Guid createdByUserId);
        Task<bool> UpdateProjectAsync(Guid id, UpdateProjectRequest request);
        Task<bool> DeleteProjectAsync(Guid id);
        Task<bool> ArchiveProjectAsync(Guid id);
        Task<bool> ProjectCodeExistsAsync(string projectCode);
        Task<IEnumerable<Domain.Entities.User>> GetProjectTeamAsync(Guid projectId);
        Task<bool> AddTeamMemberAsync(Guid projectId, Guid userId, Guid roleId);
        Task<bool> RemoveTeamMemberAsync(Guid projectId, Guid userId);
    }
}
