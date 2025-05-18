namespace VIAPadelClub.Core.QueryContracts.QueryDispatching;

using Contract;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query)
    {
        Type queryInterfaceWithTypes = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TAnswer));

        dynamic? handler = _serviceProvider.GetService(queryInterfaceWithTypes);

        if (handler == null)
        {
            throw new QueryHandlerNotFoundException(query.GetType().ToString(), typeof(TAnswer).ToString());
        }

        return handler.HandleAsync((dynamic)query);
    }
}