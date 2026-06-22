using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.Dashboard;
using PMTool.Application.Interfaces;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;

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

    public async Task<List<CollabProjectDto>> GetCollabDataAsync()
    {
        var projects = await _context.Projects
            .Where(p => !p.IsArchived)
            .OrderBy(p => p.ExpectedEndDate)
            .Take(4)
            .ToListAsync();

        if (!projects.Any())
            return new List<CollabProjectDto>();

        var projectIds = projects.Select(p => p.Id).ToList();

        var userRoles = await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => ur.ProjectId.HasValue && projectIds.Contains(ur.ProjectId.Value) && ur.IsActive)
            .ToListAsync();

        var userIds = userRoles.Select(ur => ur.UserId).Distinct().ToList();

        var teamMembers = await _context.TeamMembers
            .Include(tm => tm.Team)
            .Where(tm => userIds.Contains(tm.UserId))
            .ToListAsync();

        var productBacklogs = await _context.ProductBacklogs
            .Include(pb => pb.Product)
            .Where(pb => pb.OwnerId.HasValue
                         && userIds.Contains(pb.OwnerId.Value)
                         && pb.Product != null
                         && projectIds.Contains(pb.Product.ProjectId))
            .ToListAsync();

        var result = new List<CollabProjectDto>();

        foreach (var project in projects)
        {
            var projectUserRoles = userRoles
                .Where(ur => ur.ProjectId == project.Id)
                .GroupBy(ur => ur.UserId)
                .Select(g => g.First())
                .ToList();

            if (!projectUserRoles.Any())
                continue;

            var teamGroups = new Dictionary<Guid, CollabTeamDto>();

            foreach (var ur in projectUserRoles)
            {
                if (ur.User == null) continue;

                var membership = teamMembers.FirstOrDefault(tm => tm.UserId == ur.UserId);
                var teamKey = membership?.TeamId ?? Guid.Empty;
                var teamName = membership?.Team?.Name ?? "Project Team";
                var teamColor = membership?.Team?.ColorCode ?? "#6c757d";

                if (!teamGroups.ContainsKey(teamKey))
                {
                    teamGroups[teamKey] = new CollabTeamDto
                    {
                        Id = teamKey,
                        Name = teamName,
                        ColorCode = teamColor
                    };
                }

                var userTasks = productBacklogs
                    .Where(pb => pb.OwnerId == ur.UserId && pb.Product?.ProjectId == project.Id)
                    .ToList();

                var fn = ur.User.FirstName ?? "";
                var ln = ur.User.LastName ?? "";
                var initials = ((fn.Length > 0 ? fn[0].ToString() : "") +
                                (ln.Length > 0 ? ln[0].ToString() : "")).ToUpper();
                if (string.IsNullOrEmpty(initials) && ur.User.Email.Length >= 2)
                    initials = ur.User.Email.Substring(0, 2).ToUpper();

                teamGroups[teamKey].Members.Add(new CollabMemberDto
                {
                    Id = ur.UserId,
                    DisplayName = string.IsNullOrWhiteSpace(ur.User.DisplayName)
                        ? $"{fn} {ln}".Trim()
                        : ur.User.DisplayName,
                    Email = ur.User.Email,
                    RoleName = ur.Role?.Name ?? "Member",
                    Initials = initials,
                    TotalTasks = userTasks.Count,
                    CompletedTasks = userTasks.Count(t => t.Status == 4)
                });
            }

            result.Add(new CollabProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                ColourCode = string.IsNullOrEmpty(project.ColourCode) ? "#0D6EFD" : project.ColourCode,
                DueDate = project.ExpectedEndDate,
                Status = project.Status,
                Teams = teamGroups.Values.ToList()
            });
        }

        return result;
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

        // All active sprints where this user has assigned items
        var activeSprints = await _context.Sprints
            .Include(s => s.BacklogItems)
            .Where(s => s.Status == 2 && s.BacklogItems.Any(b => b.OwnerId == userGuid))
            .OrderBy(s => s.EndDate)
            .ToListAsync();

        var sprintDtos = activeSprints.Select(s => new PersonalSprintDto
        {
            Id = s.Id,
            Name = s.Name,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Goal = s.Goal,
            TotalItems = s.BacklogItems.Count,
            DoneItems = s.BacklogItems.Count(b => b.Status == 4),
            MyItems = s.BacklogItems.Count(b => b.OwnerId == userGuid)
        }).ToList();

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
            ActiveSprints = sprintDtos
        };
    }
}
