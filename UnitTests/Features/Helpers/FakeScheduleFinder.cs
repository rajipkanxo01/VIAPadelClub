using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers;

// better to use Moq instead of this
public class FakeScheduleFinder : IScheduleFinder
{
    private readonly List<DailySchedule> _schedules = new();

    public Result<DailySchedule> FindSchedule(Guid scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.scheduleId == scheduleId);
        return schedule == null
            ? Result<DailySchedule>.Fail(ErrorMessage.ScheduleNotFound()._message)
            : Result<DailySchedule>.Ok(schedule);
    }

    public void AddSchedule(DailySchedule schedule)
    {
            _schedules.Add(schedule);
    }
    
}