using PMTool.Domain.Enums;

namespace PMTool.Domain.Entities;

public class DailyTask
{
    public Guid Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? ProductBacklogId { get; set; }
    public ProductBacklog? ProductBacklog { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    public bool IsCompleted { get; set; }
    public DailyTaskStatus Status { get; set; } = DailyTaskStatus.Pending;
    public string? PMComment { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


