using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDailyScheduleRepository
{
    Task<Result> AddAsync(DailySchedule dailySchedule);
    Task<Result<DailySchedule>> GetAsync(Guid scheduleId);
    Task<Result> RemoveAsync(Guid scheduleId);


    // TODO: Remove this method after adding EFC. This method is used only for testing purposes. 
    // Since we don't use EF Core (which provides automatic change tracking),
    // we need this method to explicitly persist changes made to the aggregate.
    // This ensures modified DailySchedule instances are saved to the data store.
    Task<Result> UpdateAsync(DailySchedule dailySchedule);
}