namespace PMTool.Application.DTOs.Backlog;

public class CreateProductBacklogItemRequest
{
    public Guid ProductId { get; set; }
    public Guid? ParentBacklogItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public int Status { get; set; }
    public Guid? OwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int StoryPoints { get; set; } = 0;
}
