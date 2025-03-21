using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDailyScheduleRepository
{
    Task<Result> AddAsync();
    Task<Result> GetAsync();
    Task<Result> RemoveAsync();
}