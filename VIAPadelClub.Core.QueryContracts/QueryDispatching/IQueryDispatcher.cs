namespace VIAPadelClub.Core.QueryContracts.QueryDispatching;

using Contract;

public interface IQueryDispatcher
{
    Task<TAnswer> DispatchAsync<TAnswer>(IQuery<TAnswer> query);
}