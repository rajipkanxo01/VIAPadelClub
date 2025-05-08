using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;

public interface IScheduleFinder
{
    Result<DailySchedule> FindSchedule(ScheduleId scheduleId);
    void AddSchedule(DailySchedule schedule);    //Todo: Remove Add schedule after session 6
}