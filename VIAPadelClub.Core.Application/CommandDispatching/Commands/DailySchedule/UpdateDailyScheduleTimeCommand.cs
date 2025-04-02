using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class UpdateDailyScheduleTimeCommand
{
    internal Guid ScheduleId { get; private set; }
    internal DateOnly NewDate { get; private set; }
    internal TimeOnly NewStartTime { get; private set; }
    internal TimeOnly NewEndTime { get; private set; }

    private UpdateDailyScheduleTimeCommand(Guid scheduleId, DateOnly newDate, TimeOnly newStartTime, TimeOnly newEndTime)
    {
        ScheduleId = scheduleId;
        NewDate = newDate;
        NewStartTime = newStartTime;
        NewEndTime = newEndTime;
    }

    public static Result<UpdateDailyScheduleTimeCommand> Create(string scheduleIdStr, string dateStr, string startTimeStr, string endTimeStr)
    {
        if (!Guid.TryParse(scheduleIdStr, out var scheduleId))
        {
            return Result<UpdateDailyScheduleTimeCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }

        if (!DateOnly.TryParse(dateStr, out var date))
        {
            return Result<UpdateDailyScheduleTimeCommand>.Fail(DailyScheduleError.InvalidDateformatWhileParsing()._message);
        }

        if (!TimeOnly.TryParse(startTimeStr, out var startTime))
        {
            return Result<UpdateDailyScheduleTimeCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }

        if (!TimeOnly.TryParse(endTimeStr, out var endTime))
        {
            return Result<UpdateDailyScheduleTimeCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }

        var command = new UpdateDailyScheduleTimeCommand(scheduleId, date, startTime, endTime);
        return Result<UpdateDailyScheduleTimeCommand>.Ok(command);
    }
}