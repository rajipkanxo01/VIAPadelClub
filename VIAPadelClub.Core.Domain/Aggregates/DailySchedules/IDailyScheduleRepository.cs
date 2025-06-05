using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDailyScheduleRepository : IGenericRepository<DailySchedule, ScheduleId>
{
    Task<Result<List<DailySchedule>>> GetAllAsync();
}