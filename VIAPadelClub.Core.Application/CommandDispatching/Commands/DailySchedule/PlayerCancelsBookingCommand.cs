using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class PlayerCancelsBookingCommand
{
    internal  Guid BookingId { get; private set; }
    internal Guid DailyScheduleId { get; private set; }
    internal Email PlayerMakingCancel { get; private set; }

    private PlayerCancelsBookingCommand(Guid bookingId, Email playerMakingCancel, Guid dailyScheduleId)
    {
        BookingId = bookingId;
        PlayerMakingCancel = playerMakingCancel;
        DailyScheduleId = dailyScheduleId;
    }   

    public static Result<PlayerCancelsBookingCommand> Create(string id, string playerMakingCancel, string dailyScheduleIdStr)
    {
        var bookingId   = Guid.Parse(id);
        var emailResult = Email.Create(playerMakingCancel);
        var scheduleIdParseResult = Guid.TryParse(dailyScheduleIdStr, out var dailyScheduleId);

        if (!scheduleIdParseResult)
        {
            return Result<PlayerCancelsBookingCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }
        
        if (!emailResult.Success)
        {
            return Result<PlayerCancelsBookingCommand>.Fail(emailResult.ErrorMessage);
        }

        var cancelsBookingCommand = new PlayerCancelsBookingCommand(bookingId, emailResult.Data, dailyScheduleId);
        return Result<PlayerCancelsBookingCommand>.Ok(cancelsBookingCommand);
    }
}