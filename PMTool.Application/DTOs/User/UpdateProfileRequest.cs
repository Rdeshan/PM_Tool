namespace PMTool.Application.DTOs.User;

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
}
