namespace PMTool.Domain.Entities;

public class RolePermission
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
