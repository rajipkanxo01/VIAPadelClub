using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class AddVipTimeSlotCommand
{
    internal ScheduleId DailyScheduleId { get; private set; }
    internal TimeOnly StartTime { get; private set; }
    internal TimeOnly EndTime { get; private set; }
    
    private AddVipTimeSlotCommand(ScheduleId dailyScheduleId, TimeOnly startTime, TimeOnly endTime)
    {
        DailyScheduleId = dailyScheduleId;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public static Result<AddVipTimeSlotCommand> Create(string dailyScheduleIdStr, string startTimeStr, string endTimeStr)
    {
        if (!Guid.TryParse(dailyScheduleIdStr, out var dailyScheduleId))
        {
            return Result<AddVipTimeSlotCommand>.Fail(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message);
        }
        
        if (!TimeOnly.TryParse(startTimeStr, out var startTime))
        {
            return Result<AddVipTimeSlotCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }
        
        if (!TimeOnly.TryParse(endTimeStr, out var endTime))
        {
            return Result<AddVipTimeSlotCommand>.Fail(DailyScheduleError.InvalidTimeformatWhileParsing()._message);
        }
        
        var setsPartOfDailyScheduleAsVipOnlyCommand = new AddVipTimeSlotCommand(ScheduleId.FromGuid(dailyScheduleId), startTime, endTime);
        return Result<AddVipTimeSlotCommand>.Ok(setsPartOfDailyScheduleAsVipOnlyCommand);
    }
}