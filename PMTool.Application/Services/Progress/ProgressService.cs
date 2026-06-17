using Microsoft.EntityFrameworkCore;
using PMTool.Application.DTOs.Progress;
using PMTool.Application.Interfaces;
using PMTool.Domain.Entities;
using PMTool.Infrastructure.Data;

namespace PMTool.Application.Services.Progress;

public class ProgressService : IProgressService
{
    private readonly AppDbContext _context;

    private static readonly string[] AvatarPalette =
    {
        "#4F46E5", "#0891B2", "#059669", "#D97706", "#DC2626",
        "#7C3AED", "#DB2777", "#0284C7", "#65A30D", "#EA580C"
    };

    private static readonly Dictionary<int, string> TypeNames = new()
    {
        { 1, "BRD" }, { 2, "User Story" }, { 3, "Use Case" }, { 4, "Epic" },
        { 5, "Change Request" }, { 6, "Feature" }, { 7, "Improvement" },
        { 8, "Test Case" }, { 9, "Bug" }
    };

    private static readonly (int Status, string Name, string Color)[] StatusDefs =
    {
        (4, "Done",        "#198754"),
        (3, "In Progress", "#ffc107"),
        (2, "Approved",    "#0d6efd"),
        (1, "Draft",       "#6c757d"),
    };

    public ProgressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardProgressDto> GetDashboardProgressAsync()
    {
        var todayDate = DateTime.UtcNow.Date;

        var projects = await _context.Projects
            .Where(p => !p.IsArchived)
            .OrderBy(p => p.Name)
            .ToListAsync();

        if (!projects.Any())
            return new DashboardProgressDto();

        var projectIds = projects.Select(p => p.Id).ToList();

        var products = await _context.Products
            .Where(p => projectIds.Contains(p.ProjectId))
            .ToListAsync();

        var productIds = products.Select(p => p.Id).ToList();

        // Subtask completion counts per product via join
        var subtaskStats = await (
            from pb in _context.ProductBacklogs
            join s in _context.BacklogSubtasks on pb.Id equals s.ProductBacklogId
            where productIds.Contains(pb.ProductId)
            group s by pb.ProductId into g
            select new
            {
                ProductId = g.Key,
                Total = g.Count(),
                Done = g.Count(s => s.Status == 3)
            }
        ).ToListAsync();

        // Backlog item counts per product
        var backlogStats = await _context.ProductBacklogs
            .Where(pb => productIds.Contains(pb.ProductId))
            .GroupBy(pb => pb.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Total = g.Count(),
                Done = g.Count(x => x.Status == 4)
            })
            .ToListAsync();

        var activeSprints = await _context.Sprints
            .Where(s => productIds.Contains(s.ProductId) && s.Status == 2)
            .ToListAsync();

        var overdueCount = await _context.ProductBacklogs
            .Where(pb => productIds.Contains(pb.ProductId)
                         && pb.DueDate.HasValue
                         && pb.DueDate.Value < todayDate
                         && pb.Status != 4)
            .CountAsync();

        var blockedSprintCount = await _context.Sprints
            .Where(s => productIds.Contains(s.ProductId)
                        && s.Status == 2
                        && s.BacklogItems.Any(b => b.DueDate.HasValue && b.DueDate.Value < todayDate && b.Status != 4))
            .CountAsync();

        int ProductProgress(Guid productId)
        {
            var sub = subtaskStats.FirstOrDefault(s => s.ProductId == productId);
            if (sub != null && sub.Total > 0)
                return (int)Math.Round(100.0 * sub.Done / sub.Total, MidpointRounding.AwayFromZero);

            var bl = backlogStats.FirstOrDefault(b => b.ProductId == productId);
            if (bl != null && bl.Total > 0)
                return (int)Math.Round(100.0 * bl.Done / bl.Total, MidpointRounding.AwayFromZero);

            return 0;
        }

        var projectDtos = new List<ProjectProgressSummaryDto>();

