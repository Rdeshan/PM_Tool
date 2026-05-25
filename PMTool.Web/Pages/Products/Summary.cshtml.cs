using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PMTool.Application.DTOs.Backlog;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.DTOs.Sprint;
using PMTool.Application.Interfaces;
using PMTool.Application.Services.SubProject;
using PMTool.Domain.Enums;

namespace PMTool.Web.Pages.Products;

public class SummaryModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IProductService _productService;
    private readonly IProductBacklogService _productBacklogService;
    private readonly IBacklogService _backlogService;
    private readonly ISubProjectService _subProjectService;
    private readonly ISprintService _sprintService;

    public SummaryModel(
        IProjectService projectService,
        IProductService productService,
        IProductBacklogService productBacklogService,
        IBacklogService backlogService,
        ISubProjectService subProjectService,
        ISprintService sprintService)
    {
        _projectService = projectService;
        _productService = productService;
        _productBacklogService = productBacklogService;
        _backlogService = backlogService;
        _subProjectService = subProjectService;
        _sprintService = sprintService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid ProjectId { get; set; }

    [BindProperty(SupportsGet = true, Name = "id")]
    public Guid ProductId { get; set; }

    public string ProjectName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public ChartData StatusDistribution { get; set; } = new();
    public ChartData CategoryBreakdown { get; set; } = new();
    public ChartData TeamWorkload { get; set; } = new();
    public string WorkloadUnit { get; set; } = "Story points";
    public List<MilestoneProgress> Milestones { get; set; } = new();
    public TrendData BugTrend { get; set; } = new();
    public List<SubProjectDTO> SubProjects { get; set; } = new();
    public List<SprintDTO> Sprints { get; set; } = new();
    public List<RtmRow> RtmRows { get; set; } = new();
    public List<DependencyItem> TicketDependencies { get; set; } = new();
    public List<SubProjectDependencyItem> SubProjectDependencies { get; set; } = new();
    public List<TeamOption> TeamOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid projectId, Guid id)
    {
        ProjectId = projectId;
        ProductId = id;

        var project = await _projectService.GetProjectByIdAsync(projectId);
        ProjectName = project?.Name ?? string.Empty;

        var product = await _productService.GetProductByIdAsync(ProductId);
        if (product == null) return NotFound();
        ProductName = product.VersionName;

        var backlogItems = await _productBacklogService.GetBacklogItemsAsync(ProductId, null);
        var projectBacklogItems = await _backlogService.GetBacklogItemsAsync(ProjectId, ProductId, null, null);
        var subProjects = await _subProjectService.GetSubProjectsByProductAsync(ProductId);
        var activeSprint = await _sprintService.GetActiveSprintAsync(ProductId);
        var sprints = await _sprintService.GetSprintsByProductAsync(ProductId);

        StatusDistribution = BuildStatusDistribution(backlogItems);
        CategoryBreakdown = BuildCategoryBreakdown(backlogItems);
        BuildTeamWorkload(backlogItems, activeSprint);
        Milestones = BuildMilestoneProgress(backlogItems, subProjects);
        BugTrend = BuildBugTrend(backlogItems);
        SubProjects = subProjects;
        Sprints = sprints;
        RtmRows = BuildRtmRows(projectBacklogItems);
        TicketDependencies = BuildTicketDependencies(backlogItems, subProjects);
        SubProjectDependencies = BuildSubProjectDependencies(subProjects);
        TeamOptions = BuildTeamOptions(subProjects);

        return Page();
    }

    private static List<RtmRow> BuildRtmRows(List<BacklogItemDTO> items)
    {
        var requirements = items.Where(i => i.Type == (int)BacklogItemType.BRD).ToList();
        var useCases = items.Where(i => i.Type == (int)BacklogItemType.UseCase).ToList();
        var userStories = items.Where(i => i.Type == (int)BacklogItemType.UserStory).ToList();
        var testCases = items.Where(i => i.Type == (int)BacklogItemType.TestCase).ToList();

        var ticketTypes = new HashSet<int>
        {
            (int)BacklogItemType.Bug,
            (int)BacklogItemType.Feature,
            (int)BacklogItemType.Improvement,
            (int)BacklogItemType.ChangeRequest,
            (int)BacklogItemType.Epic
        };

        var tickets = items.Where(i => ticketTypes.Contains(i.Type)).ToList();
        var rows = new List<RtmRow>();

        foreach (var requirement in requirements)
        {
            var linkedUseCases = useCases.Where(u => u.ParentBacklogItemId == requirement.Id).DefaultIfEmpty();
            foreach (var useCase in linkedUseCases)
            {
                var useCaseId = useCase?.Id;
                var linkedStories = userStories.Where(us => us.ParentBacklogItemId == useCaseId || us.ParentBacklogItemId == requirement.Id).DefaultIfEmpty();

                foreach (var story in linkedStories)
                {
                    var storyId = story?.Id;
                    var linkedTickets = tickets.Where(t => t.ParentBacklogItemId == storyId || t.ParentBacklogItemId == useCaseId || t.ParentBacklogItemId == requirement.Id).ToList();
                    var linkedTests = testCases.Where(tc => tc.ParentBacklogItemId == storyId || tc.ParentBacklogItemId == useCaseId || tc.ParentBacklogItemId == requirement.Id).ToList();

                    if (linkedTickets.Count == 0)
                    {
                        rows.Add(BuildRtmRow(requirement, useCase, story, linkedTests, null, true, false));
                        continue;
                    }

                    foreach (var ticket in linkedTickets)
                    {
                        rows.Add(BuildRtmRow(requirement, useCase, story, linkedTests, ticket, false, false));
                    }
                }
            }
        }

        var requirementIds = new HashSet<Guid>(requirements.Select(r => r.Id));
        var linkedTicketIds = new HashSet<Guid>(rows.Where(r => r.TicketId.HasValue).Select(r => r.TicketId!.Value));

        foreach (var ticket in tickets.Where(t => !linkedTicketIds.Contains(t.Id)))
        {
            rows.Add(new RtmRow
            {
                RequirementId = "Unlinked",
                RequirementDescription = "No linked requirement",
                UseCaseNumber = string.Empty,
                UserStory = string.Empty,
                LinkedTickets = ShortId(ticket.Id, "TCK"),
                TicketStatus = ticket.StatusName,
                TestCaseNumber = string.Empty,
                TestStatus = string.Empty,
                MissingRequirement = !requirementIds.Contains(ticket.ParentBacklogItemId ?? Guid.Empty),
                MissingTicket = false,
                SubProjectId = ticket.SubProjectId,
                TicketId = ticket.Id
            });
        }

        return rows;
    }

    private static RtmRow BuildRtmRow(
        BacklogItemDTO requirement,
        BacklogItemDTO? useCase,
        BacklogItemDTO? userStory,
        List<BacklogItemDTO> tests,
        BacklogItemDTO? ticket,
        bool missingTicket,
        bool missingRequirement)
    {
        return new RtmRow
        {
            RequirementId = ShortId(requirement.Id, "REQ"),
            RequirementDescription = string.IsNullOrWhiteSpace(requirement.Description)
                ? requirement.Title
                : requirement.Description,
            UseCaseNumber = useCase == null ? string.Empty : ShortId(useCase.Id, "UC"),
            UserStory = userStory == null ? string.Empty : userStory.Title,
            LinkedTickets = ticket == null ? string.Empty : ShortId(ticket.Id, "TCK") + " " + ticket.Title,
            TicketStatus = ticket?.StatusName ?? string.Empty,
            TestCaseNumber = tests.Count == 0 ? string.Empty : string.Join(", ", tests.Select(tc => ShortId(tc.Id, "TC"))),
            TestStatus = tests.Count == 0 ? string.Empty : string.Join(", ", tests.Select(tc => tc.StatusName)),
            MissingTicket = missingTicket,
            MissingRequirement = missingRequirement,
            SubProjectId = ticket?.SubProjectId ?? userStory?.SubProjectId ?? useCase?.SubProjectId ?? requirement.SubProjectId,
            TicketId = ticket?.Id
        };
    }

    private static List<DependencyItem> BuildTicketDependencies(
        List<ProductBacklogItemDTO> items,
        List<SubProjectDTO> subProjects)
    {
        var teamMap = subProjects
            .SelectMany(sp => sp.Teams.Select(t => (sp.Id, t.TeamId)))
            .GroupBy(x => x.Id)
            .ToDictionary(g => g.Key, g => g.Select(x => x.TeamId).Distinct().ToList());

        return items.Select(item => new DependencyItem
        {
            Id = item.Id,
            ShortId = ShortId(item.Id, "PB"),
            Title = item.Title,
            Status = item.Status,
            StatusName = item.StatusName,
            SubProjectId = item.SubProjectId,
            SprintId = item.SprintId,
            ParentId = item.ParentBacklogItemId,
            TeamIds = item.SubProjectId.HasValue && teamMap.TryGetValue(item.SubProjectId.Value, out var teams)
                ? teams
                : new List<Guid>()
        }).ToList();
    }

    private static List<SubProjectDependencyItem> BuildSubProjectDependencies(List<SubProjectDTO> subProjects)
    {
        return subProjects.Select(sp => new SubProjectDependencyItem
        {
            Id = sp.Id,
            Name = sp.Name,
            Status = sp.Status,
            DependsOnIds = sp.Dependencies.Select(d => d.DependsOnSubProjectId).ToList()
        }).ToList();
    }

    private static List<TeamOption> BuildTeamOptions(List<SubProjectDTO> subProjects)
    {
        return subProjects
            .SelectMany(sp => sp.Teams)
            .GroupBy(t => t.TeamId)
            .Select(g => new TeamOption { TeamId = g.Key, TeamName = g.First().TeamName })
            .OrderBy(t => t.TeamName)
            .ToList();
    }

    private static string ShortId(Guid id, string prefix)
    {
        return $"{prefix}-{id.ToString("N")[..8].ToUpperInvariant()}";
    }

    private static ChartData BuildStatusDistribution(IEnumerable<ProductBacklogItemDTO> items)
    {
        var map = new Dictionary<int, string>
        {
            [1] = "To do",
            [2] = "In progress",
            [3] = "In review",
            [4] = "Done"
        };

        var groups = items
            .GroupBy(i => i.Status)
            .OrderBy(g => g.Key)
            .Select(g => new { Label = map.GetValueOrDefault(g.Key, "Other"), Value = g.Count() })
            .ToList();

        return new ChartData
        {
            Labels = groups.Select(g => g.Label).ToList(),
            Values = groups.Select(g => g.Value).ToList()
        };
    }

    private static ChartData BuildCategoryBreakdown(IEnumerable<ProductBacklogItemDTO> items)
    {
        var groups = items
            .GroupBy(i => string.IsNullOrWhiteSpace(i.TypeName) ? $"Type {i.Type}" : i.TypeName)
            .OrderByDescending(g => g.Count())
            .Select(g => new { Label = g.Key, Value = g.Count() })
            .ToList();

        return new ChartData
        {
            Labels = groups.Select(g => g.Label).ToList(),
            Values = groups.Select(g => g.Value).ToList()
        };
    }

    private void BuildTeamWorkload(IEnumerable<ProductBacklogItemDTO> backlogItems, SprintDTO? activeSprint)
    {
        var items = activeSprint?.BacklogItems?.Any() == true
            ? activeSprint.BacklogItems
            : backlogItems.ToList();

        var hasPoints = items.Any(i => i.StoryPoints > 0);
        WorkloadUnit = hasPoints ? "Story points" : "Tickets";

        var groups = items
            .GroupBy(i => string.IsNullOrWhiteSpace(i.OwnerName) ? "Unassigned" : i.OwnerName)
            .OrderByDescending(g => hasPoints ? g.Sum(i => i.StoryPoints) : g.Count())
            .ToList();

        TeamWorkload = new ChartData
        {
            Labels = groups.Select(g => g.Key).ToList(),
            Values = groups.Select(g => hasPoints ? g.Sum(i => i.StoryPoints) : g.Count()).ToList()
        };
    }

    private static List<MilestoneProgress> BuildMilestoneProgress(
        IEnumerable<ProductBacklogItemDTO> items,
        IEnumerable<SubProjectDTO> subProjects)
    {
        var itemList = items.ToList();
        var milestones = new List<MilestoneProgress>();

        foreach (var subProject in subProjects)
        {
            var scoped = itemList.Where(i => i.SubProjectId == subProject.Id).ToList();
            if (!scoped.Any()) continue;

            var totalPoints = scoped.Sum(i => i.StoryPoints);
            var donePoints = scoped.Where(i => i.Status == 4).Sum(i => i.StoryPoints);

            if (totalPoints == 0)
            {
                totalPoints = scoped.Count;
                donePoints = scoped.Count(i => i.Status == 4);
            }

            var percent = totalPoints == 0 ? 0 : (int)Math.Round(donePoints * 100d / totalPoints);

            milestones.Add(new MilestoneProgress
            {
                Name = subProject.Name,
                Completed = donePoints,
                Total = totalPoints,
                Percent = percent
            });
        }

        return milestones.OrderByDescending(m => m.Percent).ToList();
    }

    private static TrendData BuildBugTrend(IEnumerable<ProductBacklogItemDTO> items)
    {
        var bugItems = items
            .Where(i => i.Type == 3 || i.TypeName.Contains("bug", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var today = DateTime.UtcNow.Date;
        var start = today.AddDays(-7 * 11);
        var labels = new List<string>();
        var opened = new List<int>();
        var closed = new List<int>();

        for (var i = 0; i < 12; i++)
        {
            var weekStart = start.AddDays(i * 7);
            var weekEnd = weekStart.AddDays(7);
            labels.Add($"{weekStart:MMM d}");

            opened.Add(bugItems.Count(b => b.CreatedAt >= weekStart && b.CreatedAt < weekEnd));
            closed.Add(bugItems.Count(b => b.Status == 4 && b.UpdatedAt >= weekStart && b.UpdatedAt < weekEnd));
        }

        return new TrendData
        {
            Labels = labels,
            Opened = opened,
            Closed = closed
        };
    }

    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Values { get; set; } = new();
    }

    public class TrendData
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Opened { get; set; } = new();
        public List<int> Closed { get; set; } = new();
    }

    public class MilestoneProgress
    {
        public string Name { get; set; } = string.Empty;
        public int Completed { get; set; }
        public int Total { get; set; }
        public int Percent { get; set; }
    }

    public class RtmRow
    {
        public string RequirementId { get; set; } = string.Empty;
        public string RequirementDescription { get; set; } = string.Empty;
        public string UseCaseNumber { get; set; } = string.Empty;
        public string UserStory { get; set; } = string.Empty;
        public string LinkedTickets { get; set; } = string.Empty;
        public string TicketStatus { get; set; } = string.Empty;
        public string TestCaseNumber { get; set; } = string.Empty;
        public string TestStatus { get; set; } = string.Empty;
        public bool MissingTicket { get; set; }
        public bool MissingRequirement { get; set; }
        public Guid? SubProjectId { get; set; }
        public Guid? TicketId { get; set; }
    }

    public class DependencyItem
    {
        public Guid Id { get; set; }
        public string ShortId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid? SubProjectId { get; set; }
        public Guid? SprintId { get; set; }
        public Guid? ParentId { get; set; }
        public List<Guid> TeamIds { get; set; } = new();
    }

    public class SubProjectDependencyItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Status { get; set; }
        public List<Guid> DependsOnIds { get; set; } = new();
    }

    public class TeamOption
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }
}
