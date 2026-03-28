namespace DigitalStore.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(int userId, string email, string role);
}
