namespace PMTool.Application.DTOs.Progress;

public enum HealthStatus { OnTrack, AtRisk, Blocked }
public enum RiskSeverity { Ok, Warning, Critical }

public class DashboardProgressDto
{
    public int TotalProjects { get; set; }
    public int ProductCount { get; set; }
    public int AvgCompletionPct { get; set; }
    public int OverdueItemCount { get; set; }
    public int BlockedSprintCount { get; set; }
    public List<ProjectProgressSummaryDto> Projects { get; set; } = new();
}

public class ProjectProgressSummaryDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? ColourCode { get; set; }
    public int OverallProgressPct { get; set; }
    public List<ProductProgressDto> Products { get; set; } = new();
}

public class ProductProgressDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int ProgressPct { get; set; }
    public string ActiveSprintName { get; set; } = "No active sprint";
    public string SprintStatus { get; set; } = "None";
    public int WorkItemCount { get; set; }
    public DateTime? DueDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ProductAnalysisDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int ProductStatus { get; set; }
    public int OverallProgressPct { get; set; }
    public int TotalWorkItems { get; set; }
    public int CompletedWorkItems { get; set; }
    public int InProgressWorkItems { get; set; }
    public int BlockedWorkItems { get; set; }
    public string SprintLabel { get; set; } = "No active sprint";
    public SprintSummaryDto? ActiveSprint { get; set; }
    public List<StatusCountDto> BacklogByStatus { get; set; } = new();
    public List<TypeCountDto> WorkItemsByType { get; set; } = new();
    public List<SubProjectProgressDto> SubProjects { get; set; } = new();
    public List<TeamMemberProgressDto> TeamMembers { get; set; } = new();
    public TaskProgressSummaryDto TaskProgress { get; set; } = new();
    public ForecastDto? Forecast { get; set; }
}

public class SprintSummaryDto
{
    public Guid SprintId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SprintNumber { get; set; }
    public int TotalSprints { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysLeft { get; set; }
    public int TotalItems { get; set; }
    public int RemainingItems { get; set; }
    public List<BurndownPointDto> BurndownPoints { get; set; } = new();
}

public class BurndownPointDto
{
    public DateTime Date { get; set; }
    public double? Ideal { get; set; }
    public int? Actual { get; set; }
    public double? Forecast { get; set; }
}

public class StatusCountDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public string ColourHex { get; set; } = "#6c757d";
}

public class TypeCountDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SubProjectProgressDto
{
    public Guid SubProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProgressPct { get; set; }
    public HealthStatus HealthStatus { get; set; }
}

public class TeamMemberProgressDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int OpenTaskCount { get; set; }
    public string AvatarColorHex { get; set; } = string.Empty;
}

public class WorkItemProgressDto
{
    public Guid WorkItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string WorkType { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int ProgressPct { get; set; }
    public int SubTaskCount { get; set; }
    public int CompletedSubTaskCount { get; set; }
    public DateTime? DueDate { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public string AssigneeInitials { get; set; } = "?";
    public string AssigneeColorHex { get; set; } = "#6c757d";
}

public class TaskProgressSummaryDto
{
    public int TotalSubTasks { get; set; }
    public int CompletedSubTasks { get; set; }
    public List<WorkItemProgressDto> InProgressItems { get; set; } = new();
}

public class ForecastDto
{
    public double Velocity { get; set; }
    public int RemainingItems { get; set; }
    public int TotalItems { get; set; }
    public DateTime? ProjectedCompletionDate { get; set; }
    public int ProjectedOverrunDays { get; set; }
    public bool IsOverrun { get; set; }
    public List<RiskItemDto> Risks { get; set; } = new();
}

public class RiskItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public RiskSeverity Severity { get; set; }
}
