namespace DailyCost.Application.DTOs.Statistics;

public sealed record ByCategoryItemDto(Guid? CategoryId, string CategoryName, decimal TotalDailyCost, decimal Percent);

