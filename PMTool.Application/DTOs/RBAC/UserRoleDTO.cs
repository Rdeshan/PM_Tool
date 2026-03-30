namespace PMTool.Application.DTOs.RBAC;

public class UserRoleDTO
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public DateTime AssignedAt { get; set; }
}
