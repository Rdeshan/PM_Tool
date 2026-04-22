namespace PMTool.Application.DTOs.Backlog;

public class CreateBacklogItemRequest
{
    public Guid ProjectId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? SubProjectId { get; set; }
    public Guid? ParentBacklogItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Status { get; set; }
    public Guid? OwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
}
