namespace PMTool.Application.DTOs.Team;

public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ColorCode { get; set; } = "#007bff";
    public bool IsActive { get; set; } = true;
}
