using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public static class QueryHandlerExtension
{
    public static void RegisterQueryHandler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryHandler<ViewManagerOverview.Query, Result<ViewManagerOverview.Answer>>, ViewManagerOverviewHandler>();
        serviceCollection.AddScoped<IQueryHandler<ViewBookingDetails.Query, Result<ViewBookingDetails.Answer>>, ViewBookingDetailsHandler>();
        serviceCollection.AddScoped<IQueryHandler<PlayerPageOverview.Query, Result<PlayerPageOverview.Answer>>, PlayerPageOverviewQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<PlayerScheduleOverview.Query, Result<PlayerScheduleOverview.Answer>>, PlayerScheduleOverviewQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<ViewSchedulePlayerOverview.Query, Result<ViewSchedulePlayerOverview.Answer>>, ViewSchedulePlayerOverviewQueryHandler>();
        serviceCollection.AddScoped<IQueryHandler<ViewBookingDetails.Query, Result<ViewBookingDetails.Answer>>, ViewBookingDetailsHandler>();
    }
}