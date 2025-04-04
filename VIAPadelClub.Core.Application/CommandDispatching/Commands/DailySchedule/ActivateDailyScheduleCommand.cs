using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class ActivateDailyScheduleCommand
{
    internal ScheduleId ScheduleId { get; private set; }
    
    private ActivateDailyScheduleCommand(ScheduleId scheduleId)
    {
        ScheduleId = scheduleId;
    }
    
    public static Result<ActivateDailyScheduleCommand> Create(string scheduleId)
    {
        if (!Guid.TryParse(scheduleId, out var dailyScheduleIdGuid))
        {
            return Result<ActivateDailyScheduleCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }

        var fromGuid = ScheduleId.FromGuid(dailyScheduleIdGuid);

        var command = new ActivateDailyScheduleCommand(fromGuid);
        return Result<ActivateDailyScheduleCommand>.Ok(command);
    }
}