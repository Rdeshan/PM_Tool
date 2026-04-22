namespace PMTool.Domain.Entities;

public class ProjectBacklog
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ProductId { get; set; } // Optional: if null, backlog is for project; if set, backlog is for product
    public Guid? SubProjectId { get; set; } // Optional: if set, backlog is for sub-project
    public Guid? ParentBacklogItemId { get; set; }
    public Guid? OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; } // BRD, UserStory, Requirement, etc.
    public int Priority { get; set; } // Used as rank/order for prioritization
    public int Status { get; set; } // Draft, Approved, InProgress, Done
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Project? Project { get; set; }
    public Product? Product { get; set; }
    public SubProject? SubProject { get; set; }
    public ProjectBacklog? ParentBacklogItem { get; set; }
    public ICollection<ProjectBacklog> ChildBacklogItems { get; set; } = new List<ProjectBacklog>();
    public User? Owner { get; set; }
}
