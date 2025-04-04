using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching;

public interface ICommandHandler<T>
{
    Task<Result> HandleAsync(T command);
}