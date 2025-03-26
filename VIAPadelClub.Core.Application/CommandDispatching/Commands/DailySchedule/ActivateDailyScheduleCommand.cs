using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class ActivateDailyScheduleCommand
{
    internal Guid ScheduleId { get; private set; }
    
    private ActivateDailyScheduleCommand(Guid scheduleId)
    {
        ScheduleId = scheduleId;
    }
    
    public static Result<ActivateDailyScheduleCommand> Create(string scheduleId)
    {
        if (!Guid.TryParse(scheduleId, out var dailyScheduleIdGuid))
        {
            return Result<ActivateDailyScheduleCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }

        var command = new ActivateDailyScheduleCommand(dailyScheduleIdGuid);
        return Result<ActivateDailyScheduleCommand>.Ok(command);
    }
}