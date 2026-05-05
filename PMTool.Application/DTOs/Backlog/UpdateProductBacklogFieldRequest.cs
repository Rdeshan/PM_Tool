namespace PMTool.Application.DTOs.Backlog;

public class UpdateProductBacklogFieldRequest
{
    public Guid ItemId { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
