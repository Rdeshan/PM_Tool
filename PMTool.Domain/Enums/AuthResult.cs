namespace PMTool.Domain.Enums;

public enum AuthResult
{
    Success,
    InvalidCredentials,
    UserNotFound,
    AccountLocked,
    EmailNotConfirmed,
    TwoFactorRequired,
    InvalidToken,
    TokenExpired,
    UserAlreadyExists,
    OperationFailed
}
