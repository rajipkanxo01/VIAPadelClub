using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Court
{
    internal CourtName Name { get; }
    internal ScheduleId ScheduleId { get; private set;  }

    private Court() // for efc
    {
    }

    private Court(CourtName name)
    {
        Name = name;
    }

    public static Result<Court> Create(CourtName name)
    {
        var court = new Court(name);
        return Result<Court>.Ok(court);
    }
    
    internal void AssignSchedule(ScheduleId scheduleId)
    {
        // ScheduleId = scheduleId;
    }
}