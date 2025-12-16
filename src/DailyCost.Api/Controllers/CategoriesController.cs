using DailyCost.Api.Models;
using DailyCost.Application.DTOs.Category;
using DailyCost.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyCost.Api.Controllers;

[Route("api/v1/categories")]
[Authorize]
public sealed class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> List(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(GetUserId(), cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(result.Data))
            : BadRequest(ApiResponse<IReadOnlyList<CategoryDto>>.Fail(result.Message ?? "获取失败"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(GetUserId(), request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<CategoryDto>.Ok(result.Data))
            : BadRequest(ApiResponse<CategoryDto>.Fail(result.Message ?? "创建失败"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(GetUserId(), id, request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<CategoryDto>.Ok(result.Data))
            : BadRequest(ApiResponse<CategoryDto>.Fail(result.Message ?? "更新失败"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteAsync(GetUserId(), id, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "删除失败"));
    }

    [HttpPut("sort")]
    public async Task<ActionResult<ApiResponse>> Sort([FromBody] SortCategoriesRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.SortAsync(GetUserId(), request, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "排序失败"));
    }
}

