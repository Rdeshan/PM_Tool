using PMTool.Application.DTOs.Project;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;

namespace PMTool.Application.Services.Project;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAuditService _auditService;

    public ProjectService(IProjectRepository projectRepository, IAuditService auditService)
    {
        _projectRepository = projectRepository;
        _auditService = auditService;
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
            Description = request.Description ?? string.Empty,
            ClientName = request.ClientName,
            ProjectCode = request.ProjectCode,
            StartDate = request.StartDate,
            ExpectedEndDate = request.ExpectedEndDate,
            ColourCode = request.ColourCode,
            CreatedByUserId = createdByUserId,
            Status = 1 // Active
        };

        var result = await _projectRepository.CreateAsync(project);
        if (result)
            await _auditService.LogAsync(createdByUserId, "Project.Created", "Project", project.Id.ToString(),
                newValue: new { project.Name, project.ProjectCode });

        return result;
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

        var old = new
        {
            name        = project.Name,
            description = project.Description,
            clientName  = project.ClientName,
            projectCode = project.ProjectCode,
            status      = project.Status,
            startDate   = project.StartDate,
            expectedEndDate = project.ExpectedEndDate,
            colourCode  = project.ColourCode
        };

        project.Name = request.Name;
        project.Description = request.Description ?? string.Empty;
        project.ClientName = request.ClientName;
        project.ProjectCode = request.ProjectCode;
        project.StartDate = request.StartDate;
        project.ExpectedEndDate = request.ExpectedEndDate;
        project.ColourCode = request.ColourCode;
        project.Status = request.Status;

        var result = await _projectRepository.UpdateAsync(project);
        if (result)
            await _auditService.LogAsync(Guid.Empty, "Project.Updated", "Project", id.ToString(),
                oldValue: old,
                newValue: new
                {
                    name        = project.Name,
                    description = project.Description,
                    clientName  = project.ClientName,
                    projectCode = project.ProjectCode,
                    status      = project.Status,
                    startDate   = project.StartDate,
                    expectedEndDate = project.ExpectedEndDate,
                    colourCode  = project.ColourCode
                });
        return result;
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            return false;

        var result = await _projectRepository.DeleteAsync(id);
        if (result)
            await _auditService.LogAsync(Guid.Empty, "Project.Deleted", "Project", id.ToString(),
                oldValue: new { project.Name, project.ProjectCode });

        return result;
    }

    public async Task<bool> ArchiveProjectAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            return false;

        var result = await _projectRepository.ArchiveAsync(id);
        if (result)
            await _auditService.LogAsync(Guid.Empty, "Project.Archived", "Project", id.ToString(),
                oldValue: new { project.Name, project.ProjectCode });

        return result;
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
        var result = await _projectRepository.AddTeamMemberAsync(projectId, userId, roleId);
        if (result)
            await _auditService.LogAsync(Guid.Empty, "Project.TeamMemberAdded", "Project", projectId.ToString(),
                newValue: new { userId, roleId });
        return result;
    }

    public async Task<bool> RemoveTeamMemberAsync(Guid projectId, Guid userId)
    {
        var result = await _projectRepository.RemoveTeamMemberAsync(projectId, userId);
        if (result)
            await _auditService.LogAsync(Guid.Empty, "Project.TeamMemberRemoved", "Project", projectId.ToString(),
                oldValue: new { userId });
        return result;
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
