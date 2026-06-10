namespace PMTool.Infrastructure.DTOs.Audit;

public class AuditQueryRequest
{
    public string? UserFilter { get; set; }
    public string? EntityType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
