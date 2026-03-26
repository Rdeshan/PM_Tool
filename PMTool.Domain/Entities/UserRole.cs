namespace PMTool.Domain.Entities;

public class UserRole
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? ProjectId { get; set; } // If null, role is organization-wide
    public bool IsActive { get; set; } = true;

    public DateTime AssignedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Role? Role { get; set; }
}
