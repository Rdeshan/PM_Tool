namespace PMTool.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool IsActive { get; set; } = true;
    public bool NotificationsEnabled { get; set; } = true;

    // Account lockout
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }

    // Password reset
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // Email confirmation
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiry { get; set; }

    // Invitation
    public string? InvitationToken { get; set; }
    public DateTime? InvitationTokenExpiry { get; set; }
    public bool AccountSetupCompleted { get; set; }

    // 2FA
    public string? TwoFactorCode { get; set; }
    public DateTime? TwoFactorCodeExpiry { get; set; }

    // Session
    public DateTime? LastLoginAt { get; set; }
    public DateTime? SessionExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeactivatedAt { get; set; }

    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
