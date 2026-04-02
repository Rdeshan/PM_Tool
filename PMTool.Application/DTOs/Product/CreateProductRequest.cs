namespace PMTool.Application.DTOs.Product;

public class CreateProductRequest
{
    public Guid ProjectId { get; set; }
    public string VersionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PlannedReleaseDate { get; set; }
    public int ReleaseType { get; set; } // Major=1, Minor=2, Patch=3, Hotfix=4
}
