using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers.Repository;

public class FakeDailyScheduleRepository : IDailyScheduleRepository
{
    private readonly List<DailySchedule> _listOfSchedules = new();

    public Task<Result> AddAsync(DailySchedule dailySchedule)
    {
        _listOfSchedules.Add(dailySchedule);
        return Task.FromResult(Result.Ok());
    }

    public Task<Result<DailySchedule>> GetAsync(ScheduleId id)
    {
        var schedule = _listOfSchedules.FirstOrDefault(s => s.scheduleId.Equals(id));

        if (schedule is null)
        {
            return Task.FromResult(Result<DailySchedule>.Fail(DailyScheduleError.ScheduleNotFound()._message));
        }

        return Task.FromResult(Result<DailySchedule>.Ok(schedule));
    }

    public Task<Result> RemoveAsync(ScheduleId id)
    {
        var schedule = _listOfSchedules.FirstOrDefault(s => s.scheduleId.Equals(id));

        if (schedule is null)
        {
            return Task.FromResult(Result.Fail(DailyScheduleError.ScheduleNotFound()._message));
        }

        _listOfSchedules.Remove(schedule);
        return Task.FromResult(Result.Ok());
    }
    
    public Task<Result<List<DailySchedule>>> GetAllAsync()
    {
        return Task.FromResult(Result<List<DailySchedule>>.Ok(_listOfSchedules));
    }
}