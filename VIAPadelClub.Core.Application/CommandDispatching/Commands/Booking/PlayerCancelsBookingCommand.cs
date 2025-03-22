using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;

public class PlayerCancelsBookingCommand
{
    internal  Guid BookingId { get; private set; }
    internal Email PlayerMakingCancel { get; private set; }

    private PlayerCancelsBookingCommand(Guid bookingId, Email playerMakingCancel)
    {
        BookingId = bookingId;
        PlayerMakingCancel = playerMakingCancel;
    }

    public static Result<PlayerCancelsBookingCommand> Create(string id, string playerMakingCancel)
    {
        var bookingId   = Guid.Parse(id);
        var emailResult = Email.Create(playerMakingCancel);
        
        if (emailResult.Success)
        {
            return Result<PlayerCancelsBookingCommand>.Fail(emailResult.ErrorMessage);
        }

        var cancelsBookingCommand = new PlayerCancelsBookingCommand(bookingId, emailResult.Data);
        return Result<PlayerCancelsBookingCommand>.Ok(cancelsBookingCommand);
    }
}