using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.Helpers;

public class FakeScheduleFinder : IScheduleFinder
{
    private readonly IDailyScheduleRepository _dailyScheduleRepository;
    
    public FakeScheduleFinder(IDailyScheduleRepository dailyScheduleRepository)
    {
        _dailyScheduleRepository = dailyScheduleRepository;
    }
    
    public Result<DailySchedule> FindSchedule(Guid scheduleId)
    {
        var result = _dailyScheduleRepository.GetAsync(scheduleId).Result;
        
        if (!result.Success)
        {
            return Result<DailySchedule>.Fail(result.ErrorMessage);
        }
        
        return result;
    }

    public void AddSchedule(DailySchedule schedule)
    {
        _dailyScheduleRepository.AddAsync(schedule);
    }
}