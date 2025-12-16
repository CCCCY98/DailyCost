using DailyCost.Domain.Enums;

namespace DailyCost.Application.DTOs.User;

public sealed record UserDto(
    Guid Id,
    string Email,
    string? Nickname,
    string? Avatar,
    CalcMode DefaultCalcMode,
    string Currency,
    string Timezone,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? LastLoginAt);

