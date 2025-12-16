using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Statistics;

namespace DailyCost.Application.Services;

public interface IStatisticsService
{
    Task<Result<TodayResponseDto>> GetTodayAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<TrendResponseDto>> GetTrendAsync(Guid userId, int days, CancellationToken cancellationToken);
    Task<Result<ByCategoryResponseDto>> GetByCategoryAsync(Guid userId, CancellationToken cancellationToken);
}

