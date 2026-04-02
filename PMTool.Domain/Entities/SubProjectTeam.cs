namespace PMTool.Domain.Entities;

public class SubProjectTeam
{
    public Guid Id { get; set; }
    public Guid SubProjectId { get; set; }
    public Guid TeamId { get; set; }
    public string? Role { get; set; } // e.g., "Development", "QA", "BA", etc.
    
    public DateTime AssignedDate { get; set; }

    // Navigation properties
    public SubProject? SubProject { get; set; }
    public Team? Team { get; set; }
}
