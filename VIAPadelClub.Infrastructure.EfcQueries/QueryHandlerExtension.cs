using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.QueryContracts.Contract;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Infrastructure.EfcQueries;

public static class QueryHandlerExtension
{
    public static void RegisterQueryHandler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IQueryHandler<ViewManagerOverview.Query, Result<ViewManagerOverview.Answer>>>();
        serviceCollection.AddSingleton<IQueryHandler<ViewBookingDetails.Query, ViewBookingDetails.Answer>>();
    }
}