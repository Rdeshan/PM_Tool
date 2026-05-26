namespace PMTool.Application.DTOs.Backlog;

public class BacklogSubtaskDto
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid? ProductBacklogId { get; set; }
    public Guid? ProjectBacklogId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}