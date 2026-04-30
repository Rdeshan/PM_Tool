using PMTool.Application.DTOs;
using PMTool.Application.DTOs.Dashboard;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;

public class DashboardService : IDashboardService
{
    private readonly IProjectRepository _projectRepository;

    public DashboardService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(string userId)
    {
        var projects = await _projectRepository.GetAllAsync();

        var total = projects.Count();

        var completed = projects.Count(p => p.Status == (int)ProjectStatus.Completed);
        var active = projects.Count(p => p.Status == (int)ProjectStatus.Active);
        var onHold = projects.Count(p => p.Status == (int)ProjectStatus.OnHold);

        return new DashboardDto
        {
            TotalProjects = total,
            CompletedProjects = completed,
            RunningProjects = active,
            PendingProjects = onHold,
            UpcomingProjects = projects
                .Where(p => !p.IsArchived)
                .OrderBy(p => p.ExpectedEndDate)
                .Take(5)
                .Select(p => new DashboardProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    DueDate = p.ExpectedEndDate,
                    ColourCode = p.ColourCode
                })
                .ToList()
        };
    }
}
