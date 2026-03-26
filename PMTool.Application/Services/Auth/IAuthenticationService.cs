using PMTool.Application.DTOs.Auth;

namespace PMTool.Application.Services.Auth;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> VerifyTwoFactorCodeAsync(string email, string code);
    Task<bool> RegisterAsync(RegisterRequest request);
    Task<bool> ConfirmEmailAsync(string email, string token);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task<bool> EnableTwoFactorAsync(string email);
    Task<bool> DisableTwoFactorAsync(string email);
}
