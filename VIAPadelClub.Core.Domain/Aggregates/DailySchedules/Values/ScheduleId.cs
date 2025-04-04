using VIAPadelClub.Core.Domain.Common.BaseClasses;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

public class ScheduleId : ValueObject
{
    public Guid Value { get; private set; }

    protected ScheduleId(Guid value)
    {
        Value = value;
    }

    public static ScheduleId Create() => new(Guid.NewGuid());
    
    public static ScheduleId FromGuid(Guid guid) => new ScheduleId(guid);

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}