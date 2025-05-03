using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Court
{
    internal CourtName Name { get; }
    internal ScheduleId ScheduleId { get; set; }
    
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
    
    public override bool Equals(object obj)
    {
        if (obj is not Court other) return false;
        return Name.Value.Equals(other.Name.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Name.Value.GetHashCode();
    }

    public void AssignToSchedule(ScheduleId id)
    {
        ScheduleId = id;
    }
}