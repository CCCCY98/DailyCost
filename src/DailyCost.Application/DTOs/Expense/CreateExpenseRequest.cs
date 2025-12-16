using DailyCost.Domain.Enums;

namespace DailyCost.Application.DTOs.Expense;

public sealed class CreateExpenseRequest
{
    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public Guid? CategoryId { get; set; }

    public DateTime StartDate { get; set; }
    public int? ExpectedDays { get; set; }

    public BillingCycle? BillingCycle { get; set; }
    public bool AutoRenew { get; set; } = true;
    public DateTime? NextRenewalDate { get; set; }

    public CalcMode? CalcMode { get; set; }
    public string? Note { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Tags { get; set; }
}

