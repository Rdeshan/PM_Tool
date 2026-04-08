namespace PMTool.Application.DTOs.Backlog;

public class CreateBacklogItemRequest
{
    public Guid ProjectId { get; set; }
    public Guid? ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Status { get; set; }
    public Guid? OwnerId { get; set; }
}
