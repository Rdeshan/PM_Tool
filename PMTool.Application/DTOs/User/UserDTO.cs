namespace PMTool.Application.DTOs.User;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Teams { get; set; } = new();
}
