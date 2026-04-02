namespace PMTool.Application.DTOs.Auth;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string? TempToken { get; set; }
    public List<string> Roles { get; set; } = new();
}

