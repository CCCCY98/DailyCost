using System.Security.Claims;

namespace DailyCost.Application.Abstractions;

public interface IJwtService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(Guid userId, string email);
    string CreateRefreshToken();
    string CreatePasswordResetToken(Guid userId, string email);
    ClaimsPrincipal? ValidatePasswordResetToken(string token);
}

