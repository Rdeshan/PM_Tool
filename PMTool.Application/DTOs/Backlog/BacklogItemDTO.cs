namespace PMTool.Application.DTOs.Backlog;

public class BacklogItemDTO
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
