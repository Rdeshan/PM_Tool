using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.Dashboard;
using PMTool.Application.Interfaces;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;

public class DashboardService : IDashboardService
{
    private readonly IProjectRepository _projectRepository;
    private readonly AppDbContext _context;

    public DashboardService(IProjectRepository projectRepository, AppDbContext context)
    {
        _projectRepository = projectRepository;
        _context = context;
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

    public async Task<PersonalDashboardDto> GetPersonalDashboardAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return new PersonalDashboardDto();

        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);

        var myTickets = await _context.ProductBacklogs
            .Include(t => t.Product)
                .ThenInclude(p => p!.Project)
            .Where(t => t.OwnerId == userGuid)
            .OrderBy(t => t.Priority)
            .ToListAsync();

        var openCount = myTickets.Count(t => t.Status != 4);
        var overdueCount = myTickets.Count(t => t.DueDate.HasValue && t.DueDate.Value < now && t.Status != 4);
        var inProgressCount = myTickets.Count(t => t.Status == 2);
        var doneThisWeek = myTickets.Count(t => t.Status == 4 && t.UpdatedAt >= weekAgo);

        // Active sprint where this user has assigned items
        var activeSprint = await _context.Sprints
            .Include(s => s.BacklogItems)
            .Where(s => s.Status == 2 && s.BacklogItems.Any(b => b.OwnerId == userGuid))
            .FirstOrDefaultAsync();

        PersonalSprintDto? sprintDto = null;
        if (activeSprint != null)
        {
            sprintDto = new PersonalSprintDto
            {
                Id = activeSprint.Id,
                Name = activeSprint.Name,
                StartDate = activeSprint.StartDate,
                EndDate = activeSprint.EndDate,
                Goal = activeSprint.Goal,
                TotalItems = activeSprint.BacklogItems.Count,
                DoneItems = activeSprint.BacklogItems.Count(b => b.Status == 4),
                MyItems = activeSprint.BacklogItems.Count(b => b.OwnerId == userGuid)
            };
        }

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userGuid)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .Select(n => new PersonalNotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                ItemId = n.ItemId
            })
            .ToListAsync();

        // Use recently updated assigned tickets as activity proxy
        // TODO: replace with an audit log table when available
        var recentActivity = myTickets
            .OrderByDescending(t => t.UpdatedAt)
            .Take(6)
            .Select(t => new PersonalActivityDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                UpdatedAt = t.UpdatedAt,
                ProductName = t.Product?.Project?.Name ?? t.Product?.VersionName,
                ProductId = t.ProductId
            })
            .ToList();

        return new PersonalDashboardDto
        {
            OpenTickets = openCount,
            OverdueTickets = overdueCount,
            InProgressTickets = inProgressCount,
            DoneThisWeek = doneThisWeek,
            MyTickets = myTickets.Select(t => new PersonalTicketDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                ProductName = t.Product?.Project?.Name ?? t.Product?.VersionName,
                ProductId = t.ProductId,
                SprintId = t.SprintId,
                UpdatedAt = t.UpdatedAt
            }).ToList(),
            RecentActivity = recentActivity,
            ActiveSprint = sprintDto,
            Notifications = notifications
        };
    }
}
