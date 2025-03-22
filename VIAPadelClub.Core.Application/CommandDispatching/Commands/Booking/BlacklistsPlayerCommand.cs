using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;

public class BlacklistsPlayerCommand(Email playerId, Guid dailyScheduleId)
{
    internal Email PlayerId { get; private set; } = playerId;
    internal Guid DailyScheduleId { get; private set; } = dailyScheduleId;

    public static Result<BlacklistsPlayerCommand> Create(string scheduleId, string playerId)
    {
        if (!Guid.TryParse(scheduleId, out var dailyScheduleIdGuid))
        {
            return Result<BlacklistsPlayerCommand>.Fail(ErrorMessage.InvalidScheduleIdFormatWhileParsing()._message);
        }

        var emailResult = Email.Create(playerId);

        if (!emailResult.Success)
        {
            return Result<BlacklistsPlayerCommand>.Fail(emailResult.ErrorMessage);
        }
        

        var command = new BlacklistsPlayerCommand( emailResult.Data, dailyScheduleIdGuid);
        return Result<BlacklistsPlayerCommand>.Ok(command);
    }
}