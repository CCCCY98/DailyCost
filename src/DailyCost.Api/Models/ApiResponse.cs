namespace DailyCost.Api.Models;

public sealed class ApiResponse
{
    public bool Success { get; init; }
    public object? Data { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse Ok(object? data) => new() { Success = true, Data = data, Message = null };
    public static ApiResponse Fail(string message, Dictionary<string, string[]>? errors = null) =>
        new() { Success = false, Data = null, Message = message, Errors = errors };
}

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data, Message = null };
    public static ApiResponse<T> Fail(string message, Dictionary<string, string[]>? errors = null) =>
        new() { Success = false, Data = default, Message = message, Errors = errors };
}

