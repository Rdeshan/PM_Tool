namespace PMTool.Application.DTOs.Project;

public class UpdateProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public string? ColourCode { get; set; }
    public int Status { get; set; }
}