        foreach (var project in projects)
        {
            var projectProducts = products.Where(p => p.ProjectId == project.Id).ToList();
            var productDtos = projectProducts.Select(product =>
            {
                var pct = ProductProgress(product.Id);
                var sprint = activeSprints.FirstOrDefault(s => s.ProductId == product.Id);
                var bl = backlogStats.FirstOrDefault(b => b.ProductId == product.Id);

                return new ProductProgressDto
                {
                    ProductId = product.Id,
                    ProductName = product.VersionName,
                    Version = product.VersionName,
                    ProgressPct = pct,
                    ActiveSprintName = sprint?.Name ?? "No active sprint",
                    SprintStatus = sprint != null ? "Active" : "None",
                    WorkItemCount = bl?.Total ?? 0,
                    DueDate = product.PlannedReleaseDate,
                    Description = product.Description
                };
            }).ToList();

            var overallPct = productDtos.Any()
                ? (int)Math.Round(productDtos.Average(p => (double)p.ProgressPct), MidpointRounding.AwayFromZero)
                : 0;

            projectDtos.Add(new ProjectProgressSummaryDto
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                ClientName = project.ClientName,
                Status = project.Status,
                ColourCode = project.ColourCode,
                OverallProgressPct = overallPct,
                Products = productDtos
            });
        }

        var avgCompletion = projectDtos.Any()
            ? (int)Math.Round(projectDtos.Average(p => (double)p.OverallProgressPct), MidpointRounding.AwayFromZero)
            : 0;

        return new DashboardProgressDto
        {
            TotalProjects = projects.Count,
            ProductCount = products.Count,
            AvgCompletionPct = avgCompletion,
            OverdueItemCount = overdueCount,
            BlockedSprintCount = blockedSprintCount,
            Projects = projectDtos
        };
    }

    public async Task<ProductAnalysisDto> GetProductAnalysisAsync(Guid productId)
    {
        var now = DateTime.UtcNow;
        var todayDate = now.Date;

        var product = await _context.Products
            .Include(p => p.Project)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null) return new ProductAnalysisDto();

        // Load all backlog items with their subtasks in one query
        var backlogItems = await _context.ProductBacklogs
            .Where(pb => pb.ProductId == productId)
            .Include(pb => pb.Subtasks)
            .ToListAsync();

        // Overall progress: subtask-weighted with fallback to item completion ratio
        var allSubtasks = backlogItems.SelectMany(pb => pb.Subtasks).ToList();
        int overallPct = CalcProgress(allSubtasks.Count,
                                      allSubtasks.Count(s => s.Status == 3),
                                      backlogItems.Count,
                                      backlogItems.Count(pb => pb.Status == 4));

        var totalItems = backlogItems.Count;
        var completedItems = backlogItems.Count(pb => pb.Status == 4);
        var inProgressCount = backlogItems.Count(pb => pb.Status == 3);
        var blockedItems = backlogItems.Count(pb =>
            pb.DueDate.HasValue && pb.DueDate.Value.Date < todayDate && pb.Status != 4);

        // Backlog by status (Done first for stacked bar readability)
        var backlogByStatus = StatusDefs
            .Select(def => new StatusCountDto
            {
                Name = def.Name,
                Count = backlogItems.Count(pb => pb.Status == def.Status),
                ColourHex = def.Color
            })
            .Where(s => s.Count > 0)
            .ToList();

        // Work items by type
        var workItemsByType = backlogItems
            .GroupBy(pb => pb.Type)
            .Select(g => new TypeCountDto
            {
                Name = TypeNames.TryGetValue(g.Key, out var n) ? n : $"Type {g.Key}",
                Count = g.Count()
            })
            .OrderByDescending(t => t.Count)
            .ToList();

        // Sprints for this product
        var allSprints = await _context.Sprints
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.StartDate)
            .ToListAsync();

        var activeSprint = allSprints.FirstOrDefault(s => s.Status == 2);
        int totalSprints = allSprints.Count;
        int sprintNumber = activeSprint != null ? allSprints.IndexOf(activeSprint) + 1 : 0;

        string sprintLabel = "No active sprint";
        SprintSummaryDto? sprintDto = null;
        ForecastDto? forecastDto = null;

        if (activeSprint != null)
        {
            var sprintItems = backlogItems.Where(b => b.SprintId == activeSprint.Id).ToList();
            int sprintTotal = sprintItems.Count;
            int sprintDone = sprintItems.Count(b => b.Status == 4);
            int sprintRemaining = sprintTotal - sprintDone;

            var startDate = activeSprint.StartDate.Date;
            var endDate = activeSprint.EndDate.Date;
            int sprintLen = Math.Max(1, (endDate - startDate).Days);
            int daysElapsed = Math.Max(1, (todayDate - startDate).Days);

            double velocity = sprintTotal > 0 ? (double)sprintDone / daysElapsed : 0;

            DateTime? projectedCompletion = null;
            int projectedOverrunDays = 0;
            bool isOverrun = false;

            if (sprintRemaining == 0)
            {
                projectedCompletion = todayDate;
                projectedOverrunDays = (todayDate - endDate).Days;
                isOverrun = projectedOverrunDays > 0;
            }
            else if (velocity > 0)
            {
                int daysToFinish = (int)Math.Ceiling(sprintRemaining / velocity);
                projectedCompletion = todayDate.AddDays(daysToFinish);
                projectedOverrunDays = (projectedCompletion.Value - endDate).Days;
                isOverrun = projectedOverrunDays > 0;
            }

            var burndownEnd = projectedCompletion.HasValue && projectedCompletion.Value > endDate
                ? projectedCompletion.Value : endDate;

            var burndownPoints = new List<BurndownPointDto>();
            for (var d = startDate; d <= burndownEnd; d = d.AddDays(1))
            {
                int i = (d - startDate).Days;

                double? ideal = i <= sprintLen
                    ? Math.Max(0, sprintTotal * (1.0 - (double)i / sprintLen))
                    : null;

                int? actual = d <= todayDate
                    ? Math.Max(0, (int)Math.Round(sprintTotal - velocity * i, MidpointRounding.AwayFromZero))
                    : null;

                double? forecast = d >= todayDate
                    ? Math.Max(0, sprintRemaining - velocity * (d - todayDate).Days)
                    : null;

                burndownPoints.Add(new BurndownPointDto { Date = d, Ideal = ideal, Actual = actual, Forecast = forecast });
            }

            int daysLeft = Math.Max(0, (endDate - todayDate).Days);
            sprintLabel = $"Sprint {sprintNumber} of {totalSprints} — Active";

            sprintDto = new SprintSummaryDto
            {
                SprintId = activeSprint.Id,
                Name = activeSprint.Name,
                SprintNumber = sprintNumber,
                TotalSprints = totalSprints,
                StartDate = activeSprint.StartDate,
                EndDate = activeSprint.EndDate,
                DaysLeft = daysLeft,
                TotalItems = sprintTotal,
                RemainingItems = sprintRemaining,
                BurndownPoints = burndownPoints
            };

            // Forecast risks (sub-project risks added after sub-project loop)
            var risks = new List<RiskItemDto>();

            var due48h = backlogItems.Count(pb =>
                pb.DueDate.HasValue &&
                pb.DueDate.Value > now &&
                pb.DueDate.Value <= now.AddHours(48) &&
                pb.Status != 4);

            if (due48h > 0)
                risks.Add(new RiskItemDto
                {
                    Title = $"{due48h} item{(due48h > 1 ? "s" : "")} due in 48h",
                    Detail = "Review and ensure these items can be completed on time",
                    Severity = RiskSeverity.Ok
                });

            if (isOverrun && projectedCompletion.HasValue)
                risks.Add(new RiskItemDto
                {
                    Title = "Sprint projected to overrun",
                    Detail = $"Projected completion {projectedCompletion.Value:MMM d} — {projectedOverrunDays} day{(projectedOverrunDays != 1 ? "s" : "")} late",
                    Severity = RiskSeverity.Warning
                });

            forecastDto = new ForecastDto
            {
                Velocity = Math.Round(velocity, 2),
                RemainingItems = sprintRemaining,
                TotalItems = sprintTotal,
                ProjectedCompletionDate = projectedCompletion,
                ProjectedOverrunDays = projectedOverrunDays,
                IsOverrun = isOverrun,
                Risks = risks
            };
        }

        // Sub-projects with progress and health
        var subProjects = await _context.SubProjects
            .Where(sp => sp.ProductId == productId)
            .ToListAsync();

        int elapsedPct = 0;
        if (activeSprint != null)
        {
            int sprintLen2 = Math.Max(1, (activeSprint.EndDate.Date - activeSprint.StartDate.Date).Days);
            int elapsed2 = Math.Max(1, (todayDate - activeSprint.StartDate.Date).Days);
            elapsedPct = (int)Math.Round(100.0 * elapsed2 / sprintLen2, MidpointRounding.AwayFromZero);
        }

        var subProjectDtos = new List<SubProjectProgressDto>();
        foreach (var sp in subProjects)
        {
            var spItems = backlogItems.Where(pb => pb.SubProjectId == sp.Id).ToList();
            var spSubtasks = spItems.SelectMany(pb => pb.Subtasks).ToList();

            int spPct = CalcProgress(spSubtasks.Count,
                                     spSubtasks.Count(s => s.Status == 3),
                                     spItems.Count,
                                     spItems.Count(pb => pb.Status == 4));

            bool hasOverdue = spItems.Any(pb =>
                pb.DueDate.HasValue && pb.DueDate.Value.Date < todayDate && pb.Status != 4);

            HealthStatus health;
            if (hasOverdue)
                health = HealthStatus.Blocked;
            else if (activeSprint != null && spPct < elapsedPct)
                health = HealthStatus.AtRisk;
            else
                health = HealthStatus.OnTrack;

            subProjectDtos.Add(new SubProjectProgressDto
            {
                SubProjectId = sp.Id,
                Name = sp.Name,
                ProgressPct = spPct,
                HealthStatus = health
            });

            // Prepend sub-project risks so they appear before generic sprint risks
            if (forecastDto != null)
            {
                if (health == HealthStatus.Blocked)
                {
                    var overdueCount2 = spItems.Count(pb =>
                        pb.DueDate.HasValue && pb.DueDate.Value.Date < todayDate && pb.Status != 4);
                    forecastDto.Risks.Insert(0, new RiskItemDto
                    {
                        Title = $"{sp.Name} is blocked",
                        Detail = $"{overdueCount2} overdue item{(overdueCount2 != 1 ? "s" : "")}",
                        Severity = RiskSeverity.Critical
                    });
                }
                else if (health == HealthStatus.AtRisk)
                {
                    forecastDto.Risks.Insert(0, new RiskItemDto
                    {
                        Title = $"{sp.Name} behind pace",
                        Detail = $"{spPct}% complete with {sprintDto!.DaysLeft} day{(sprintDto.DaysLeft != 1 ? "s" : "")} left",
                        Severity = RiskSeverity.Warning
                    });
                }
            }
        }

        // Team members: users assigned as OwnerId on any ProductBacklog item for this product
        var ownerIds = backlogItems
            .Where(pb => pb.OwnerId.HasValue)
            .Select(pb => pb.OwnerId!.Value)
            .Distinct()
            .ToList();

        var owners = ownerIds.Any()
            ? await _context.Users
                .Where(u => ownerIds.Contains(u.Id))
                .ToListAsync()
            : new List<User>();

        var ownerLookup = owners.ToDictionary(u => u.Id);

        var openByUser = backlogItems
            .Where(pb => pb.OwnerId.HasValue && pb.Status != 4)
            .GroupBy(pb => pb.OwnerId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var teamMemberDtos = owners.Select(u =>
        {
            var fn = u.FirstName;
            var ln = u.LastName;
            var initials = ((fn.Length > 0 ? fn[0].ToString() : "") +
                            (ln.Length > 0 ? ln[0].ToString() : "")).ToUpper();
            var colorHex = AvatarPalette[(uint)u.Id.GetHashCode() % AvatarPalette.Length];
            return new TeamMemberProgressDto
            {
                UserId = u.Id,
                FullName = $"{fn} {ln}".Trim(),
                Initials = string.IsNullOrEmpty(initials) ? "?" : initials,
                Role = "Assignee",
                OpenTaskCount = openByUser.TryGetValue(u.Id, out var cnt) ? cnt : 0,
                AvatarColorHex = colorHex
            };
        }).ToList();

        // Task progress: subtask rollup + in-progress item list with assignee and due date
        int totalSubtasks = allSubtasks.Count;
        int completedSubtasks = allSubtasks.Count(s => s.Status == 3);

        var inProgressItems = backlogItems
            .Where(pb => pb.Status == 3)
            .Select(pb =>
            {
                var subs = pb.Subtasks.ToList();
                int pct = subs.Any()
                    ? (int)Math.Round(100.0 * subs.Count(s => s.Status == 3) / subs.Count, MidpointRounding.AwayFromZero)
                    : 50;

                string assigneeName = "";
                string assigneeInitials = "?";
                string assigneeColor = "#6c757d";
                if (pb.OwnerId.HasValue && ownerLookup.TryGetValue(pb.OwnerId.Value, out var owner))
                {
                    var fn2 = owner.FirstName;
                    var ln2 = owner.LastName;
                    assigneeName = $"{fn2} {ln2}".Trim();
                    assigneeInitials = ((fn2.Length > 0 ? fn2[0].ToString() : "") +
                                       (ln2.Length > 0 ? ln2[0].ToString() : "")).ToUpper();
                    if (string.IsNullOrEmpty(assigneeInitials)) assigneeInitials = "?";
                    assigneeColor = AvatarPalette[(uint)pb.OwnerId.Value.GetHashCode() % AvatarPalette.Length];
                }

                return new WorkItemProgressDto
                {
                    WorkItemId = pb.Id,
                    Title = pb.Title,
                    WorkType = TypeNames.TryGetValue(pb.Type, out var tn) ? tn : $"Type {pb.Type}",
                    StatusName = "In Progress",
                    ProgressPct = pct,
                    SubTaskCount = subs.Count,
                    CompletedSubTaskCount = subs.Count(s => s.Status == 3),
                    DueDate = pb.DueDate,
                    AssigneeName = assigneeName,
                    AssigneeInitials = assigneeInitials,
                    AssigneeColorHex = assigneeColor
                };
            })
            .OrderByDescending(w => w.ProgressPct)
            .ToList();

        return new ProductAnalysisDto
        {
            ProductId = product.Id,
            ProductName = product.VersionName,
            Version = product.VersionName,
            ProjectName = product.Project?.Name ?? "",
            ClientName = product.Project?.ClientName ?? "",
            ProductStatus = product.Status,
            OverallProgressPct = overallPct,
            TotalWorkItems = totalItems,
            CompletedWorkItems = completedItems,
            InProgressWorkItems = inProgressCount,
            BlockedWorkItems = blockedItems,
            SprintLabel = sprintLabel,
            ActiveSprint = sprintDto,
            BacklogByStatus = backlogByStatus,
            WorkItemsByType = workItemsByType,
            SubProjects = subProjectDtos,
            TeamMembers = teamMemberDtos,
            TaskProgress = new TaskProgressSummaryDto
            {
                TotalSubTasks = totalSubtasks,
                CompletedSubTasks = completedSubtasks,
                InProgressItems = inProgressItems
            },
            Forecast = forecastDto
        };
    }

    // Subtask-weighted progress with fallback to item completion ratio
    private static int CalcProgress(int subtaskTotal, int subtaskDone, int itemTotal, int itemDone)
    {
        if (subtaskTotal > 0)
            return (int)Math.Round(100.0 * subtaskDone / subtaskTotal, MidpointRounding.AwayFromZero);
        if (itemTotal > 0)
            return (int)Math.Round(100.0 * itemDone / itemTotal, MidpointRounding.AwayFromZero);
        return 0;
    }
}
