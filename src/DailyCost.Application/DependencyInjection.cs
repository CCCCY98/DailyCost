using DailyCost.Application.Calculations;
using DailyCost.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DailyCost.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddSingleton<DailyCostCalculator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        return services;
    }
}
