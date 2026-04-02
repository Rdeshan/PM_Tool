namespace PMTool.Application.DTOs.SubProject;

public class CreateSubProjectRequest
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid ModuleOwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public List<Guid> TeamIds { get; set; } = new(); // IDs of teams to assign
    public List<string> TeamRoles { get; set; } = new(); // Roles for each team (Development, QA, etc.)
}
