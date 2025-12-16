using DailyCost.Domain.Enums;

namespace DailyCost.Application.DTOs.Expense;

public sealed class UpdateExpenseStatusRequest
{
    public ExpenseStatus Status { get; set; }
}

