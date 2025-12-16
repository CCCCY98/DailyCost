using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Expense;

namespace DailyCost.Application.Services;

public interface IExpenseService
{
    Task<Result<PagedResult<ExpenseDto>>> ListAsync(Guid userId, ExpenseListQuery query, CancellationToken cancellationToken);
    Task<Result<ExpenseDto>> GetAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<Result<ExpenseDto>> CreateAsync(Guid userId, CreateExpenseRequest request, CancellationToken cancellationToken);
    Task<Result<ExpenseDto>> UpdateAsync(Guid userId, Guid id, UpdateExpenseRequest request, CancellationToken cancellationToken);
    Task<Result<object>> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<Result<ExpenseDto>> UpdateStatusAsync(Guid userId, Guid id, UpdateExpenseStatusRequest request, CancellationToken cancellationToken);
    Task<Result<string>> ExportCsvAsync(Guid userId, ExpenseListQuery query, CancellationToken cancellationToken);
}

