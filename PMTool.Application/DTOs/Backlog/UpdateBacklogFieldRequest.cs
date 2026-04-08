namespace PMTool.Application.DTOs.Backlog;

public class UpdateBacklogFieldRequest
{
    public Guid ItemId { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
