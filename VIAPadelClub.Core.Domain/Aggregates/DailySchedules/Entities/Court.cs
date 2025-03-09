using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

namespace VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;

public class Court
{
    internal CourtName Name { get; }

    public Court(CourtName name)
    {
        Name = name;
    }
}