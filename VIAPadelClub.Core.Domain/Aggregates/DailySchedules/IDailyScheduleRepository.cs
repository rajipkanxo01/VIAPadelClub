using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDailyScheduleRepository
{
    Task<Result> AddAsync(DailySchedule dailySchedule);
    Task<Result<DailySchedule>> GetAsync(ScheduleId scheduleId);
    Task<Result> RemoveAsync(ScheduleId scheduleId);
    Task<Result<List<DailySchedule>>> GetAllAsync();

}