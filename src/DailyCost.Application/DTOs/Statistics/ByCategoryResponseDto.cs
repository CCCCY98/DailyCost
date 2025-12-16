namespace DailyCost.Application.DTOs.Statistics;

public sealed record ByCategoryResponseDto(DateTime Date, decimal TotalDailyCost, IReadOnlyList<ByCategoryItemDto> Items);

