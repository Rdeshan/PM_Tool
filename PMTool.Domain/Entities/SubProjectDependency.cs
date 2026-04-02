namespace PMTool.Domain.Entities;

public class SubProjectDependency
{
    public Guid Id { get; set; }
    public Guid SubProjectId { get; set; } // The sub-project that depends on another
    public Guid DependsOnSubProjectId { get; set; } // The sub-project that must be completed first
    public string? Notes { get; set; } // Why this dependency exists
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public SubProject? SubProject { get; set; }
    public SubProject? DependsOnSubProject { get; set; }
}
