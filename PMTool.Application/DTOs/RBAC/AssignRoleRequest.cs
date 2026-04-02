namespace PMTool.Application.DTOs.RBAC;

public class AssignRoleRequest
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
}
