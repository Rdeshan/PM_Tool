namespace PMTool.Application.DTOs.Backlog;

public class ReorderBacklogItemRequest
{
    public Guid ItemId { get; set; }
    public int Priority { get; set; }
}
