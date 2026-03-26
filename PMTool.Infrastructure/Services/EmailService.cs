using PMTool.Infrastructure.Services.Interfaces;
using PMTool.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace PMTool.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _emailSettings = new EmailSettings();
        configuration.GetSection("Email").Bind(_emailSettings);
        _logger = logger;
    }

    public async Task<bool> SendPasswordResetAsync(string email, string resetLink)
    {
        try
        {
            if (!_emailSettings.IsEnabled)
            {
                _logger.LogInformation("Email service is disabled. Reset link: {ResetLink}", resetLink);
                return true;
            }

            var subject = "Password Reset Request - PMTool";
            var body = $@"
                <h2>Password Reset Request</h2>
                <p>You requested a password reset for your PMTool account.</p>
                <p>Click the link below to reset your password (valid for 1 hour):</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request this, you can safely ignore this email.</p>
                <hr/>
                <p><small>This is an automated message. Please do not reply to this email.</small></p>
            ";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        try
        {
            if (!_emailSettings.IsEnabled)
            {
                _logger.LogInformation("Email service is disabled. Confirmation link: {ConfirmationLink}", confirmationLink);
                return true;
            }

            var subject = "Confirm Your Email - PMTool";
            var body = $@"
                <h2>Welcome to PMTool!</h2>
                <p>Thank you for registering. Please confirm your email address to complete your registration.</p>
                <p>Click the link below to confirm your email (valid for 24 hours):</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you didn't create this account, you can safely ignore this email.</p>
                <hr/>
                <p><small>This is an automated message. Please do not reply to this email.</small></p>
            ";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email confirmation to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendTwoFactorCodeAsync(string email, string code)
    {
        try
        {
            if (!_emailSettings.IsEnabled)
            {
                _logger.LogInformation("Email service is disabled. 2FA code: {Code}", code);
                return true;
            }

            var subject = "Your Two-Factor Authentication Code - PMTool";
            var body = $@"
                <h2>Two-Factor Authentication</h2>
                <p>You're signing in to your PMTool account.</p>
                <p>Your authentication code is:</p>
                <h1 style='font-size: 32px; letter-spacing: 5px; font-family: monospace;'>{code}</h1>
                <p>This code is valid for 5 minutes.</p>
                <p>If you didn't request this code, please secure your account immediately.</p>
                <hr/>
                <p><small>This is an automated message. Please do not reply to this email.</small></p>
            ";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending 2FA code to {Email}", email);
            return false;
        }
    }

    public async Task<bool> SendAccountLockedAsync(string email)
    {
        try
        {
            if (!_emailSettings.IsEnabled)
            {
                _logger.LogInformation("Email service is disabled. Account locked notification for {Email}", email);
                return true;
            }

            var subject = "Account Locked - PMTool";
            var body = $@"
                <h2>Account Security Alert</h2>
                <p>Your PMTool account has been temporarily locked due to multiple failed login attempts.</p>
                <p>For security reasons, your account will be automatically unlocked in 15 minutes.</p>
                <p><a href='https://localhost:7115/Auth/ForgotPassword'>Click here to reset your password</a></p>
                <p>If this wasn't you, please contact support immediately.</p>
                <hr/>
                <p><small>This is an automated message. Please do not reply to this email.</small></p>
            ";

            return await SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending account locked notification to {Email}", email);
            return false;
        }
    }

    private async Task<bool> SendEmailAsync(string recipientEmail, string subject, string htmlBody)
    {
        try
        {
            using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                smtpClient.EnableSsl = _emailSettings.UseSSL;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    mailMessage.To.Add(recipientEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = htmlBody;
                    mailMessage.IsBodyHtml = true;

                    await smtpClient.SendMailAsync(mailMessage);

                    _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {RecipientEmail}", recipientEmail);
            return false;
        }
    }
}

