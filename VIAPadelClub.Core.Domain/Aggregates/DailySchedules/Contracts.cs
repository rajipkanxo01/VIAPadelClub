using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules;

public interface IDateProvider
{
    DateOnly Today();
}

public interface IScheduleRepository
{
    Result<DailySchedule> FindSchedule(Guid scheduleId);
    void AddSchedule(DailySchedule schedule);
}