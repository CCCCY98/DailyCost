using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Category;

namespace DailyCost.Application.Services;

public interface ICategoryService
{
    Task<Result<IReadOnlyList<CategoryDto>>> GetAllAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<CategoryDto>> CreateAsync(Guid userId, CreateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result<CategoryDto>> UpdateAsync(Guid userId, Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken);
    Task<Result<object>> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken);
    Task<Result<object>> SortAsync(Guid userId, SortCategoriesRequest request, CancellationToken cancellationToken);
}

