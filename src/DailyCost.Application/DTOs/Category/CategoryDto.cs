namespace DailyCost.Application.DTOs.Category;

public sealed record CategoryDto(
    Guid Id,
    Guid? UserId,
    string Name,
    string? Icon,
    string? Color,
    bool IsSystem,
    int SortOrder);

