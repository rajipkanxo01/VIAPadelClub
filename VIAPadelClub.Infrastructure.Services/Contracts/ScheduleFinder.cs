using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Tools.OperationResult;

namespace Services.Contracts;

public class ScheduleFinder : IScheduleFinder
{
    private readonly List<DailySchedule> _schedules = new();
    
    public Result<DailySchedule> FindSchedule(Guid scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        return schedule == null
            ? Result<DailySchedule>.Fail(DailyScheduleError.ScheduleNotFound()._message)
            : Result<DailySchedule>.Ok(schedule);
    }

    public void AddSchedule(DailySchedule schedule)
    {
        _schedules.Add(schedule);
    }
}