using PMTool.Application.DTOs.Auth;
using PMTool.Domain.Entities;
using PMTool.Domain.Enums;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Web;
using PMTool.Application.Interfaces;

namespace PMTool.Application.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;
    private const int SessionTimeoutMinutes = 30;

    public AuthenticationService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            return new LoginResponse { Success = false, Message = "Invalid email or password" };

        if (!user.IsActive)
            return new LoginResponse { Success = false, Message = "Account is inactive" };

        if (user.IsLockedOut)
            return new LoginResponse 
            { 
                Success = false, 
                Message = $"Account is locked. Try again after {user.LockoutEnd:HH:mm} UTC" 
            };

        if (!_tokenService.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                await _userRepository.UpdateAsync(user);
                await _emailService.SendAccountLockedAsync(user.Email);
                return new LoginResponse { Success = false, Message = "Account locked due to too many failed attempts" };
            }

            await _userRepository.UpdateAsync(user);
            return new LoginResponse { Success = false, Message = "Invalid email or password" };
        }

        // Reset failed attempts on successful password verification
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;

        if (!user.EmailConfirmed)
            return new LoginResponse { Success = false, Message = "Please confirm your email address" };

        if (user.TwoFactorEnabled)
        {
            var code = _tokenService.GenerateRandomToken().Substring(0, 6);
            user.TwoFactorCode = _tokenService.HashPassword(code);
            user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(5);
            await _userRepository.UpdateAsync(user);
            await _emailService.SendTwoFactorCodeAsync(user.Email, code);

            return new LoginResponse
            {
                Success = true,
                Message = "Two-factor code sent to your email",
                RequiresTwoFactor = true,
                TempToken = _tokenService.GenerateRandomToken(),
                UserId = user.Id.ToString()
            };
        }

        user.LastLoginAt = DateTime.UtcNow;
        user.SessionExpiresAt = DateTime.UtcNow.AddMinutes(SessionTimeoutMinutes);
        await _userRepository.UpdateAsync(user);

        var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "User").ToList() ?? new List<string> { "User" };

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            UserId = user.Id.ToString(),
            Roles = roles
        };
    }

    public async Task<TwoFactorVerifyResponse> VerifyTwoFactorCodeAsync(string email, string code)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !user.TwoFactorEnabled)
            return new TwoFactorVerifyResponse { Success = false, Message = "User not found" };

        if (user.TwoFactorCodeExpiry < DateTime.UtcNow)
            return new TwoFactorVerifyResponse { Success = false, Message = "Verification code expired" };

        if (!_tokenService.VerifyPassword(code, user.TwoFactorCode ?? string.Empty))
            return new TwoFactorVerifyResponse { Success = false, Message = "Invalid verification code" };

        user.LastLoginAt = DateTime.UtcNow;
        user.SessionExpiresAt = DateTime.UtcNow.AddMinutes(SessionTimeoutMinutes);
        user.TwoFactorCode = null;
        user.TwoFactorCodeExpiry = null;
        await _userRepository.UpdateAsync(user);

        var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "User").ToList() ?? new List<string> { "User" };

        return new TwoFactorVerifyResponse
        {
            Success = true,
            Message = "Two-factor verification successful",
            UserId = user.Id.ToString(),
            Roles = roles
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return false;

        var user = new Domain.Entities.User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = _tokenService.HashPassword(request.Password),
            EmailConfirmed = false,
            TwoFactorEnabled = false,
            IsActive = true,
            EmailConfirmationToken = _tokenService.GenerateRandomToken(),
            EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24)
        };

        var result = await _userRepository.CreateAsync(user);

        if (result)
        {
            var confirmationLink = $"{_configuration["AppUrl"]}/Auth/ConfirmEmail?token={System.Web.HttpUtility.UrlEncode(user.EmailConfirmationToken)}&email={System.Web.HttpUtility.UrlEncode(request.Email)}";
            await _emailService.SendEmailConfirmationAsync(request.Email, confirmationLink);
        }

        return result;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        if (user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
            return false;

        if (user.EmailConfirmationToken != token)
            return false;

        user.EmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.EmailConfirmationTokenExpiry = null;

        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        user.PasswordResetToken = _tokenService.GenerateRandomToken();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        var updated = await _userRepository.UpdateAsync(user);

        if (updated)
        {
            var resetLink = $"{_configuration["AppUrl"]}/Auth/ResetPassword?token={System.Web.HttpUtility.UrlEncode(user.PasswordResetToken)}&email={System.Web.HttpUtility.UrlEncode(email)}";
            await _emailService.SendPasswordResetAsync(email, resetLink);
        }

        return updated;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return false;

        if (user.PasswordResetToken != token)
            return false;

        user.PasswordHash = _tokenService.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;

        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> EnableTwoFactorAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        user.TwoFactorEnabled = true;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> DisableTwoFactorAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        user.TwoFactorEnabled = false;
        user.TwoFactorCode = null;
        user.TwoFactorCodeExpiry = null;
        return await _userRepository.UpdateAsync(user);
    }
}
