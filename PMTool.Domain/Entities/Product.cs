namespace PMTool.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string VersionName { get; set; } = string.Empty; // e.g., "1.0.0", "2.5.3"
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedReleaseDate { get; set; }
    public DateTime? ActualReleaseDate { get; set; } // Null until released
    public int Status { get; set; } = 1; // ProductStatus enum value (Planned = 1)
    public int ReleaseType { get; set; } = 1; // ReleaseType enum value (Major = 1)
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Project? Project { get; set; }
    public ICollection<ReleaseNotes> ReleaseNotes { get; set; } = new List<ReleaseNotes>();
    public ICollection<ProjectBacklog> Backlogs { get; set; } = new List<ProjectBacklog>();
}
