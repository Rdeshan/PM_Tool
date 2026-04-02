namespace PMTool.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int RoleType { get; set; } // RoleType enum value
    public bool IsSystemRole { get; set; } // System roles cannot be deleted
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
