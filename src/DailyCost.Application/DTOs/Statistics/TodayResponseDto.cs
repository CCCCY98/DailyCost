namespace DailyCost.Application.DTOs.Statistics;

public sealed record TodayResponseDto(DateTime Date, decimal TotalDailyCost, int ActiveCount, string Currency);

