using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;

public class SetsPartOfDailyScheduleAsVipOnlyCommand
{
    internal Guid DailyScheduleId { get; private set; }
    internal TimeOnly StartTime { get; private set; }
    internal TimeOnly EndTime { get; private set; }
    
    private SetsPartOfDailyScheduleAsVipOnlyCommand(Guid dailyScheduleId, TimeOnly startTime, TimeOnly endTime)
    {
        DailyScheduleId = dailyScheduleId;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public static Result<SetsPartOfDailyScheduleAsVipOnlyCommand> Create(string dailyScheduleId, string startTime, string endTime)
    {
        var dailyScheduleIdGuid = Guid.Parse(dailyScheduleId);
        var startTimeResult = TimeOnly.Parse(startTime);
        var endTimeResult = TimeOnly.Parse(endTime);
        
        var setsPartOfDailyScheduleAsVipOnlyCommand = new SetsPartOfDailyScheduleAsVipOnlyCommand(dailyScheduleIdGuid, startTimeResult, endTimeResult);
        return Result<SetsPartOfDailyScheduleAsVipOnlyCommand>.Ok(setsPartOfDailyScheduleAsVipOnlyCommand);
    }
}