namespace PMTool.Application.DTOs.Product;

public class UpdateProductRequest
{
    public string VersionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedReleaseDate { get; set; }
    public DateTime? ActualReleaseDate { get; set; }
    public int Status { get; set; } // ProductStatus enum value
    public int ReleaseType { get; set; } // ReleaseType enum value
}
