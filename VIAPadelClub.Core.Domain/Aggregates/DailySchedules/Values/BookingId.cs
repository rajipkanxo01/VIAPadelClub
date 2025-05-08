using VIAPadelClub.Core.Domain.Common.BaseClasses;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

public class BookingId : ValueObject
{
    internal Guid Value { get; }

    private BookingId(Guid value)
    {
        Value = value;
    }

    public static BookingId Create() => new BookingId(Guid.NewGuid());
    
    public static BookingId FromGuid(Guid guid) => new BookingId(guid);

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}