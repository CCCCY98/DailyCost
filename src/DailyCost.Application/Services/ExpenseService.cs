using System.Globalization;
using System.Text;
using System.Text.Json;
using AutoMapper;
using DailyCost.Application.Abstractions;
using DailyCost.Application.Calculations;
using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Expense;
using DailyCost.Domain.Entities;
using DailyCost.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Application.Services;

public sealed class ExpenseService : IExpenseService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DailyCostCalculator _calculator;
    private readonly IMapper _mapper;

    public ExpenseService(IAppDbContext db, IDateTimeProvider dateTimeProvider, DailyCostCalculator calculator, IMapper mapper)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
        _calculator = calculator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ExpenseDto>>> ListAsync(Guid userId, ExpenseListQuery query, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<PagedResult<ExpenseDto>>.Fail("用户不存在");

        var today = _dateTimeProvider.TodayLocal(user.Timezone);

        var q = _db.ExpenseItems
            .AsNoTracking()
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.DeletedAt == null);

        if (query.Status is not null)
            q = q.Where(e => e.Status == query.Status.Value);
        if (query.CategoryId is not null)
            q = q.Where(e => e.CategoryId == query.CategoryId);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(e => e.Name.Contains(keyword));
        }

        var all = await q.ToListAsync(cancellationToken);
        var mapped = all.Select(e => ToDto(e, user, today)).ToList();

        mapped = ApplySorting(mapped, query.SortBy, query.SortOrder);

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 200);
        var total = mapped.Count;
        var items = mapped.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Result<PagedResult<ExpenseDto>>.Ok(new PagedResult<ExpenseDto>(items, total, page, pageSize));
    }

    public async Task<Result<ExpenseDto>> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<ExpenseDto>.Fail("用户不存在");

        var today = _dateTimeProvider.TodayLocal(user.Timezone);

        var item = await _db.ExpenseItems
            .AsNoTracking()
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.DeletedAt == null, cancellationToken);

        if (item is null) return Result<ExpenseDto>.Fail("消费项不存在");
        return Result<ExpenseDto>.Ok(ToDto(item, user, today));
    }

    public async Task<Result<ExpenseDto>> CreateAsync(Guid userId, CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<ExpenseDto>.Fail("用户不存在");

        if (request.CategoryId is not null)
        {
            var category = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(
                c => c.Id == request.CategoryId && !c.IsDeleted && (c.IsSystem || c.UserId == userId),
                cancellationToken);
            if (category is null) return Result<ExpenseDto>.Fail("分类不存在");
        }

        var now = _dateTimeProvider.UtcNow;

        var item = new ExpenseItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId,
            Name = request.Name.Trim(),
            Amount = request.Amount,
            ExpenseType = request.ExpenseType,
            StartDate = request.StartDate.Date,
            ExpectedDays = request.ExpectedDays,
            BillingCycle = request.BillingCycle,
            AutoRenew = request.AutoRenew,
            NextRenewalDate = request.NextRenewalDate?.Date,
            CalcMode = request.CalcMode,
            Note = request.Note,
            ImageUrl = request.ImageUrl,
            Tags = request.Tags is null ? null : JsonSerializer.Serialize(request.Tags, JsonOptions),
            Status = ExpenseStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.ExpenseItems.Add(item);
        await _db.SaveChangesAsync(cancellationToken);

        var today = _dateTimeProvider.TodayLocal(user.Timezone);
        var loaded = await _db.ExpenseItems.AsNoTracking().Include(e => e.Category).FirstAsync(e => e.Id == item.Id, cancellationToken);
        return Result<ExpenseDto>.Ok(ToDto(loaded, user, today));
    }

    public async Task<Result<ExpenseDto>> UpdateAsync(Guid userId, Guid id, UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<ExpenseDto>.Fail("用户不存在");

        var item = await _db.ExpenseItems.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.DeletedAt == null, cancellationToken);
        if (item is null) return Result<ExpenseDto>.Fail("消费项不存在");

        if (request.CategoryId is not null)
        {
            var category = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(
                c => c.Id == request.CategoryId && !c.IsDeleted && (c.IsSystem || c.UserId == userId),
                cancellationToken);
            if (category is null) return Result<ExpenseDto>.Fail("分类不存在");
        }

        item.Name = request.Name.Trim();
        item.Amount = request.Amount;
        item.ExpenseType = request.ExpenseType;
        item.CategoryId = request.CategoryId;
        item.StartDate = request.StartDate.Date;
        item.ExpectedDays = request.ExpectedDays;
        item.BillingCycle = request.BillingCycle;
        item.AutoRenew = request.AutoRenew;
        item.NextRenewalDate = request.NextRenewalDate?.Date;
        item.CalcMode = request.CalcMode;
        item.Note = request.Note;
        item.ImageUrl = request.ImageUrl;
        item.Tags = request.Tags is null ? null : JsonSerializer.Serialize(request.Tags, JsonOptions);
        item.UpdatedAt = _dateTimeProvider.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        var today = _dateTimeProvider.TodayLocal(user.Timezone);
        var loaded = await _db.ExpenseItems.AsNoTracking().Include(e => e.Category).FirstAsync(e => e.Id == item.Id, cancellationToken);
        return Result<ExpenseDto>.Ok(ToDto(loaded, user, today));
    }

    public async Task<Result<object>> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var item = await _db.ExpenseItems.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.DeletedAt == null, cancellationToken);
        if (item is null) return Result<object>.Ok(new { });

        item.DeletedAt = _dateTimeProvider.UtcNow;
        item.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result<object>.Ok(new { });
    }

    public async Task<Result<ExpenseDto>> UpdateStatusAsync(Guid userId, Guid id, UpdateExpenseStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        if (user is null) return Result<ExpenseDto>.Fail("用户不存在");

        var item = await _db.ExpenseItems.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId && e.DeletedAt == null, cancellationToken);
        if (item is null) return Result<ExpenseDto>.Fail("消费项不存在");

        item.Status = request.Status;
        item.EndDate = request.Status == ExpenseStatus.Inactive ? _dateTimeProvider.TodayLocal(user.Timezone).Date : null;
        item.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var today = _dateTimeProvider.TodayLocal(user.Timezone);
        var loaded = await _db.ExpenseItems.AsNoTracking().Include(e => e.Category).FirstAsync(e => e.Id == item.Id, cancellationToken);
        return Result<ExpenseDto>.Ok(ToDto(loaded, user, today));
    }

    public async Task<Result<string>> ExportCsvAsync(Guid userId, ExpenseListQuery query, CancellationToken cancellationToken)
    {
        query.Page = 1;
        query.PageSize = 200000;
        var list = await ListAsync(userId, query, cancellationToken);
        if (!list.Success || list.Data is null) return Result<string>.Fail(list.Message ?? "导出失败");

        var sb = new StringBuilder();
        sb.AppendLine("Name,Amount,ExpenseType,Category,StartDate,Status,DailyCost,UsedDays");
        foreach (var e in list.Data.Items)
        {
            sb.Append(Escape(e.Name)).Append(',');
            sb.Append(e.Amount.ToString(CultureInfo.InvariantCulture)).Append(',');
            sb.Append(e.ExpenseType).Append(',');
            sb.Append(Escape(e.Category?.Name ?? "")).Append(',');
            sb.Append(e.StartDate.ToString("yyyy-MM-dd")).Append(',');
            sb.Append(e.Status).Append(',');
            sb.Append(e.DailyCost.ToString(CultureInfo.InvariantCulture)).Append(',');
            sb.Append(e.UsedDays);
            sb.AppendLine();
        }
        return Result<string>.Ok(sb.ToString());
    }

    private ExpenseDto ToDto(ExpenseItem e, User user, DateTime today)
    {
        var usedDays = _calculator.CalculateUsedDays(e, today);
        var daily = _calculator.CalculateDailyCost(e, user, today);

        var tags = new List<string>();
        if (!string.IsNullOrWhiteSpace(e.Tags))
        {
            try
            {
                tags = JsonSerializer.Deserialize<List<string>>(e.Tags, JsonOptions) ?? new List<string>();
            }
            catch
            {
                tags = new List<string>();
            }
        }

        var category = e.Category is null ? null : _mapper.Map<CategoryBriefDto>(e.Category);

        return new ExpenseDto(
            e.Id,
            e.Name,
            e.Amount,
            e.ExpenseType,
            GetExpenseTypeName(e.ExpenseType),
            category,
            e.StartDate.Date,
            e.EndDate?.Date,
            usedDays,
            e.ExpectedDays,
            e.BillingCycle,
            e.AutoRenew,
            e.NextRenewalDate?.Date,
            e.CalcMode,
            e.Status,
            decimal.Round(daily, 2, MidpointRounding.AwayFromZero),
            e.Note,
            e.ImageUrl,
            tags,
            e.CreatedAt,
            e.UpdatedAt);
    }

    private static List<ExpenseDto> ApplySorting(List<ExpenseDto> list, string? sortBy, string? sortOrder)
    {
        var desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        return (sortBy ?? "").ToLowerInvariant() switch
        {
            "dailycost" => desc ? list.OrderByDescending(x => x.DailyCost).ToList() : list.OrderBy(x => x.DailyCost).ToList(),
            "amount" => desc ? list.OrderByDescending(x => x.Amount).ToList() : list.OrderBy(x => x.Amount).ToList(),
            "createdat" => desc ? list.OrderByDescending(x => x.CreatedAt).ToList() : list.OrderBy(x => x.CreatedAt).ToList(),
            "startdate" => desc ? list.OrderByDescending(x => x.StartDate).ToList() : list.OrderBy(x => x.StartDate).ToList(),
            _ => desc ? list.OrderByDescending(x => x.UpdatedAt).ToList() : list.OrderBy(x => x.UpdatedAt).ToList()
        };
    }

    private static string GetExpenseTypeName(ExpenseType type) => type switch
    {
        ExpenseType.FixedAsset => "固定资产",
        ExpenseType.Subscription => "订阅服务",
        ExpenseType.Periodic => "周期支出",
        _ => "未知"
    };

    private static string Escape(string value)
    {
        if (value.Contains('"')) value = value.Replace("\"", "\"\"");
        if (value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            value = $"\"{value}\"";
        return value;
    }
}

