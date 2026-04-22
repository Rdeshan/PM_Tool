namespace PMTool.Domain.Entities;

public class SubProject
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., "Student Registration Portal"
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; } = 1; // SubProjectStatus enum value (NotStarted = 1)
    public Guid ModuleOwnerId { get; set; } // The project manager/lead for this sub-project
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ColorCode { get; set; }
    public int Progress { get; set; } = 0; // 0-100, calculated from ticket completion ratio
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public User? ModuleOwner { get; set; }
    public ICollection<SubProjectTeam> SubProjectTeams { get; set; } = new List<SubProjectTeam>();
    public ICollection<SubProjectDependency> DependsOn { get; set; } = new List<SubProjectDependency>(); // Dependencies this sub-project has
    public ICollection<SubProjectDependency> DependentOn { get; set; } = new List<SubProjectDependency>(); // Sub-projects that depend on this one
    public ICollection<ProjectBacklog> Backlog { get; set; } = new List<ProjectBacklog>(); // Tickets specific to this sub-project
}
