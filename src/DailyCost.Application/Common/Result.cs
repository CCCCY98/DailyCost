namespace DailyCost.Application.Common;

public sealed record Result<T>(bool Success, T? Data, string? Message = null)
{
    public static Result<T> Ok(T data) => new(true, data);
    public static Result<T> Fail(string message) => new(false, default, message);
}

