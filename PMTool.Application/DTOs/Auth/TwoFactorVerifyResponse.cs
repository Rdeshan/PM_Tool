namespace PMTool.Application.DTOs.Auth;

public class TwoFactorVerifyResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public List<string> Roles { get; set; } = new();
}
