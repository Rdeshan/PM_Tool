namespace PMTool.Application.DTOs.Backlog;

public class ReorderProductBacklogRequest
{
    public Guid ProductId { get; set; }
    public List<ReorderProductBacklogItemRequest> Items { get; set; } = new();
}
