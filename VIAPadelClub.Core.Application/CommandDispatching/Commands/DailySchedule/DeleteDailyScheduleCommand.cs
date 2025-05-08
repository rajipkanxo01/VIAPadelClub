using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class DeleteDailyScheduleCommand
{
    internal ScheduleId ScheduleId { get; private set; }

    private DeleteDailyScheduleCommand(ScheduleId scheduleId)
    {
        ScheduleId = scheduleId;
    }

    public static Result<DeleteDailyScheduleCommand> Create(string scheduleIdStr)
    {
        if (!Guid.TryParse(scheduleIdStr, out var scheduleId))
        {
            return Result<DeleteDailyScheduleCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }

        return Result<DeleteDailyScheduleCommand>.Ok(new DeleteDailyScheduleCommand(ScheduleId.FromGuid(scheduleId)));
    }
}