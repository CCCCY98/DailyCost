using DailyCost.Application.Abstractions;

namespace DailyCost.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime TodayLocal(string timezone)
    {
        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var local = TimeZoneInfo.ConvertTimeFromUtc(UtcNow, tz);
            return local.Date;
        }
        catch
        {
            return DateTime.UtcNow.Date;
        }
    }
}

