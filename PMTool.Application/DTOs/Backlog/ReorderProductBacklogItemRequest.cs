namespace PMTool.Application.DTOs.Backlog;

public class ReorderProductBacklogItemRequest
{
    public Guid ItemId { get; set; }
    public int Priority { get; set; }
}
