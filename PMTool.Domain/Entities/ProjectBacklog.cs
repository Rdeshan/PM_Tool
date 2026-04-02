namespace PMTool.Domain.Entities;

public class ProjectBacklog
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ProductId { get; set; } // Optional: if null, backlog is for project; if set, backlog is for product
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; } // BRD, UserStory, Requirement, etc.
    public int Priority { get; set; } // High, Medium, Low
    public int Status { get; set; } // Draft, Active, Completed, Archived

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Project? Project { get; set; }
    public Product? Product { get; set; }
}
