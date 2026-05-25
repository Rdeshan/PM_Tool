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

namespace PMTool.Web.Pages.Products;

public class SummaryModel : PageModel
{
    private readonly IProjectService _projectService;
    private readonly IProductService _productService;
    private readonly IProductBacklogService _productBacklogService;
    private readonly ISubProjectService _subProjectService;
    private readonly ISprintService _sprintService;

    public SummaryModel(
        IProjectService projectService,
        IProductService productService,
        IProductBacklogService productBacklogService,
        ISubProjectService subProjectService,
        ISprintService sprintService)
    {
        _projectService = projectService;
        _productService = productService;
        _productBacklogService = productBacklogService;
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
        var subProjects = await _subProjectService.GetSubProjectsByProductAsync(ProductId);
        var activeSprint = await _sprintService.GetActiveSprintAsync(ProductId);

        StatusDistribution = BuildStatusDistribution(backlogItems);
        CategoryBreakdown = BuildCategoryBreakdown(backlogItems);
        BuildTeamWorkload(backlogItems, activeSprint);
        Milestones = BuildMilestoneProgress(backlogItems, subProjects);
        BugTrend = BuildBugTrend(backlogItems);

        return Page();
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
}
