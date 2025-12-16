using System.Net;
using DailyCost.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyCost.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error");
            await WriteAsync(context, HttpStatusCode.BadRequest, ApiResponse.Fail("数据库操作失败"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var msg = _env.IsDevelopment() ? ex.Message : "服务器内部错误";
            await WriteAsync(context, HttpStatusCode.InternalServerError, ApiResponse.Fail(msg));
        }
    }

    private static async Task WriteAsync(HttpContext ctx, HttpStatusCode status, ApiResponse payload)
    {
        ctx.Response.StatusCode = (int)status;
        ctx.Response.ContentType = "application/json; charset=utf-8";
        await ctx.Response.WriteAsJsonAsync(payload);
    }
}

