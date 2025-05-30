using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public static class RepositoriesExtension
{
    public static void RegisterRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDailyScheduleRepository, DailyScheduleRepository>();
        serviceCollection.AddScoped<IPlayerRepository, PlayerRepository>();

        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}