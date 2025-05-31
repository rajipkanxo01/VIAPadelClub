using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.QueryContracts.QueryDispatching;

namespace VIAPadelClub.Core.QueryContracts;

public static class QueryContractsExtension
{
    public static void RegisterQueryDispatcher(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IQueryDispatcher>(provider =>
        {
            var dispatcher = new QueryDispatcher(provider);
            return dispatcher;
        });
    }
}