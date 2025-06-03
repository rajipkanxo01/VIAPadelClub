using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace Services.Contracts;

public class ScheduleFinder : IScheduleFinder
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;

    public ScheduleFinder(IDailyScheduleRepository dailyScheduleRepository)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
    }


    public async <Task<Result<DailySchedule>> FindSchedule(ScheduleId scheduleId)
    {
        var schedule = await _dailyScheduleRepository.GetAsync(scheduleId);
        return schedule.Data == null
            ? Result<DailySchedule>.Fail(DailyScheduleError.ScheduleNotFound()._message)
            : Result<DailySchedule>.Ok(schedule.Data);
    }

    public void AddSchedule(DailySchedule schedule)
    {
        _schedules.Add(schedule);
    }
}