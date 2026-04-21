namespace PMTool.Application.DTOs.SubProject;

public class SubProjectDTO
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; } // SubProjectStatus enum value
    public Guid ModuleOwnerId { get; set; }
    public string ModuleOwnerName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int Progress { get; set; } // 0-100
    public int TicketCount { get; set; }
    public int CompletedTicketCount { get; set; }
    public List<SubProjectTeamDTO> Teams { get; set; } = new();
    public List<SubProjectDependencyDTO> Dependencies { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SubProjectTeamDTO
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? Role { get; set; }
    public DateTime AssignedDate { get; set; }
}

public class SubProjectDependencyDTO
{
    public Guid Id { get; set; }
    public Guid DependsOnSubProjectId { get; set; }
    public string DependsOnSubProjectName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
