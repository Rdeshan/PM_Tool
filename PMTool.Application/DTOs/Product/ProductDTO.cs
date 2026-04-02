namespace PMTool.Application.DTOs.Product;

public class ProductDTO
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string VersionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedReleaseDate { get; set; }
    public DateTime? ActualReleaseDate { get; set; }
    public int Status { get; set; }
    public int ReleaseType { get; set; }
    public int BacklogItemCount { get; set; }
    public int ReleaseNotesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
