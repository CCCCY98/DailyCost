using AutoMapper;
using DailyCost.Application.Abstractions;
using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Category;
using DailyCost.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CategoryService(IAppDbContext db, IDateTimeProvider dateTimeProvider, IMapper mapper)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .Where(c => !c.IsDeleted && (c.IsSystem || c.UserId == userId))
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<CategoryDto>>.Ok(categories.Select(_mapper.Map<CategoryDto>).ToList());
    }

    public async Task<Result<CategoryDto>> CreateAsync(Guid userId, CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name.Trim(),
            Icon = request.Icon,
            Color = request.Color,
            SortOrder = request.SortOrder,
            IsSystem = false,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }

    public async Task<Result<CategoryDto>> UpdateAsync(Guid userId, Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        if (category is null) return Result<CategoryDto>.Fail("分类不存在");
        if (category.IsSystem) return Result<CategoryDto>.Fail("系统分类不可修改");
        if (category.UserId != userId) return Result<CategoryDto>.Fail("无权限");

        category.Name = request.Name.Trim();
        category.Icon = request.Icon;
        category.Color = request.Color;
        category.SortOrder = request.SortOrder;
        category.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
    }

    public async Task<Result<object>> DeleteAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        if (category is null) return Result<object>.Ok(new { });
        if (category.IsSystem) return Result<object>.Fail("系统分类不可删除");
        if (category.UserId != userId) return Result<object>.Fail("无权限");

        category.IsDeleted = true;
        category.UpdatedAt = _dateTimeProvider.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Result<object>.Ok(new { });
    }

    public async Task<Result<object>> SortAsync(Guid userId, SortCategoriesRequest request, CancellationToken cancellationToken)
    {
        var ids = request.Items.Select(i => i.Id).ToHashSet();
        var categories = await _db.Categories.Where(c => ids.Contains(c.Id) && !c.IsDeleted).ToListAsync(cancellationToken);

        foreach (var item in request.Items)
        {
            var category = categories.FirstOrDefault(c => c.Id == item.Id);
            if (category is null) continue;
            if (category.IsSystem) continue;
            if (category.UserId != userId) continue;
            category.SortOrder = item.SortOrder;
            category.UpdatedAt = _dateTimeProvider.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result<object>.Ok(new { });
    }
}

