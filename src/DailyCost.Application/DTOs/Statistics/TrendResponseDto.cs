namespace DailyCost.Application.DTOs.Statistics;

public sealed record TrendResponseDto(int Days, IReadOnlyList<TrendPointDto> Items);

