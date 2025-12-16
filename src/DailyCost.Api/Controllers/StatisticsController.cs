using DailyCost.Api.Models;
using DailyCost.Application.DTOs.Statistics;
using DailyCost.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyCost.Api.Controllers;

[Route("api/v1/statistics")]
[Authorize]
public sealed class StatisticsController : BaseApiController
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("today")]
    public async Task<ActionResult<ApiResponse<TodayResponseDto>>> Today(CancellationToken cancellationToken)
    {
        var result = await _statisticsService.GetTodayAsync(GetUserId(), cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<TodayResponseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<TodayResponseDto>.Fail(result.Message ?? "获取失败"));
    }

    [HttpGet("trend")]
    public async Task<ActionResult<ApiResponse<TrendResponseDto>>> Trend([FromQuery] int days = 30, CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetTrendAsync(GetUserId(), days, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<TrendResponseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<TrendResponseDto>.Fail(result.Message ?? "获取失败"));
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<ApiResponse<ByCategoryResponseDto>>> ByCategory(CancellationToken cancellationToken)
    {
        var result = await _statisticsService.GetByCategoryAsync(GetUserId(), cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<ByCategoryResponseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<ByCategoryResponseDto>.Fail(result.Message ?? "获取失败"));
    }
}

