using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Dispatcher;

public class CommandDispatcher(IServiceProvider serviceProvider): ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler.HandleAsync(command);
    }
}