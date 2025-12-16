using DailyCost.Application.Calculations;
using DailyCost.Domain.Entities;
using DailyCost.Domain.Enums;
using Xunit;

namespace DailyCost.UnitTests;

public sealed class DailyCostCalculatorTests
{
    private readonly DailyCostCalculator _calc = new();

    [Fact]
    public void FixedAsset_Dynamic_UsesUsedDays()
    {
        var user = new User { DefaultCalcMode = CalcMode.Dynamic };
        var item = new ExpenseItem
        {
            ExpenseType = ExpenseType.FixedAsset,
            Amount = 9000m,
            StartDate = new DateTime(2025, 1, 1),
            CalcMode = CalcMode.Dynamic
        };

        var today = new DateTime(2025, 1, 10);
        var daily = _calc.CalculateDailyCost(item, user, today);
        Assert.Equal(900m, decimal.Round(daily, 2));
        Assert.Equal(10, _calc.CalculateUsedDays(item, today));
    }

    [Fact]
    public void FixedAsset_Fixed_UsesExpectedDays()
    {
        var user = new User { DefaultCalcMode = CalcMode.Dynamic };
        var item = new ExpenseItem
        {
            ExpenseType = ExpenseType.FixedAsset,
            Amount = 3650m,
            StartDate = new DateTime(2025, 1, 1),
            CalcMode = CalcMode.Fixed,
            ExpectedDays = 365
        };

        var today = new DateTime(2025, 1, 10);
        var daily = _calc.CalculateDailyCost(item, user, today);
        Assert.Equal(10m, decimal.Round(daily, 2));
    }

    [Fact]
    public void Subscription_Monthly_Uses30Days()
    {
        var user = new User { DefaultCalcMode = CalcMode.Dynamic };
        var item = new ExpenseItem
        {
            ExpenseType = ExpenseType.Subscription,
            Amount = 30m,
            BillingCycle = BillingCycle.Monthly,
            StartDate = new DateTime(2025, 1, 1)
        };

        var daily = _calc.CalculateDailyCost(item, user, new DateTime(2025, 1, 10));
        Assert.Equal(1m, decimal.Round(daily, 2));
    }

    [Fact]
    public void Dynamic_WhenStartDateInFuture_ReturnsAmount()
    {
        var user = new User { DefaultCalcMode = CalcMode.Dynamic };
        var item = new ExpenseItem
        {
            ExpenseType = ExpenseType.FixedAsset,
            Amount = 100m,
            StartDate = new DateTime(2025, 2, 1),
            CalcMode = CalcMode.Dynamic
        };

        var daily = _calc.CalculateDailyCost(item, user, new DateTime(2025, 1, 10));
        Assert.Equal(100m, decimal.Round(daily, 2));
        Assert.Equal(0, _calc.CalculateUsedDays(item, new DateTime(2025, 1, 10)));
    }
}

