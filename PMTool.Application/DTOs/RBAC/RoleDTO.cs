namespace PMTool.Application.DTOs.RBAC;

public class RoleDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
}
