using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.Features.Booking;

public class PlayerCancelsBookingHandler: ICommandHandler<PlayerCancelsBookingCommand>
{
    public Task<Result> HandleAsync(PlayerCancelsBookingCommand command)
    {
        throw new NotImplementedException();
    }
}