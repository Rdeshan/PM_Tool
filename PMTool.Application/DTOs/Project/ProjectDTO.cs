namespace PMTool.Application.DTOs.Project;

public class ProjectDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ColourCode { get; set; }
    public bool IsArchived { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
