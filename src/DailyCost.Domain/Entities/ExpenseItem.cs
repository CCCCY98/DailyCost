using DailyCost.Domain.Enums;

namespace DailyCost.Domain.Entities;

public class ExpenseItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? FamilyId { get; set; }

    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public ExpenseType ExpenseType { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ExpectedDays { get; set; }

    public BillingCycle? BillingCycle { get; set; }
    public bool AutoRenew { get; set; } = true;
    public DateTime? NextRenewalDate { get; set; }

    public CalcMode? CalcMode { get; set; }
    public ExpenseStatus Status { get; set; } = ExpenseStatus.Active;

    public string? Note { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

