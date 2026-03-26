namespace PMTool.Infrastructure.Services.Interfaces;

public interface ITokenService
{
    string GenerateRandomToken();
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
