using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class PlayerCancelsBookingCommand
{
    internal  BookingId BookingId { get; private set; }
    internal ScheduleId DailyScheduleId { get; private set; }
    internal Email PlayerMakingCancel { get; private set; }

    private PlayerCancelsBookingCommand(BookingId bookingId, Email playerMakingCancel, ScheduleId dailyScheduleId)
    {
        BookingId = bookingId;
        PlayerMakingCancel = playerMakingCancel;
        DailyScheduleId = dailyScheduleId;
    }   

    public static Result<PlayerCancelsBookingCommand> Create(string id, string playerMakingCancel, string dailyScheduleIdStr)
    {
        var bookingIdGuid   = Guid.Parse(id);
        
        var bookingId = BookingId.FromGuid(bookingIdGuid);
        
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

        var cancelsBookingCommand = new PlayerCancelsBookingCommand(bookingId, emailResult.Data, ScheduleId.FromGuid(dailyScheduleId));
        return Result<PlayerCancelsBookingCommand>.Ok(cancelsBookingCommand);
    }
}