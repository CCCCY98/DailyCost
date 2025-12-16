using DailyCost.Domain.Entities;
using DailyCost.Domain.Enums;

namespace DailyCost.Application.Calculations;

public sealed class DailyCostCalculator
{
    public decimal CalculateDailyCost(ExpenseItem item, User user, DateTime today)
    {
        var calcMode = item.CalcMode ?? user.DefaultCalcMode;

        return item.ExpenseType switch
        {
            ExpenseType.FixedAsset => calcMode == CalcMode.Dynamic
                ? CalculateDynamic(item, today)
                : CalculateFixed(item),
            ExpenseType.Subscription => CalculateSubscription(item),
            ExpenseType.Periodic => CalculateSubscription(item),
            _ => 0m
        };
    }

    public int CalculateUsedDays(ExpenseItem item, DateTime today)
    {
        var usedDays = (today.Date - item.StartDate.Date).Days + 1;
        return Math.Max(0, usedDays);
    }

    private static decimal CalculateDynamic(ExpenseItem item, DateTime today)
    {
        var usedDays = (today.Date - item.StartDate.Date).Days + 1;
        if (usedDays <= 0) return item.Amount;
        return item.Amount / usedDays;
    }

    private static decimal CalculateFixed(ExpenseItem item)
    {
        var expectedDays = item.ExpectedDays ?? 365;
        if (expectedDays <= 0) expectedDays = 365;
        return item.Amount / expectedDays;
    }

    private static decimal CalculateSubscription(ExpenseItem item)
    {
        var cycleDays = item.BillingCycle switch
        {
            BillingCycle.Monthly => 30,
            BillingCycle.Quarterly => 90,
            BillingCycle.Yearly => 365,
            _ => 30
        };
        return item.Amount / cycleDays;
    }
}

