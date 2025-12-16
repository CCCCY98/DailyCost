namespace DailyCost.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime TodayLocal(string timezone);
}

