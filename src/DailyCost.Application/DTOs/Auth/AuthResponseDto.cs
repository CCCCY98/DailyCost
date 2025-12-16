using DailyCost.Application.DTOs.User;

namespace DailyCost.Application.DTOs.Auth;

public sealed record AuthResponseDto(AuthTokensDto Tokens, UserDto User);

