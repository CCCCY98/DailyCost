using DailyCost.Api.Models;
using DailyCost.Application.Common;
using DailyCost.Application.DTOs.Expense;
using DailyCost.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyCost.Api.Controllers;

[Route("api/v1/expenses")]
[Authorize]
public sealed class ExpensesController : BaseApiController
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ExpenseDto>>>> List([FromQuery] ExpenseListQuery query, CancellationToken cancellationToken)
    {
        var result = await _expenseService.ListAsync(GetUserId(), query, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<PagedResult<ExpenseDto>>.Ok(result.Data))
            : BadRequest(ApiResponse<PagedResult<ExpenseDto>>.Fail(result.Message ?? "获取失败"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> Get(Guid id, CancellationToken cancellationToken)
    {
        var result = await _expenseService.GetAsync(GetUserId(), id, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<ExpenseDto>.Ok(result.Data))
            : NotFound(ApiResponse<ExpenseDto>.Fail(result.Message ?? "不存在"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> Create([FromBody] CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await _expenseService.CreateAsync(GetUserId(), request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<ExpenseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<ExpenseDto>.Fail(result.Message ?? "创建失败"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> Update(Guid id, [FromBody] UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await _expenseService.UpdateAsync(GetUserId(), id, request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<ExpenseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<ExpenseDto>.Fail(result.Message ?? "更新失败"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _expenseService.DeleteAsync(GetUserId(), id, cancellationToken);
        return result.Success ? Ok(ApiResponse.Ok(result.Data)) : BadRequest(ApiResponse.Fail(result.Message ?? "删除失败"));
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> UpdateStatus(Guid id, [FromBody] UpdateExpenseStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _expenseService.UpdateStatusAsync(GetUserId(), id, request, cancellationToken);
        return result.Success && result.Data is not null
            ? Ok(ApiResponse<ExpenseDto>.Ok(result.Data))
            : BadRequest(ApiResponse<ExpenseDto>.Fail(result.Message ?? "更新失败"));
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] ExpenseListQuery query, CancellationToken cancellationToken)
    {
        var result = await _expenseService.ExportCsvAsync(GetUserId(), query, cancellationToken);
        if (!result.Success || result.Data is null) return BadRequest(ApiResponse.Fail(result.Message ?? "导出失败"));
        return File(System.Text.Encoding.UTF8.GetBytes(result.Data), "text/csv; charset=utf-8", "dailycost-expenses.csv");
    }
}

