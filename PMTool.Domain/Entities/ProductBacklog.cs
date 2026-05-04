namespace PMTool.Domain.Entities;

public class ProductBacklog
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ParentBacklogItemId { get; set; }
    public Guid? OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; } // UserStory, Task, Bug, Epic, etc.
    public int Priority { get; set; } // Used as rank/order for prioritization
    public int Status { get; set; } // Draft, Approved, InProgress, Done
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; } = 0;
    public Guid? SubProjectId { get; set; }
    public Guid? SprintId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public ProductBacklog? ParentBacklogItem { get; set; }
    public ICollection<ProductBacklog> ChildBacklogItems { get; set; } = new List<ProductBacklog>();
    public User? Owner { get; set; }
    public SubProject? SubProject { get; set; }
    public Sprint? Sprint { get; set; }
}
