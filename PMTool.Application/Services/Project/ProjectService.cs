using PMTool.Application.DTOs.Project;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;

namespace PMTool.Application.Services.Project;

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

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDTO?> GetProjectByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        return project == null ? null : MapToDTO(project);
    }

    public async Task<ProjectDTO?> GetProjectByCodeAsync(string projectCode)
    {
        var project = await _projectRepository.GetByCodeAsync(projectCode);
        return project == null ? null : MapToDTO(project);
    }

    public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<ProjectDTO>> GetActiveProjectsAsync()
    {
        var projects = await _projectRepository.GetActiveAsync();
        return projects.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<ProjectDTO>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await _projectRepository.GetByUserAsync(userId);
        return projects.Select(MapToDTO).ToList();
    }

    public async Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync()
    {
        var projects = await _projectRepository.GetArchivedAsync();
        return projects.Select(MapToDTO).ToList();
    }

    public async Task<bool> CreateProjectAsync(CreateProjectRequest request, Guid createdByUserId)
    {
        var projectCodeExists = await _projectRepository.ProjectCodeExistsAsync(request.ProjectCode);
        if (projectCodeExists)
            return false;

        var project = new Domain.Entities.Project
        {
            Name = request.Name,
            Description = request.Description,
            ClientName = request.ClientName,
            ProjectCode = request.ProjectCode,
            StartDate = request.StartDate,
            ExpectedEndDate = request.ExpectedEndDate,
            ColourCode = request.ColourCode,
            CreatedByUserId = createdByUserId,
            Status = 1 // Active
        };

        return await _projectRepository.CreateAsync(project);
    }

    public async Task<bool> UpdateProjectAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            return false;

        // Check if project code is being changed and if the new code already exists
        if (project.ProjectCode != request.ProjectCode)
        {
            var codeExists = await _projectRepository.ProjectCodeExistsAsync(request.ProjectCode);
            if (codeExists)
                return false;
        }

        project.Name = request.Name;
        project.Description = request.Description;
        project.ClientName = request.ClientName;
        project.ProjectCode = request.ProjectCode;
        project.StartDate = request.StartDate;
        project.ExpectedEndDate = request.ExpectedEndDate;
        project.ColourCode = request.ColourCode;
        project.Status = request.Status;

        return await _projectRepository.UpdateAsync(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        return await _projectRepository.DeleteAsync(id);
    }

    public async Task<bool> ArchiveProjectAsync(Guid id)
    {
        return await _projectRepository.ArchiveAsync(id);
    }

    public async Task<bool> ProjectCodeExistsAsync(string projectCode)
    {
        return await _projectRepository.ProjectCodeExistsAsync(projectCode);
    }

    public async Task<IEnumerable<Domain.Entities.User>> GetProjectTeamAsync(Guid projectId)
    {
        return await _projectRepository.GetProjectTeamAsync(projectId);
    }

    public async Task<bool> AddTeamMemberAsync(Guid projectId, Guid userId, Guid roleId)
    {
        return await _projectRepository.AddTeamMemberAsync(projectId, userId, roleId);
    }

    public async Task<bool> RemoveTeamMemberAsync(Guid projectId, Guid userId)
    {
        return await _projectRepository.RemoveTeamMemberAsync(projectId, userId);
    }

    private ProjectDTO MapToDTO(Domain.Entities.Project project)
    {
        return new ProjectDTO
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            ClientName = project.ClientName,
            ProjectCode = project.ProjectCode,
            Status = project.Status,
            AvatarUrl = project.AvatarUrl,
            ColourCode = project.ColourCode,
            IsArchived = project.IsArchived,
            StartDate = project.StartDate,
            ExpectedEndDate = project.ExpectedEndDate,
            CreatedByUserId = project.CreatedByUserId,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
