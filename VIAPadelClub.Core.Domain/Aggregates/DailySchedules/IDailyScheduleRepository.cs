using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDailyScheduleRepository
{
    Task<Result> AddAsync(DailySchedule dailySchedule);
    Task<Result<DailySchedule>> GetAsync(Guid scheduleId);
    Task<Result> RemoveAsync(Guid scheduleId);
    Task<Result<List<DailySchedule>>> GetAllAsync();

}