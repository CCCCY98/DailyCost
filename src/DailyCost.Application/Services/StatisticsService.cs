using DailyCost.Application.Abstractions;
using DailyCost.Application.Calculations;
using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Statistics;
using DailyCost.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Application.Services;

public sealed class StatisticsService : IStatisticsService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DailyCostCalculator _calculator;

    public StatisticsService(IAppDbContext db, IDateTimeProvider dateTimeProvider, DailyCostCalculator calculator)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
        _calculator = calculator;
    }

    public async Task<Result<TodayResponseDto>> GetTodayAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<TodayResponseDto>.Fail("用户不存在");

        var today = _dateTimeProvider.TodayLocal(user.Timezone);
        var items = await _db.ExpenseItems.AsNoTracking()
            .Where(e => e.UserId == userId && e.DeletedAt == null && e.Status == ExpenseStatus.Active && e.StartDate.Date <= today.Date)
            .ToListAsync(cancellationToken);

        var total = items.Sum(i => _calculator.CalculateDailyCost(i, user, today));
        return Result<TodayResponseDto>.Ok(new TodayResponseDto(today.Date, decimal.Round(total, 2, MidpointRounding.AwayFromZero), items.Count, user.Currency));
    }

    public async Task<Result<TrendResponseDto>> GetTrendAsync(Guid userId, int days, CancellationToken cancellationToken)
    {
        days = Math.Clamp(days, 1, 365);

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<TrendResponseDto>.Fail("用户不存在");

        var today = _dateTimeProvider.TodayLocal(user.Timezone).Date;
        var start = today.AddDays(-(days - 1));

        var items = await _db.ExpenseItems.AsNoTracking()
            .Where(e => e.UserId == userId && e.DeletedAt == null && e.StartDate.Date <= today)
            .ToListAsync(cancellationToken);

        var points = new List<TrendPointDto>(days);
        for (var d = 0; d < days; d++)
        {
            var date = start.AddDays(d);
            var total = 0m;
            foreach (var item in items)
            {
                if (item.StartDate.Date > date) continue;
                if (item.EndDate is not null && item.EndDate.Value.Date < date) continue;
                if (item.Status == ExpenseStatus.Inactive && item.EndDate is null) continue;
                total += _calculator.CalculateDailyCost(item, user, date);
            }
            points.Add(new TrendPointDto(date, decimal.Round(total, 2, MidpointRounding.AwayFromZero)));
        }

        return Result<TrendResponseDto>.Ok(new TrendResponseDto(days, points));
    }

    public async Task<Result<ByCategoryResponseDto>> GetByCategoryAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<ByCategoryResponseDto>.Fail("用户不存在");

        var today = _dateTimeProvider.TodayLocal(user.Timezone);
        var items = await _db.ExpenseItems.AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.DeletedAt == null && e.Status == ExpenseStatus.Active && e.StartDate.Date <= today.Date)
            .ToListAsync(cancellationToken);

        var totals = items
            .Select(e => new
            {
                e.CategoryId,
                CategoryName = e.Category?.Name ?? "未分类",
                DailyCost = _calculator.CalculateDailyCost(e, user, today)
            })
            .GroupBy(x => new { x.CategoryId, x.CategoryName })
            .Select(g => new { g.Key.CategoryId, g.Key.CategoryName, Total = g.Sum(x => x.DailyCost) })
            .OrderByDescending(x => x.Total)
            .ToList();

        var grand = totals.Sum(x => x.Total);
        var list = totals.Select(x =>
        {
            var percent = grand <= 0 ? 0m : (x.Total / grand) * 100m;
            return new ByCategoryItemDto(x.CategoryId, x.CategoryName, decimal.Round(x.Total, 2, MidpointRounding.AwayFromZero), decimal.Round(percent, 2, MidpointRounding.AwayFromZero));
        }).ToList();

        return Result<ByCategoryResponseDto>.Ok(new ByCategoryResponseDto(today.Date, decimal.Round(grand, 2, MidpointRounding.AwayFromZero), list));
    }
}

