using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features;

public class UnblacklistsPlayerHandler : ICommandHandler<UnblacklistsPlayerCommand>
{
    public Task<Result> HandleAsync(UnblacklistsPlayerCommand command)
    {
        throw new NotImplementedException();
    }
}