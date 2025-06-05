using Microsoft.Extensions.DependencyInjection;
using Services.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using TimeProvider = Services.Contracts.TimeProvider;

namespace Services;

public static class ServiceExtensions
{
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDateProvider, DateProvider>();
        serviceCollection.AddScoped<ITimeProvider, TimeProvider>();
        serviceCollection.AddScoped<IScheduleFinder, ScheduleFinder>(); 
        serviceCollection.AddScoped<IEmailUniqueChecker, EmailUniqueChecker>();
        serviceCollection.AddScoped<IPlayerFinder, PlayerFinder>();
        

    }
}