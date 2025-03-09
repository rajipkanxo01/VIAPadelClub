using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Court
{
    internal CourtName Name { get; }

    private Court(CourtName name)
    {
        Name = name;
    }

    public static Court Create(CourtName name)
    {
        return new Court(name);
    }
}