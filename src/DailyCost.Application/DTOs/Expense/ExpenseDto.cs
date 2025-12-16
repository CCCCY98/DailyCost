using DailyCost.Domain.Enums;

namespace DailyCost.Application.DTOs.Expense;

public sealed record ExpenseDto(
    Guid Id,
    string Name,
    decimal Amount,
    ExpenseType ExpenseType,
    string ExpenseTypeName,
    CategoryBriefDto? Category,
    DateTime StartDate,
    DateTime? EndDate,
    int UsedDays,
    int? ExpectedDays,
    BillingCycle? BillingCycle,
    bool AutoRenew,
    DateTime? NextRenewalDate,
    CalcMode? CalcMode,
    ExpenseStatus Status,
    decimal DailyCost,
    string? Note,
    string? ImageUrl,
    List<string> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt);

