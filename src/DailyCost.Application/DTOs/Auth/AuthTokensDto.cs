namespace DailyCost.Application.DTOs.Auth;

public sealed record AuthTokensDto(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

