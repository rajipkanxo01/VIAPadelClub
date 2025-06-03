using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace Services.Contracts;

public class ScheduleFinder(IDailyScheduleRepository dailyScheduleRepository) : IScheduleFinder
{
    private List<DailySchedule> _schedules = new();
    
    public async Task<Result<DailySchedule>> FindSchedule(ScheduleId scheduleId)
    {
        var scheduleResult = await dailyScheduleRepository.GetAsync(scheduleId);

        if (!scheduleResult.Success)
        {
            return scheduleResult;
        }
        
        return Result<DailySchedule>.Ok(scheduleResult.Data);
        
        /*return schedule.Data == null
            ? Result<DailySchedule>.Fail(DailyScheduleError.ScheduleNotFound()._message)
            : Result<DailySchedule>.Ok(schedule.Data);*/
    }
    
    public void AddSchedule(DailySchedule schedule)
    {
        
        _schedules.Add(schedule);
    }
}