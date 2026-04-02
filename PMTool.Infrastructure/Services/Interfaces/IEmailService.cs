namespace PMTool.Infrastructure.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendPasswordResetAsync(string email, string resetLink);
    Task<bool> SendEmailConfirmationAsync(string email, string confirmationLink);
    Task<bool> SendTwoFactorCodeAsync(string email, string code);
    Task<bool> SendAccountLockedAsync(string email);
    Task<bool> SendAccountInvitationAsync(string email, string setupLink);
}
