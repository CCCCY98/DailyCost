using DailyCost.Domain.Enums;

namespace DailyCost.Application.DTOs.Expense;

public sealed class ExpenseListQuery
{
    public ExpenseStatus? Status { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Keyword { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
}

