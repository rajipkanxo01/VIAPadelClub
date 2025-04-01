using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Dispatcher;

public interface ICommandDispatcher
{
    Task<Result> DispatchAsync<TCommand>(TCommand command);
}