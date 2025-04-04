using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

public interface IScheduleFinder
{
    Result<DailySchedule> FindSchedule(Guid scheduleId);
    void AddSchedule(DailySchedule schedule);    //Todo: Remove Add schedule after session 6
}