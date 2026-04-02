namespace PMTool.Application.DTOs.SubProject;

public class UpdateSubProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; } // SubProjectStatus enum value
    public Guid ModuleOwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<Guid> TeamIds { get; set; } = new();
    public List<string> TeamRoles { get; set; } = new();
}
